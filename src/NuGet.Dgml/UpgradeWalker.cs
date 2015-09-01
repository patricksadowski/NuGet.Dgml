using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace NuGet
{
    /// <summary>
    /// Walks the dependencies of a package and identifies upgradeable dependencies.
    /// </summary>
    public class UpgradeWalker
    {
        private readonly IPackageRepository _packageRepository;
        private readonly FrameworkName _targetFramework;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradeWalker"/> class.
        /// </summary>
        /// <param name="packageRepository">The repository to resolve package dependencies.</param>
        /// <exception cref="ArgumentNullException"><paramref name="packageRepository"/> is <c>null</c>.</exception>
        public UpgradeWalker(IPackageRepository packageRepository)
            : this(packageRepository, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradeWalker"/> class.
        /// </summary>
        /// <param name="packageRepository">The repository to resolve package dependencies.</param>
        /// <param name="targetFramework">The framework to find compatible package dependencies.</param>
        /// <exception cref="ArgumentNullException"><paramref name="packageRepository"/> is <c>null</c>.</exception>
        public UpgradeWalker(IPackageRepository packageRepository, FrameworkName targetFramework)
        {
            if (packageRepository == null)
            {
                throw new ArgumentNullException(nameof(packageRepository));
            }

            _packageRepository = packageRepository;
            _targetFramework = targetFramework;
        }

        /// <summary>
        /// Gets the repository to resolve package dependencies.
        /// </summary>
        public IPackageRepository PackageRepository
        {
            get
            {
                return _packageRepository;
            }
        }

        /// <summary>
        /// Gets the dependency upgrades of the specified package.
        /// </summary>
        /// <param name="package">The package to identify upgrades.</param>
        /// <returns>The package dependency upgrades of the specified package.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="package"/> is <c>null</c>.</exception>
        public IEnumerable<PackageUpgrade> GetPackageUpgrades(IPackage package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            return IdentifyUpgrades(package);
        }

        private IEnumerable<PackageUpgrade> IdentifyUpgrades(IPackage package)
        {
            IList<PackageUpgrade> upgrades = new List<PackageUpgrade>();

            foreach (var dependency in package.GetCompatiblePackageDependencies(_targetFramework))
            {
                var recentDependencyPackage = _packageRepository.FindPackage(dependency.Id);

                var upgradeType = DetectUpgradeAction(dependency, recentDependencyPackage);

                upgrades.Add(new PackageUpgrade(dependency, upgradeType));
            }

            return upgrades;
        }

        private static PackageUpgradeAction DetectUpgradeAction(PackageDependency dependency, IPackage recentPackage)
        {
            var upgradeType = PackageUpgradeAction.None;
            if (dependency.VersionSpec.Satisfies(recentPackage.Version))
            {
                upgradeType = IsMinVersionUpgraeable(dependency, recentPackage)
                        ? PackageUpgradeAction.MinVersion
                        : PackageUpgradeAction.None;
            }
            else
            {
                var fromRelease = DependsOnReleaseVersion(dependency);

                if (recentPackage.IsReleaseVersion())
                {
                    upgradeType = fromRelease ? PackageUpgradeAction.ReleaseToRelease : PackageUpgradeAction.PrereleaseToRelease;
                }
                else
                {
                    upgradeType = fromRelease ? PackageUpgradeAction.ReleaseToPrerelease : PackageUpgradeAction.PrereleaseToPrerelease;
                }
            }

            return upgradeType;
        }

        private static bool IsMinVersionUpgraeable(PackageDependency dependency, IPackage recentPackage)
        {
            return (dependency.VersionSpec.MinVersion != null) && (dependency.VersionSpec.MinVersion < recentPackage.Version);
        }

        private static bool DependsOnReleaseVersion(PackageDependency dependency)
        {
            return (dependency.VersionSpec.MaxVersion != null) &&
                string.IsNullOrEmpty(dependency.VersionSpec.MaxVersion.SpecialVersion) &&
                dependency.VersionSpec.IsMaxInclusive;
        }
    }
}
