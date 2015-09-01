using System;
using System.Linq;
using Xunit;

namespace NuGet.Dgml
{
    public class PackageUpgradeVisualizerFacts
    {
        public class Constructor
        {
            [Fact]
            public void ThrowsOnNull()
            {
                Assert.Throws<ArgumentNullException>("directedGraph", () => new PackageUpgradeVisualizer(null));
            }
        }

        public class Visualize
        {
            private readonly DirectedGraph directedGraph;
            private readonly PackageUpgradeVisualizer visualizer;

            public Visualize()
            {
                directedGraph = new DirectedGraph();
                visualizer = new PackageUpgradeVisualizer(directedGraph);
            }

            [Fact]
            public void ThrowsOnNullPackage()
            {
                Assert.Throws<ArgumentNullException>("package", () => visualizer.Visualize(null, Enumerable.Empty<PackageUpgrade>()));
            }

            [Fact]
            public void AcceptsNullUpgrades()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");

                visualizer.Visualize(package, null);

                Assert.Equal(1, directedGraph.Nodes.Length);
                Assert.Null(directedGraph.Links);
            }

            [Fact]
            public void CreatesNodeWithIdAndVersionOfPackage()
            {
                var package = CreatePackage();

                visualizer.Visualize(package, Enumerable.Empty<PackageUpgrade>());

                var node = directedGraph.Nodes[0];
                Assert.Equal("A 1.2.4", node.Id);
                Assert.Equal("A 1.2.4", node.Label);
            }

            [Fact]
            public void UsesExistingNodes()
            {
                directedGraph.Nodes = new[]
                {
                    new DirectedGraphNode { Id = "A 1.2.4", },
                    new DirectedGraphNode { Id = "B 1.1.0", },
                    new DirectedGraphNode { Id = "C", },
                };
                var package = CreatePackage();
                var packageUpgradeWithPackage = CreatePackageUpgradeWithPackage();
                var packageUpgradeWithoutPackage = CreatePackageUpgradeWithoutPackage();

                visualizer.Visualize(package, new[] { packageUpgradeWithPackage, packageUpgradeWithoutPackage, });

                Assert.Equal(3, directedGraph.Nodes.Length);
                Assert.Equal(2, directedGraph.Links.Length);
            }

            [Fact]
            public void ReleasePackageIsNotColoredNode()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");

                visualizer.Visualize(package, Enumerable.Empty<PackageUpgrade>());

                Assert.Null(directedGraph.Nodes[0].Background);
            }

            [Fact]
            public void PrereleasePackageIsGainsboroNode()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0-a");

                visualizer.Visualize(package, Enumerable.Empty<PackageUpgrade>());

                Assert.Equal("Gainsboro", directedGraph.Nodes[0].Background);
            }

            [Fact]
            public void CreatesNodesForUpgrades()
            {
                var package = CreatePackage();
                var packageUpgradeWithPackage = CreatePackageUpgradeWithPackage();
                var packageUpgradeWithoutPackage = CreatePackageUpgradeWithoutPackage();

                visualizer.Visualize(package, new[] { packageUpgradeWithPackage, packageUpgradeWithoutPackage, });

                Assert.Equal(3, directedGraph.Nodes.Length);
            }

            [Fact]
            public void CreatesNodeWithIdAndVersionOfPackageUsedForUpgradeIfExisting()
            {
                var package = CreatePackage();
                var packageUpgrade = CreatePackageUpgradeWithPackage();

                visualizer.Visualize(package, new[] { packageUpgrade, });

                var node = directedGraph.Nodes.ElementAt(1);
                Assert.Equal("B 1.1.0", node.Id);
                Assert.Equal("B 1.1.0", node.Label);
            }

            [Fact]
            public void CreatesNodeWithIdOfPackageDependencyIfPackageUsedForUpgradeIsMissing()
            {
                var package = CreatePackage();
                var packageUpgrade = CreatePackageUpgradeWithoutPackage();

                visualizer.Visualize(package, new[] { packageUpgrade, });

                var node = directedGraph.Nodes[1];
                Assert.Equal("C", node.Id);
                Assert.Equal("C", node.Label);
            }

            [Fact]
            public void ConnectsNodeOfPackageWithNodeOfDependency()
            {
                var package = CreatePackage();
                var packageUpgradeWithPackage = CreatePackageUpgradeWithPackage();
                var packageUpgradeWithoutPackage = CreatePackageUpgradeWithoutPackage();

                visualizer.Visualize(package, new[] { packageUpgradeWithPackage, packageUpgradeWithoutPackage, });

                Assert.Equal("A 1.2.4", directedGraph.Links[0].Source);
                Assert.Equal("B 1.1.0", directedGraph.Links[0].Target);

                Assert.Equal("A 1.2.4", directedGraph.Links[1].Source);
                Assert.Equal("C", directedGraph.Links[1].Target);
            }

            [Fact]
            public void CreatesLinkWithVersionSpecOfPackageDependency()
            {
                var package = CreatePackage();
                var packageUpgrade = CreatePackageUpgradeWithPackage();

                visualizer.Visualize(package, new[] { packageUpgrade, });

                Assert.Equal("[1.0.0]", directedGraph.Links[0].Label);
            }

            [Fact]
            public void NoneUpgradeActionIsBlackLink()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");
                var packageUpgrade = new PackageUpgrade(
                    StubPackageDependencyFactory.CreateExact("B", "1.0.0"),
                    PackageUpgradeAction.None,
                    null);

                visualizer.Visualize(package, new[] { packageUpgrade, });

                Assert.Equal("Black", directedGraph.Links[0].Stroke);
            }

            [Fact]
            public void MinVersionUpgradeActionIsForestGreenLink()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");
                var packageUpgrade = new PackageUpgrade(
                    StubPackageDependencyFactory.CreateExact("B", "1.0.0"),
                    PackageUpgradeAction.MinVersion,
                    null);

                visualizer.Visualize(package, new[] { packageUpgrade, });

                Assert.Equal("ForestGreen", directedGraph.Links[0].Stroke);
            }

            [Fact]
            public void ReleaseToReleaseUpgradeActionIsGoldenrodLink()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");
                var packageUpgrade = new PackageUpgrade(
                    StubPackageDependencyFactory.CreateExact("B", "1.0.0"),
                    PackageUpgradeAction.ReleaseToRelease,
                    null);

                visualizer.Visualize(package, new[] { packageUpgrade, });

                Assert.Equal("Goldenrod", directedGraph.Links[0].Stroke);
            }

            [Fact]
            public void PrereleaseToReleaseUpgradeActionIsDarkOrangeLink()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");
                var packageUpgrade = new PackageUpgrade(
                    StubPackageDependencyFactory.CreateExact("B", "1.0.0"),
                    PackageUpgradeAction.PrereleaseToRelease,
                    null);

                visualizer.Visualize(package, new[] { packageUpgrade, });

                Assert.Equal("DarkOrange", directedGraph.Links[0].Stroke);
            }

            [Fact]
            public void PrereleaseToPrereleaseUpgradeActionIsOrangeRedLink()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");
                var packageUpgrade = new PackageUpgrade(
                    StubPackageDependencyFactory.CreateExact("B", "1.0.0"),
                    PackageUpgradeAction.PrereleaseToPrerelease,
                    null);

                visualizer.Visualize(package, new[] { packageUpgrade, });

                Assert.Equal("OrangeRed", directedGraph.Links[0].Stroke);
            }

            [Fact]
            public void ReleaseToPrereleaseUpgradeActionIsFirebrickLink()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");
                var packageUpgrade = new PackageUpgrade(
                    StubPackageDependencyFactory.CreateExact("B", "1.0.0"),
                    PackageUpgradeAction.ReleaseToPrerelease,
                    null);

                visualizer.Visualize(package, new[] { packageUpgrade, });

                Assert.Equal("Firebrick", directedGraph.Links[0].Stroke);
            }

            private static IPackage CreatePackage()
            {
                return StubPackageFactory.CreatePackage("A", "1.2.4");
            }

            private static PackageUpgrade CreatePackageUpgradeWithoutPackage()
            {
                return new PackageUpgrade(
                    StubPackageDependencyFactory.CreateExact("C", "2.2.0"),
                    PackageUpgradeAction.None,
                    null);
            }

            private static PackageUpgrade CreatePackageUpgradeWithPackage()
            {
                return new PackageUpgrade(
                    StubPackageDependencyFactory.CreateExact("B", "1.0.0"),
                    PackageUpgradeAction.None,
                    StubPackageFactory.CreatePackage("B", "1.1.0"));
            }
        }
    }
}
