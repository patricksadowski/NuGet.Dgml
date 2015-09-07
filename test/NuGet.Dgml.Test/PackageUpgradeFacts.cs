using Xunit;

namespace NuGet
{
    public class PackageUpgradeFacts
    {
        public class Constructor
        {
            [Fact]
            public void AcceptsNull()
            {
                new PackageUpgrade(null, PackageUpgradeAction.None);
            }
        }

        public class PackageDependency
        {
            [Fact]
            public void ReturnsConstructorParameter()
            {
                var packageDependency = StubPackageDependencyFactory.CreateExact("A", "1.0.0");
                var packageUpgrade = new PackageUpgrade(packageDependency, PackageUpgradeAction.None);
                Assert.Same(packageDependency, packageUpgrade.PackageDependency);
            }
        }

        public class Action
        {
            [Fact]
            public void ReturnsConstructorParameter()
            {
                var packageUpgrade = new PackageUpgrade(null, PackageUpgradeAction.PrereleaseToRelease);
                Assert.Equal(PackageUpgradeAction.PrereleaseToRelease, packageUpgrade.Action);
            }
        }

        public new class ToString
        {
            [Fact]
            public void ConsistsOfPackageDependencyAndAction()
            {
                PackageUpgrade packageUpgrade;

                packageUpgrade = new PackageUpgrade(StubPackageDependencyFactory.CreateExact("A", "1.0.0"), PackageUpgradeAction.MinVersion);
                Assert.Equal("A (= 1.0.0) MinVersion", packageUpgrade.ToString());

                packageUpgrade = new PackageUpgrade(StubPackageDependencyFactory.Create("B", "1.0.0", "2.0.0"), PackageUpgradeAction.ReleaseToPrerelease);
                Assert.Equal("B (≥ 1.0.0 && < 2.0.0) ReleaseToPrerelease", packageUpgrade.ToString());
            }
        }
    }
}
