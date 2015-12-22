using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace NuGet.Dgml
{
    /// <summary>
    /// Provides static extension methods for <see cref="IPackageRepository"/>.
    /// </summary>
    public static class PackageRepositoryExtensions
    {
        /// <summary>
        /// Visualizes the upgradeable dependencies of the packages in the specified repository.
        /// </summary>
        /// <param name="packageRepository">The repository.</param>
        /// <returns>The graph of the upgradeable dependencies of the packages in the repository.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="packageRepository"/> is <c>null</c>.</exception>
        /// <seealso cref="VisualizeUpgradeableDependencies(IPackageRepository, FrameworkName)"/>
        public static DirectedGraph VisualizeUpgradeableDependencies(this IPackageRepository packageRepository)
            => VisualizeUpgradeableDependencies(packageRepository, null);

        /// <summary>
        /// Visualizes the upgradeable dependencies of the packages in the specified repository.
        /// </summary>
        /// <param name="packageRepository">The repository.</param>
        /// <param name="targetFramework">The framework to find compatible package dependencies.</param>
        /// <returns>The graph of the upgradeable dependencies of the packages in the repository.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="packageRepository"/> is <c>null</c>.</exception>
        /// <seealso cref="IEnumerableIPackageExtensions.VisualizeUpgradeableDependencies(IEnumerable{IPackage}, IPackageRepository, FrameworkName)"/>
        public static DirectedGraph VisualizeUpgradeableDependencies(this IPackageRepository packageRepository, FrameworkName targetFramework)
        {
            var packages = packageRepository.GetRecentPackages();
            return packages.VisualizeUpgradeableDependencies(packageRepository, targetFramework);
        }
    }
}
