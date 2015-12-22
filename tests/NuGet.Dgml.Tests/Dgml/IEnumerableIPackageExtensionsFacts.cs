using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using Xunit;

namespace NuGet.Dgml
{
    public class IEnumerableIPackageExtensionsFacts
    {
        public class VisualizeUpgradeableDependenciesIEnumerableIPackageIPackageRepository
        {
            [Fact]
            public void ThrowsOnNullPackages()
            {
                IEnumerable<IPackage> packages = null;
                var repository = StubPackageRepositoryFactory.Create(Enumerable.Empty<IPackage>());
                Assert.Throws<ArgumentNullException>(
                    "packages",
                    () => packages.VisualizeUpgradeableDependencies(repository));
            }

            [Fact]
            public void ThrowsOnNullPackageRepository()
            {
                var packages = Enumerable.Empty<IPackage>();
                Assert.Throws<ArgumentNullException>(
                    "packageRepository",
                    () => packages.VisualizeUpgradeableDependencies(null));
            }

            /* No further tests necessary because the method calls already tested methods. */
        }

        public class VisualizeUpgradeableDependenciesIEnumerableIPackageIPackageRepositoryFrameworkName
        {
            private readonly FrameworkName _targetFramework;

            public VisualizeUpgradeableDependenciesIEnumerableIPackageIPackageRepositoryFrameworkName()
            {
                _targetFramework = new StubFrameworkNameFactory().NET45();
            }

            [Fact]
            public void ThrowsOnNullPackages()
            {
                IEnumerable<IPackage> packages = null;
                var repository = StubPackageRepositoryFactory.Create(Enumerable.Empty<IPackage>());
                Assert.Throws<ArgumentNullException>(
                    "packages",
                    () => packages.VisualizeUpgradeableDependencies(repository, _targetFramework));
            }

            [Fact]
            public void ThrowsOnNullPackageRepository()
            {
                var packages = Enumerable.Empty<IPackage>();
                Assert.Throws<ArgumentNullException>(
                    "packageRepository",
                    () => packages.VisualizeUpgradeableDependencies(null, _targetFramework));
            }

            [Fact]
            public void PackagesAreNodesWithVersion()
            {
                var packageBuilder = new StubPackageBuilder();
                packageBuilder.AddPackageDefinition("A", "1.0.0");
                packageBuilder.AddPackageDefinition("B", "2.1.0-beta");
                var packages = packageBuilder.BuildPackages();
                var repository = StubPackageRepositoryFactory.Create(Enumerable.Empty<IPackage>());

                var directedGraph = packages.VisualizeUpgradeableDependencies(repository, _targetFramework);

                Assert.Equal(2, directedGraph.Nodes.Length);
                Assert.Equal("A 1.0.0", directedGraph.Nodes[0].Label);
                Assert.Equal("B 2.1.0-beta", directedGraph.Nodes[1].Label);
                Assert.Null(directedGraph.Links);
            }

            /* No further tests necessary because the method calls already tested methods. */
        }
    }
}
