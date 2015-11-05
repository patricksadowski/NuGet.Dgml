using System;
using System.Runtime.Versioning;

namespace NuGet.Dgml
{
    /// <summary>
    /// Provides static extension methods for <see cref="IPackage"/>.
    /// </summary>
    public static class PackageExtensions
    {
        /// <summary>
        /// Visualizes the upgradeable dependencies of the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <param name="packageRepository">The repository to resolve package dependencies.</param>
        /// <returns>The graph of the upgradeable dependencies of the specified package.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="package"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="packageRepository"/> is <c>null</c>.</exception>
        /// <seealso cref="UpgradeWalker"/>
        /// <seealso cref="PackageUpgradeVisualizer"/>
        public static DirectedGraph VisualizeUpgradeableDependencies(this IPackage package, IPackageRepository packageRepository)
            => VisualizeUpgradeableDependencies(package, packageRepository);

        /// <summary>
        /// Visualizes the upgradeable dependencies of the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <param name="packageRepository">The repository to resolve package dependencies.</param>
        /// <param name="targetFramework">The framework to find compatible package dependencies.</param>
        /// <returns>The graph of the upgradeable dependencies of the specified package.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="package"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="packageRepository"/> is <c>null</c>.</exception>
        /// <seealso cref="UpgradeWalker"/>
        /// <seealso cref="PackageUpgradeVisualizer"/>
        public static DirectedGraph VisualizeUpgradeableDependencies(
            this IPackage package,
            IPackageRepository packageRepository,
            FrameworkName targetFramework)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            var walker = new UpgradeWalker(packageRepository, targetFramework);

            var directedGraphFactory = new DirectedGraphFactory();
            var directedGraph = directedGraphFactory.CreateDependencyGraph();
            var visualizer = new PackageUpgradeVisualizer(directedGraph);

            var upgrades = walker.GetPackageUpgrades(package);
            visualizer.Visualize(package, upgrades);

            return directedGraph;
        }
    }
}
