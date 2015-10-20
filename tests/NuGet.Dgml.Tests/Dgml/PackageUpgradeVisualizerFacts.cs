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
            private readonly DirectedGraph _directedGraph;
            private readonly PackageUpgradeVisualizer _visualizer;

            public Visualize()
            {
                _directedGraph = new DirectedGraph();
                _visualizer = new PackageUpgradeVisualizer(_directedGraph);
            }

            [Fact]
            public void ThrowsOnNullPackage()
            {
                Assert.Throws<ArgumentNullException>("package", () => _visualizer.Visualize(null, Enumerable.Empty<PackageUpgrade>()));
            }

            [Fact]
            public void AcceptsNullUpgrades()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");

                _visualizer.Visualize(package, null);

                Assert.Equal(1, _directedGraph.Nodes.Length);
                Assert.Null(_directedGraph.Links);
            }

            [Fact]
            public void CreatesNodeWithIdAndVersionOfPackage()
            {
                var package = CreatePackage();

                _visualizer.Visualize(package, Enumerable.Empty<PackageUpgrade>());

                var node = _directedGraph.Nodes[0];
                Assert.Equal("A 1.2.4", node.Id);
                Assert.Equal("A 1.2.4", node.Label);
            }

            [Fact]
            public void UsesExistingNodes()
            {
                _directedGraph.Nodes = new[]
                {
                    new DirectedGraphNode { Id = "A 1.2.4", },
                    new DirectedGraphNode { Id = "B 1.1.0", },
                    new DirectedGraphNode { Id = "C", },
                };
                var package = CreatePackage();
                var packageUpgradeWithPackage = CreatePackageUpgradeWithPackage();
                var packageUpgradeWithoutPackage = CreatePackageUpgradeWithoutPackage();

                _visualizer.Visualize(package, new[] { packageUpgradeWithPackage, packageUpgradeWithoutPackage, });

                Assert.Equal(3, _directedGraph.Nodes.Length);
                Assert.Equal(2, _directedGraph.Links.Length);
            }

            [Fact]
            public void ReleasePackageIsNotColoredNode()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");

                _visualizer.Visualize(package, Enumerable.Empty<PackageUpgrade>());

                Assert.Null(_directedGraph.Nodes[0].Background);
            }

            [Fact]
            public void PrereleasePackageIsGainsboroNode()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0-a");

                _visualizer.Visualize(package, Enumerable.Empty<PackageUpgrade>());

                Assert.Equal("Gainsboro", _directedGraph.Nodes[0].Background);
            }

            [Fact]
            public void CreatesNodesForUpgrades()
            {
                var package = CreatePackage();
                var packageUpgradeWithPackage = CreatePackageUpgradeWithPackage();
                var packageUpgradeWithoutPackage = CreatePackageUpgradeWithoutPackage();

                _visualizer.Visualize(package, new[] { packageUpgradeWithPackage, packageUpgradeWithoutPackage, });

                Assert.Equal(3, _directedGraph.Nodes.Length);
            }

            [Fact]
            public void CreatesNodeWithIdAndVersionOfPackageUsedForUpgradeIfExisting()
            {
                var package = CreatePackage();
                var packageUpgrade = CreatePackageUpgradeWithPackage();

                _visualizer.Visualize(package, new[] { packageUpgrade, });

                var node = _directedGraph.Nodes.ElementAt(1);
                Assert.Equal("B 1.1.0", node.Id);
                Assert.Equal("B 1.1.0", node.Label);
            }

            [Fact]
            public void CreatesNodeWithIdOfPackageDependencyIfPackageUsedForUpgradeIsMissing()
            {
                var package = CreatePackage();
                var packageUpgrade = CreatePackageUpgradeWithoutPackage();

                _visualizer.Visualize(package, new[] { packageUpgrade, });

                var node = _directedGraph.Nodes[1];
                Assert.Equal("C", node.Id);
                Assert.Equal("C", node.Label);
            }

            [Fact]
            public void ConnectsNodeOfPackageWithNodeOfDependency()
            {
                var package = CreatePackage();
                var packageUpgradeWithPackage = CreatePackageUpgradeWithPackage();
                var packageUpgradeWithoutPackage = CreatePackageUpgradeWithoutPackage();

                _visualizer.Visualize(package, new[] { packageUpgradeWithPackage, packageUpgradeWithoutPackage, });

                Assert.Equal("A 1.2.4", _directedGraph.Links[0].Source);
                Assert.Equal("B 1.1.0", _directedGraph.Links[0].Target);

                Assert.Equal("A 1.2.4", _directedGraph.Links[1].Source);
                Assert.Equal("C", _directedGraph.Links[1].Target);
            }

            [Fact]
            public void CreatesLinkWithVersionSpecOfPackageDependency()
            {
                var package = CreatePackage();
                var packageUpgrade = CreatePackageUpgradeWithPackage();

                _visualizer.Visualize(package, new[] { packageUpgrade, });

                Assert.Equal("[1.0.0]", _directedGraph.Links[0].Label);
            }

            [Fact]
            public void NoneUpgradeActionIsBlackLink()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");
                var packageUpgrade = new PackageUpgrade(
                    StubPackageDependencyFactory.CreateExact("B", "1.0.0"),
                    PackageUpgradeAction.None,
                    null);

                _visualizer.Visualize(package, new[] { packageUpgrade, });

                Assert.Equal("Black", _directedGraph.Links[0].Stroke);
            }

            [Fact]
            public void MinVersionUpgradeActionIsForestGreenLink()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");
                var packageUpgrade = new PackageUpgrade(
                    StubPackageDependencyFactory.CreateExact("B", "1.0.0"),
                    PackageUpgradeAction.MinVersion,
                    null);

                _visualizer.Visualize(package, new[] { packageUpgrade, });

                Assert.Equal("ForestGreen", _directedGraph.Links[0].Stroke);
            }

            [Fact]
            public void ReleaseToReleaseUpgradeActionIsGoldenrodLink()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");
                var packageUpgrade = new PackageUpgrade(
                    StubPackageDependencyFactory.CreateExact("B", "1.0.0"),
                    PackageUpgradeAction.ReleaseToRelease,
                    null);

                _visualizer.Visualize(package, new[] { packageUpgrade, });

                Assert.Equal("Goldenrod", _directedGraph.Links[0].Stroke);
            }

            [Fact]
            public void PrereleaseToReleaseUpgradeActionIsDarkOrangeLink()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");
                var packageUpgrade = new PackageUpgrade(
                    StubPackageDependencyFactory.CreateExact("B", "1.0.0"),
                    PackageUpgradeAction.PrereleaseToRelease,
                    null);

                _visualizer.Visualize(package, new[] { packageUpgrade, });

                Assert.Equal("DarkOrange", _directedGraph.Links[0].Stroke);
            }

            [Fact]
            public void PrereleaseToPrereleaseUpgradeActionIsOrangeRedLink()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");
                var packageUpgrade = new PackageUpgrade(
                    StubPackageDependencyFactory.CreateExact("B", "1.0.0"),
                    PackageUpgradeAction.PrereleaseToPrerelease,
                    null);

                _visualizer.Visualize(package, new[] { packageUpgrade, });

                Assert.Equal("OrangeRed", _directedGraph.Links[0].Stroke);
            }

            [Fact]
            public void ReleaseToPrereleaseUpgradeActionIsFirebrickLink()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");
                var packageUpgrade = new PackageUpgrade(
                    StubPackageDependencyFactory.CreateExact("B", "1.0.0"),
                    PackageUpgradeAction.ReleaseToPrerelease,
                    null);

                _visualizer.Visualize(package, new[] { packageUpgrade, });

                Assert.Equal("Firebrick", _directedGraph.Links[0].Stroke);
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
