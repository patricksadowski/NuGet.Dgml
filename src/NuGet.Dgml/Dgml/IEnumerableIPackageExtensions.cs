using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace NuGet.Dgml
{
    /// <summary>
    /// Provides static extension methods for <see cref="IEnumerable{IPackage}"/>.
    /// </summary>
    public static class IEnumerableIPackageExtensions
    {
        /// <summary>
        /// Visualizes the upgradeable dependencies of the specified packages.
        /// </summary>
        /// <param name="packages">The packages.</param>
        /// <param name="packageRepository">The repository to resolve package dependencies.</param>
        /// <returns>The graph of the upgradeable dependencies of the specified packages.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="packages"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="packageRepository"/> is <c>null</c>.</exception>
        /// <seealso cref="VisualizeUpgradeableDependencies(IEnumerable{IPackage}, IPackageRepository, FrameworkName)"/>
        public static DirectedGraph VisualizeUpgradeableDependencies(this IEnumerable<IPackage> packages, IPackageRepository packageRepository)
            => VisualizeUpgradeableDependencies(packages, packageRepository, null);

        /// <summary>
        /// Visualizes the upgradeable dependencies of the specified packages.
        /// </summary>
        /// <param name="packages">The packages.</param>
        /// <param name="packageRepository">The repository to resolve package dependencies.</param>
        /// <param name="targetFramework">The framework to find compatible package dependencies.</param>
        /// <returns>The graph of the upgradeable dependencies of the specified packages.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="packages"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="packageRepository"/> is <c>null</c>.</exception>
        /// <seealso cref="UpgradeWalker"/>
        /// <seealso cref="PackageUpgradeVisualizer"/>
        public static DirectedGraph VisualizeUpgradeableDependencies(
            this IEnumerable<IPackage> packages,
            IPackageRepository packageRepository,
            FrameworkName targetFramework)
        {
            if (packages == null)
            {
                throw new ArgumentNullException(nameof(packages));
            }
            if (packageRepository == null)
            {
                throw new ArgumentNullException(nameof(packageRepository));
            }

            var walker = new UpgradeWalker(packageRepository, targetFramework);

            var directedGraphFactory = new DirectedGraphFactory();
            var directedGraph = directedGraphFactory.CreateDependencyGraph();
            var visualizer = new PackageUpgradeVisualizer(directedGraph);

            foreach (var recentPackage in packages)
            {
                var upgrades = walker.GetPackageUpgrades(recentPackage);
                visualizer.Visualize(recentPackage, upgrades);
            }

            return directedGraph;
        }
    }
}
