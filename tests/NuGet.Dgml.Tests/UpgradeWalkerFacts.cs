using System;
using System.Linq;
using System.Runtime.Versioning;
using Xunit;

namespace NuGet
{
    public class UpgradeWalkerFacts
    {
        public class ConstructorIPackageRepository
        {
            [Fact]
            public void ThrowsOnNull()
            {
                Assert.Throws<ArgumentNullException>("packageRepository", () => new UpgradeWalker(null));
            }
        }

        public class ConstructorIPackageRepositoryFrameworkName
        {
            [Fact]
            public void ThrowsOnNullPackageRepository()
            {
                Assert.Throws<ArgumentNullException>(
                    "packageRepository",
                    () => new UpgradeWalker(null, new FrameworkName(".NET Framework, Version=4.0")));
            }
        }

        public class GetPackageUpgrades
        {
            [Fact]
            public void ThrowsOnNull()
            {
                var walker = new UpgradeWalker(StubPackageRepositoryFactory.Create(Enumerable.Empty<IPackage>()));
                Assert.Throws<ArgumentNullException>("package", () => walker.GetPackageUpgrades(null));
            }

            [Fact]
            public void SatisfiedVersionSpecWithMatchingInclusiveMinVersionIsNotUpgradeable()
            {
                var package = StubPackageFactory.CreatePackage("Package", "1.0.0", StubPackageDependencyFactory.Create("Dependency", "1.0.0"));
                var dependency = StubPackageFactory.CreatePackage("Dependency", "1.0.0");
                var repository = StubPackageRepositoryFactory.Create(new[] { dependency, });
                var walker = new UpgradeWalker(repository);

                var upgrades = walker.GetPackageUpgrades(package);

                Assert.Equal(1, upgrades.Count());
                Assert.Equal(PackageUpgradeAction.None, upgrades.ElementAt(0).Action);
                Assert.Same(dependency, upgrades.ElementAt(0).Package);
            }

            [Fact]
            public void SatisfiedVersionSpecWithNotMatchingInclusiveMinVersionIsUpgradeable()
            {
                var package = StubPackageFactory.CreatePackage("Package", "1.0.0", StubPackageDependencyFactory.Create("Dependency", "1.0.0"));
                var dependency1 = StubPackageFactory.CreatePackage("Dependency", "1.0.0");
                var dependency2 = StubPackageFactory.CreatePackage("Dependency", "2.0.0");
                var repository = StubPackageRepositoryFactory.Create(new[] { dependency1, dependency2, });
                var walker = new UpgradeWalker(repository);

                var upgrades = walker.GetPackageUpgrades(package);

                Assert.Equal(1, upgrades.Count());
                Assert.Equal(PackageUpgradeAction.MinVersion, upgrades.ElementAt(0).Action);
                Assert.Equal(dependency2, upgrades.ElementAt(0).Package);
            }

            [Fact]
            public void SatisfiedVersionSpecWithExclusiveMinVersionIsUpgradeable()
            {
                var package = StubPackageFactory.CreatePackage(
                    "Package",
                    "1.0.0",
                    StubPackageDependencyFactory.Create("Dependency", "1.0.0", null, false, false));
                var dependency = StubPackageFactory.CreatePackage("Dependency", "1.0.1-a");
                var repository = StubPackageRepositoryFactory.Create(new[] { dependency, });
                var walker = new UpgradeWalker(repository);

                var upgrades = walker.GetPackageUpgrades(package);

                Assert.Equal(1, upgrades.Count());
                Assert.Equal(PackageUpgradeAction.MinVersion, upgrades.ElementAt(0).Action);
                Assert.Equal(dependency, upgrades.ElementAt(0).Package);
            }

            [Fact]
            public void IdentifiesReleaseToPrerelease()
            {
                var package = StubPackageFactory.CreatePackage("Exact", "1.0.0", StubPackageDependencyFactory.CreateExact("Dependency", "1.0.0"));
                var dependencyRelease = StubPackageFactory.CreatePackage("Dependency", "1.0.0");
                var dependencyPrerelease = StubPackageFactory.CreatePackage("Dependency", "1.1.0-pre");
                var repository = StubPackageRepositoryFactory.Create(new[] { dependencyRelease, dependencyPrerelease, });
                var walker = new UpgradeWalker(repository);

                var upgrades = walker.GetPackageUpgrades(package);

                Assert.Equal(1, upgrades.Count());
                Assert.Equal(PackageUpgradeAction.ReleaseToPrerelease, upgrades.ElementAt(0).Action);
                Assert.Equal(dependencyPrerelease, upgrades.ElementAt(0).Package);
            }

            [Fact]
            public void IdentifiesReleaseToRelease()
            {
                var package = StubPackageFactory.CreatePackage("Exact", "1.0.0", StubPackageDependencyFactory.CreateExact("Dependency", "1.0.0"));
                var dependency10 = StubPackageFactory.CreatePackage("Dependency", "1.0.0");
                var dependency11 = StubPackageFactory.CreatePackage("Dependency", "1.1.0");
                var repository = StubPackageRepositoryFactory.Create(new[] { dependency10, dependency11, });
                var walker = new UpgradeWalker(repository);

                var upgrades = walker.GetPackageUpgrades(package);

                Assert.Equal(1, upgrades.Count());
                Assert.Equal(PackageUpgradeAction.ReleaseToRelease, upgrades.ElementAt(0).Action);
                Assert.Equal(dependency11, upgrades.ElementAt(0).Package);
            }

            [Fact]
            public void UnsatisfiedVersionSpecWithExclusiveMaxVersionIsPrerelease()
            {
                var package = StubPackageFactory.CreatePackage(
                    "Package",
                    "1.0.0",
                    StubPackageDependencyFactory.Create("Dependency", null, "1.0.0", false, false));
                var dependency = StubPackageFactory.CreatePackage("Dependency", "1.0.0");
                var repository = StubPackageRepositoryFactory.Create(new[] { dependency, });
                var walker = new UpgradeWalker(repository);

                var upgrades = walker.GetPackageUpgrades(package);

                Assert.Equal(1, upgrades.Count());
                Assert.Equal(PackageUpgradeAction.PrereleaseToRelease, upgrades.ElementAt(0).Action);
                Assert.Equal(dependency, upgrades.ElementAt(0).Package);
            }

            [Fact]
            public void IdentifiesPrereleaseToPrerelease()
            {
                var package = StubPackageFactory.CreatePackage("Package", "1.0.0", StubPackageDependencyFactory.CreateExact("Dependency", "1.0.0-alpha"));
                var dependencyAlpha = StubPackageFactory.CreatePackage("Dependency", "1.0.0-alpha");
                var dependencyBeta = StubPackageFactory.CreatePackage("Dependency", "1.0.0-beta");
                var repository = StubPackageRepositoryFactory.Create(new[] { dependencyAlpha, dependencyBeta, });
                var walker = new UpgradeWalker(repository);

                var upgrades = walker.GetPackageUpgrades(package);

                Assert.Equal(1, upgrades.Count());
                Assert.Equal(PackageUpgradeAction.PrereleaseToPrerelease, upgrades.ElementAt(0).Action);
                Assert.Equal(dependencyBeta, upgrades.ElementAt(0).Package);
            }

            [Fact]
            public void IdentifiesPrereleaseToRelease()
            {
                var package = StubPackageFactory.CreatePackage("Exact", "1.0.0", StubPackageDependencyFactory.CreateExact("Dependency", "1.0.0-alpha"));
                var dependencyPrerelease = StubPackageFactory.CreatePackage("Dependency", "1.0.0-alpha");
                var dependencyRelease = StubPackageFactory.CreatePackage("Dependency", "1.0.0");
                var repository = StubPackageRepositoryFactory.Create(new[] { dependencyPrerelease, dependencyRelease, });
                var walker = new UpgradeWalker(repository);

                var upgrades = walker.GetPackageUpgrades(package);

                Assert.Equal(1, upgrades.Count());
                Assert.Equal(PackageUpgradeAction.PrereleaseToRelease, upgrades.ElementAt(0).Action);
                Assert.Equal(dependencyRelease, upgrades.ElementAt(0).Package);
            }
        }
    }
}
