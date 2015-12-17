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

            [Fact]
            public void NoneUpgradeActionIsBlackLink()
            {
                var package = StubPackageFactory.CreatePackage("Package", "1.0.0", StubPackageDependencyFactory.Create("Dependency", "1.0.0"));
                var dependency = StubPackageFactory.CreatePackage("Dependency", "1.0.0");
                var repository = StubPackageRepositoryFactory.Create(new[] { dependency, });

                var directedGraph = new[] { package, }.VisualizeUpgradeableDependencies(repository, _targetFramework);

                Assert.Equal("Black", directedGraph.Links[0].Stroke);
            }

            [Fact]
            public void MinVersionUpgradeActionIsForestGreenLink()
            {
                var package = StubPackageFactory.CreatePackage("Package", "1.0.0", StubPackageDependencyFactory.Create("Dependency", "1.0.0"));
                var dependency1 = StubPackageFactory.CreatePackage("Dependency", "1.0.0");
                var dependency2 = StubPackageFactory.CreatePackage("Dependency", "2.0.0");
                var repository = StubPackageRepositoryFactory.Create(new[] { dependency1, dependency2, });

                var directedGraph = new[] { package, }.VisualizeUpgradeableDependencies(repository, _targetFramework);

                Assert.Equal("ForestGreen", directedGraph.Links[0].Stroke);
            }

            [Fact]
            public void ReleaseToReleaseUpgradeActionIsGoldenrodLink()
            {
                var package = StubPackageFactory.CreatePackage("Exact", "1.0.0", StubPackageDependencyFactory.CreateExact("Dependency", "1.0.0"));
                var dependency10 = StubPackageFactory.CreatePackage("Dependency", "1.0.0");
                var dependency11 = StubPackageFactory.CreatePackage("Dependency", "1.1.0");
                var repository = StubPackageRepositoryFactory.Create(new[] { dependency10, dependency11, });

                var directedGraph = new[] { package, }.VisualizeUpgradeableDependencies(repository, _targetFramework);

                Assert.Equal("Goldenrod", directedGraph.Links[0].Stroke);
            }

            [Fact]
            public void PrereleaseToReleaseUpgradeActionIsDarkOrangeLink()
            {
                var package = StubPackageFactory.CreatePackage("Exact", "1.0.0", StubPackageDependencyFactory.CreateExact("Dependency", "1.0.0-alpha"));
                var dependencyPrerelease = StubPackageFactory.CreatePackage("Dependency", "1.0.0-alpha");
                var dependencyRelease = StubPackageFactory.CreatePackage("Dependency", "1.0.0");
                var repository = StubPackageRepositoryFactory.Create(new[] { dependencyPrerelease, dependencyRelease, });

                var directedGraph = new[] { package, }.VisualizeUpgradeableDependencies(repository, _targetFramework);

                Assert.Equal("DarkOrange", directedGraph.Links[0].Stroke);
            }

            [Fact]
            public void PrereleaseToPrereleaseUpgradeActionIsOrangeRedLink()
            {
                var package = StubPackageFactory.CreatePackage("Package", "1.0.0", StubPackageDependencyFactory.CreateExact("Dependency", "1.0.0-alpha"));
                var dependencyAlpha = StubPackageFactory.CreatePackage("Dependency", "1.0.0-alpha");
                var dependencyBeta = StubPackageFactory.CreatePackage("Dependency", "1.0.0-beta");
                var repository = StubPackageRepositoryFactory.Create(new[] { dependencyAlpha, dependencyBeta, });

                var directedGraph = new[] { package, }.VisualizeUpgradeableDependencies(repository, _targetFramework);

                Assert.Equal("OrangeRed", directedGraph.Links[0].Stroke);
            }

            [Fact]
            public void ReleaseToPrereleaseUpgradeActionIsFirebrickLink()
            {
                var package = StubPackageFactory.CreatePackage("Exact", "1.0.0", StubPackageDependencyFactory.CreateExact("Dependency", "1.0.0"));
                var dependencyRelease = StubPackageFactory.CreatePackage("Dependency", "1.0.0");
                var dependencyPrerelease = StubPackageFactory.CreatePackage("Dependency", "1.1.0-pre");
                var repository = StubPackageRepositoryFactory.Create(new[] { dependencyRelease, dependencyPrerelease, });

                var directedGraph = new[] { package, }.VisualizeUpgradeableDependencies(repository, _targetFramework);

                Assert.Equal("Firebrick", directedGraph.Links[0].Stroke);
            }

            [Fact]
            public void UnkownUpgradeActionIsDarkGrayLink()
            {
                var package = StubPackageFactory.CreatePackage("Exact", "1.0.0", StubPackageDependencyFactory.CreateExact("Dependency", "1.0.0"));
                var repository = StubPackageRepositoryFactory.Create(Enumerable.Empty<IPackage>());

                var directedGraph = new[] { package, }.VisualizeUpgradeableDependencies(repository, _targetFramework);

                Assert.Equal("DarkGray", directedGraph.Links[0].Stroke);
            }

            /* No further tests necessary because the method calls already tested methods. */
        }
    }
}
