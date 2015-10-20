using System;
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
        /// <seealso cref="UpgradeWalker"/>
        /// <seealso cref="PackageUpgradeVisualizer"/>
        public static DirectedGraph VisualizeUpgradeableDependencies(this IPackageRepository packageRepository)
            => VisualizeUpgradeableDependencies(packageRepository, null);

        /// <summary>
        /// Visualizes the upgradeable dependencies of the packages in the specified repository.
        /// </summary>
        /// <param name="packageRepository">The repository.</param>
        /// <param name="targetFramework">The framework to find compatible package dependencies.</param>
        /// <returns>The graph of the upgradeable dependencies of the packages in the repository.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="packageRepository"/> is <c>null</c>.</exception>
        /// <seealso cref="UpgradeWalker"/>
        /// <seealso cref="PackageUpgradeVisualizer"/>
        public static DirectedGraph VisualizeUpgradeableDependencies(this IPackageRepository packageRepository, FrameworkName targetFramework)
        {
            if (packageRepository == null)
            {
                throw new ArgumentNullException(nameof(packageRepository));
            }

            var walker = new UpgradeWalker(packageRepository, targetFramework);

            var directedGraphFactory = new DirectedGraphFactory();
            var directedGraph = directedGraphFactory.CreateDependencyGraph();
            var visualizer = new PackageUpgradeVisualizer(directedGraph);

            var recentPackages = packageRepository.GetRecentPackages();
            foreach (var recentPackage in recentPackages)
            {
                var upgrades = walker.GetPackageUpgrades(recentPackage);
                visualizer.Visualize(recentPackage, upgrades);
            }

            return directedGraph;
        }
    }
}
