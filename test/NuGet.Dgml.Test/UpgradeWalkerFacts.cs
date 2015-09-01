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
                var package = StubPackageFactory.CreatePackage("Package", "1.0.0", PackageDependencyFactory.Create("Dependency", "1.0.0"));
                var packageBuilder = new StubPackageBuilder();
                packageBuilder.AddPackageDefinition("Dependency", "1.0.0");
                var repository = StubPackageRepositoryFactory.Create(packageBuilder);
                var walker = new UpgradeWalker(repository);

                var upgrades = walker.GetPackageUpgrades(package);

                Assert.Equal(1, upgrades.Count());
                Assert.Equal(PackageUpgradeAction.None, upgrades.ElementAt(0).Action);
            }

            [Fact]
            public void SatisfiedVersionSpecWithNotMatchingInclusiveMinVersionIsUpgradeable()
            {
                var package = StubPackageFactory.CreatePackage("Package", "1.0.0", PackageDependencyFactory.Create("Dependency", "1.0.0"));
                var packageBuilder = new StubPackageBuilder();
                packageBuilder.AddPackageDefinitions("Dependency", "1.0.0", "2.0.0");
                var repository = StubPackageRepositoryFactory.Create(packageBuilder);
                var walker = new UpgradeWalker(repository);

                var upgrades = walker.GetPackageUpgrades(package);

                Assert.Equal(1, upgrades.Count());
                Assert.Equal(PackageUpgradeAction.MinVersion, upgrades.ElementAt(0).Action);
            }

            [Fact]
            public void SatisfiedVersionSpecWithExclusiveMinVersionIsUpgradeable()
            {
                var package = StubPackageFactory.CreatePackage(
                    "Package",
                    "1.0.0",
                    PackageDependencyFactory.Create("Dependency", "1.0.0", null, false, false));
                var packageBuilder = new StubPackageBuilder();
                packageBuilder.AddPackageDefinitions("Dependency", "1.0.1-a");
                var repository = StubPackageRepositoryFactory.Create(packageBuilder);
                var walker = new UpgradeWalker(repository);

                var upgrades = walker.GetPackageUpgrades(package);

                Assert.Equal(1, upgrades.Count());
                Assert.Equal(PackageUpgradeAction.MinVersion, upgrades.ElementAt(0).Action);
            }

            [Fact]
            public void IdentifiesReleaseToPrerelease()
            {
                var package = StubPackageFactory.CreatePackage("Exact", "1.0.0", PackageDependencyFactory.CreateExact("Dependency", "1.0.0"));
                var packageBuilder = new StubPackageBuilder();
                packageBuilder.AddPackageDefinitions("Dependency", "1.0.0", "1.1.0-pre");
                var repository = StubPackageRepositoryFactory.Create(packageBuilder);
                var walker = new UpgradeWalker(repository);

                var upgrades = walker.GetPackageUpgrades(package);

                Assert.Equal(1, upgrades.Count());
                Assert.Equal(PackageUpgradeAction.ReleaseToPrerelease, upgrades.ElementAt(0).Action);
            }

            [Fact]
            public void IdentifiesReleaseToRelease()
            {
                var package = StubPackageFactory.CreatePackage("Exact", "1.0.0", PackageDependencyFactory.CreateExact("Dependency", "1.0.0"));
                var packageBuilder = new StubPackageBuilder();
                packageBuilder.AddPackageDefinitions("Dependency", "1.0.0", "1.1.0");
                var repository = StubPackageRepositoryFactory.Create(packageBuilder);
                var walker = new UpgradeWalker(repository);

                var upgrades = walker.GetPackageUpgrades(package);

                Assert.Equal(1, upgrades.Count());
                Assert.Equal(PackageUpgradeAction.ReleaseToRelease, upgrades.ElementAt(0).Action);
            }

            [Fact]
            public void UnsatisfiedVersionSpecWithExclusiveMaxVersionIsPrerelease()
            {
                var package = StubPackageFactory.CreatePackage(
                    "Package",
                    "1.0.0",
                    PackageDependencyFactory.Create("Dependency", null, "1.0.0", false, false));
                var packageBuilder = new StubPackageBuilder();
                packageBuilder.AddPackageDefinitions("Dependency", "1.0.0");
                var repository = StubPackageRepositoryFactory.Create(packageBuilder);
                var walker = new UpgradeWalker(repository);

                var upgrades = walker.GetPackageUpgrades(package);

                Assert.Equal(1, upgrades.Count());
                Assert.Equal(PackageUpgradeAction.PrereleaseToRelease, upgrades.ElementAt(0).Action);
            }

            [Fact]
            public void IdentifiesPrereleaseToPrerelease()
            {
                var package = StubPackageFactory.CreatePackage("Package", "1.0.0", PackageDependencyFactory.CreateExact("Dependency", "1.0.0-alpha"));
                var packageBuilder = new StubPackageBuilder();
                packageBuilder.AddPackageDefinitions("Dependency", "1.0.0-alpha", "1.0.0-beta");
                var repository = StubPackageRepositoryFactory.Create(packageBuilder);
                var walker = new UpgradeWalker(repository);

                var upgrades = walker.GetPackageUpgrades(package);

                Assert.Equal(1, upgrades.Count());
                Assert.Equal(PackageUpgradeAction.PrereleaseToPrerelease, upgrades.ElementAt(0).Action);
            }

            [Fact]
            public void IdentifiesPrereleaseToRelease()
            {
                var package = StubPackageFactory.CreatePackage("Exact", "1.0.0", PackageDependencyFactory.CreateExact("Dependency", "1.0.0-alpha"));
                var packageBuilder = new StubPackageBuilder();
                packageBuilder.AddPackageDefinitions("Dependency", "1.0.0-alpha", "1.0.0");
                var repository = StubPackageRepositoryFactory.Create(packageBuilder);
                var walker = new UpgradeWalker(repository);

                var upgrades = walker.GetPackageUpgrades(package);

                Assert.Equal(1, upgrades.Count());
                Assert.Equal(PackageUpgradeAction.PrereleaseToRelease, upgrades.ElementAt(0).Action);
            }
        }
    }
}
