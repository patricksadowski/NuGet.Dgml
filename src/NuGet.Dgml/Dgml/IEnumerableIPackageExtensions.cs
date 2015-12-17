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
        /// <remarks>
        /// The method estimates the impact of an upgrade action. The palette ranges from green to red indicating the risk of an upgrade action.
        /// <list type="table">
        /// <listheader>
        /// <term><see cref="PackageUpgradeAction"/></term>
        /// <term>Color</term>
        /// <term>Risk</term>
        /// </listheader>
        /// <item>
        /// <term><see cref="PackageUpgradeAction.None"/></term>
        /// <term>Black</term>
        /// <term>0</term>
        /// </item>
        /// <item>
        /// <term><see cref="PackageUpgradeAction.MinVersion"/></term>
        /// <term>ForestGreen</term>
        /// <term>1</term>
        /// </item>
        /// <item>
        /// <term><see cref="PackageUpgradeAction.ReleaseToRelease"/></term>
        /// <term>Goldenrod</term>
        /// <term>2</term>
        /// </item>
        /// <item>
        /// <term><see cref="PackageUpgradeAction.PrereleaseToRelease"/></term>
        /// <term>DarkOrange</term>
        /// <term>3</term>
        /// </item>
        /// <item>
        /// <term><see cref="PackageUpgradeAction.PrereleaseToPrerelease"/></term>
        /// <term>OrangeRed</term>
        /// <term>4</term>
        /// </item>
        /// <item>
        /// <term><see cref="PackageUpgradeAction.ReleaseToPrerelease"/></term>
        /// <term>Firebrick</term>
        /// <term>5</term>
        /// </item>
        /// <item>
        /// <term><see cref="PackageUpgradeAction.Unknown"/></term>
        /// <term>DarkGray</term>
        /// <term>-</term>
        /// </item>
        /// </list>
        /// </remarks>
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

            var packageUpgradeActionPalette = new PackageUpgradeActionPalette();
            packageUpgradeActionPalette[PackageUpgradeAction.None] = "Black";
            packageUpgradeActionPalette[PackageUpgradeAction.MinVersion] = "ForestGreen";
            packageUpgradeActionPalette[PackageUpgradeAction.ReleaseToRelease] = "Goldenrod";
            packageUpgradeActionPalette[PackageUpgradeAction.PrereleaseToRelease] = "DarkOrange";
            packageUpgradeActionPalette[PackageUpgradeAction.PrereleaseToPrerelease] = "OrangeRed";
            packageUpgradeActionPalette[PackageUpgradeAction.ReleaseToPrerelease] = "Firebrick";
            packageUpgradeActionPalette[PackageUpgradeAction.Unknown] = "DarkGray";

            var packageUpgradePalette = new PackageUpgradePalette(packageUpgradeActionPalette);
            packageUpgradePalette.MissingPackageColor = "Red";
            packageUpgradePalette.PrereleaseColor = "Gainsboro";

            var visualizer = new PackageUpgradeVisualizer(directedGraph, packageUpgradePalette);

            foreach (var recentPackage in packages)
            {
                var upgrades = walker.GetPackageUpgrades(recentPackage);
                visualizer.Visualize(recentPackage, upgrades);
            }

            return directedGraph;
        }
    }
}
