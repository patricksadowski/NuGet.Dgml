using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGet.Dgml
{
    /// <summary>
    /// Visualizes packages upgrades in a directed graph.
    /// </summary>
    public class PackageUpgradeVisualizer
    {
        private readonly DirectedGraph directedGraph;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageUpgradeVisualizer"/> class.
        /// </summary>
        /// <param name="directedGraph">The directed graph used to visualize package upgrades.</param>
        /// <exception cref="ArgumentNullException"><paramref name="directedGraph"/> is <c>null</c>.</exception>
        public PackageUpgradeVisualizer(DirectedGraph directedGraph)
        {
            if (directedGraph == null)
            {
                throw new ArgumentNullException(nameof(directedGraph));
            }

            this.directedGraph = directedGraph;
        }

        /// <summary>
        /// Visualizes the specified package and its upgrades in the directed graph.
        /// </summary>
        /// <param name="package">The package to visualize.</param>
        /// <param name="upgrades">The upgrades of the specified package.</param>
        /// <exception cref="ArgumentNullException"><paramref name="package"/> is <c>null</c>.</exception>
        public void Visualize(IPackage package, IEnumerable<PackageUpgrade> upgrades)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            var packageNode = EnsurePackageNode(package);
            ConfigurePackageNode(packageNode, package);

            if (upgrades != null)
            {
                foreach (var upgrade in upgrades)
                {
                    var dependencyNode = EnsureDependencyNode(upgrade);
                    var link = CreateLink(packageNode, dependencyNode);
                    ConfigureLink(link, upgrade);
                }
            }
        }

        private DirectedGraphNode EnsurePackageNode(IPackage package)
        {
            var nodeId = GetNodeId(package);
            return EnsureNode(nodeId);
        }

        private static void ConfigurePackageNode(DirectedGraphNode node, IPackage package)
        {
            if (!package.IsReleaseVersion())
            {
                node.Background = "Gainsboro";
            }
        }

        private DirectedGraphNode EnsureDependencyNode(PackageUpgrade upgrade)
        {
            string nodeId = GetNodeId(upgrade);
            return EnsureNode(nodeId);
        }

        private static string GetNodeId(PackageUpgrade upgrade) => upgrade.Package != null
                ? GetNodeId(upgrade.Package)
                : GetNodeId(upgrade.PackageDependency);

        private static string GetNodeId(IPackage package) => package.GetFullName();

        private static string GetNodeId(PackageDependency packageDependency) => packageDependency.Id;

        private DirectedGraphNode EnsureNode(string id)
        {
            EnsureNodes();

            var node = GetNode(id);
            if (node == null)
            {
                node = CreateNode(id);
                node.Label = id;
                AddNode(node);
            }

            return node;
        }

        private void EnsureNodes()
        {
            if (directedGraph.Nodes == null)
            {
                directedGraph.Nodes = new DirectedGraphNode[0];
            }
        }

        private DirectedGraphNode GetNode(string packageId) => directedGraph.Nodes.FirstOrDefault(n => packageId.Equals(n.Id));

        private DirectedGraphNode CreateNode(string packageId)
        {
            var node = new DirectedGraphNode();
            node.Id = packageId;
            return node;
        }

        private void AddNode(DirectedGraphNode node)
        {
            var nodes = directedGraph.Nodes;
            var nodeIndex = nodes.Length;
            Array.Resize(ref nodes, nodeIndex + 1);
            nodes[nodeIndex] = node;
            directedGraph.Nodes = nodes;
        }

        private DirectedGraphLink CreateLink(DirectedGraphNode source, DirectedGraphNode target)
        {
            var link = new DirectedGraphLink();
            link.Source = source.Id;
            link.Target = target.Id;
            AddLink(link);
            return link;
        }

        private void AddLink(DirectedGraphLink link)
        {
            EnsureLinks();

            var links = directedGraph.Links;
            var linkIndex = links.Length;
            Array.Resize(ref links, linkIndex + 1);
            links[linkIndex] = link;
            directedGraph.Links = links;
        }

        private void EnsureLinks()
        {
            if (directedGraph.Links == null)
            {
                directedGraph.Links = new DirectedGraphLink[0];
            }
        }

        private void ConfigureLink(DirectedGraphLink link, PackageUpgrade upgrade)
        {
            link.Label = upgrade.PackageDependency.VersionSpec.ToString();
            link.Stroke = GetStroke(upgrade.Action);
        }

        private string GetStroke(PackageUpgradeAction action)
        {
            switch (action)
            {
                case PackageUpgradeAction.None:
                    return "Black";
                case PackageUpgradeAction.MinVersion:
                    return "ForestGreen";
                case PackageUpgradeAction.ReleaseToRelease:
                    return "Goldenrod";
                case PackageUpgradeAction.PrereleaseToRelease:
                    return "DarkOrange";
                case PackageUpgradeAction.PrereleaseToPrerelease:
                    return "OrangeRed";
                case PackageUpgradeAction.ReleaseToPrerelease:
                    return "Firebrick";
            }
            return null;
        }
    }
}
