using System;
using System.Linq;
using System.Runtime.Versioning;
using Xunit;

namespace NuGet.Dgml
{
    public class PackageExtensionsFacts
    {
        public class VisualizeUpgradeableDependenciesIPackageIPackageRepository
        {
            [Fact]
            public void ThrowsOnNullPackage()
            {
                IPackage package = null;
                var repository = StubPackageRepositoryFactory.Create(Enumerable.Empty<IPackage>());
                Assert.Throws<ArgumentNullException>(
                    "package",
                    () => package.VisualizeUpgradeableDependencies(repository));
            }

            [Fact]
            public void ThrowsOnNullPackageRepository()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");
                Assert.Throws<ArgumentNullException>(
                    "packageRepository",
                    () => package.VisualizeUpgradeableDependencies(null));
            }

            /* No further tests necessary because the method calls already tested methods. */
        }

        public class VisualizeUpgradeableDependenciesIPackageIPackageRepositoryFrameworkName
        {
            private readonly FrameworkName _targetFramework;

            public VisualizeUpgradeableDependenciesIPackageIPackageRepositoryFrameworkName()
            {
                _targetFramework = new StubFrameworkNameFactory().NET45();
            }

            [Fact]
            public void ThrowsOnNullPackage()
            {
                IPackage package = null;
                var repository = StubPackageRepositoryFactory.Create(Enumerable.Empty<IPackage>());
                Assert.Throws<ArgumentNullException>(
                    "package",
                    () => package.VisualizeUpgradeableDependencies(repository, _targetFramework));
            }

            [Fact]
            public void ThrowsOnNullPackageRepository()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");
                Assert.Throws<ArgumentNullException>(
                    "packageRepository",
                    () => package.VisualizeUpgradeableDependencies(null, _targetFramework));
            }

            [Fact]
            public void PackageIsNodeWithVersion()
            {
                var package = StubPackageFactory.CreatePackage("A", "1.0.0");
                var repository = StubPackageRepositoryFactory.Create(Enumerable.Empty<IPackage>());

                var directedGraph = package.VisualizeUpgradeableDependencies(repository, _targetFramework);

                Assert.Equal(1, directedGraph.Nodes.Length);
                Assert.Equal("A 1.0.0", directedGraph.Nodes[0].Label);
                Assert.Null(directedGraph.Links);
            }

            /* No further tests necessary because the method calls already tested methods. */
        }
    }
}
