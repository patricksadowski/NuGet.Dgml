using System;
using System.Runtime.Versioning;
using Xunit;

namespace NuGet.Dgml
{
    public class PackageRepositoryExtensionsFacts
    {
        public class VisualizeUpgradeableDependenciesIPackageRepository
        {
            [Fact]
            public void ThrowsOnNullPackageRepository()
            {
                IPackageRepository repository = null;
                Assert.Throws<ArgumentNullException>(
                    "packageRepository",
                    () => repository.VisualizeUpgradeableDependencies());
            }

            /* No further tests necessary because the method calls already tested methods. */
        }

        public class VisualizeUpgradeableDependenciesIPackageRepositoryFrameworkName
        {
            private readonly FrameworkName _targetFramework;

            public VisualizeUpgradeableDependenciesIPackageRepositoryFrameworkName()
            {
                _targetFramework = new StubFrameworkNameFactory().NET45();
            }

            [Fact]
            public void ThrowsOnNullPackageRepository()
            {
                IPackageRepository repository = null;
                Assert.Throws<ArgumentNullException>(
                    "packageRepository",
                    () => repository.VisualizeUpgradeableDependencies(_targetFramework));
            }

            [Fact]
            public void RecentPackagesAreNodesWithVersion()
            {
                var packageBuilder = new StubPackageBuilder();
                packageBuilder.AddPackageDefinitions("PackageA", "0.1.0", "0.2.0-rc1", "1.0.0");
                packageBuilder.AddPackageDefinitions("PackageB", "1.0.1", "1.0.2", "1.0.3-beta");
                var repository = StubPackageRepositoryFactory.Create(packageBuilder);

                var directedGraph = repository.VisualizeUpgradeableDependencies(_targetFramework);

                Assert.Equal(2, directedGraph.Nodes.Length);
                Assert.Equal("PackageA 1.0.0", directedGraph.Nodes[0].Label);
                Assert.Equal("PackageB 1.0.3-beta", directedGraph.Nodes[1].Label);
                Assert.Null(directedGraph.Links);
            }

            /* No further tests necessary because the method calls already tested methods. */
        }
    }
}
