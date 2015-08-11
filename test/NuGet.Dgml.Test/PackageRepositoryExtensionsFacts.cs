using System;
using System.Linq;
using Xunit;

namespace NuGet
{
    public class PackageRepositoryExtensionsFacts
    {
        public class GetRecentPackages
        {
            [Fact]
            public void ThrowsOnNull()
            {
                IPackageRepository repository = null;
                Assert.Throws<ArgumentNullException>("packageRepository", () => repository.GetRecentPackages());
            }

            [Fact]
            public void ReturnsEmptyForEmptyPackageRepository()
            {
                var repository = StubPackageRepositoryFactory.Create(Enumerable.Empty<IPackage>());
                Assert.Empty(repository.GetRecentPackages());
            }

            [Theory]
            [InlineData("1.0.0", "1.0.0")]
            [InlineData("2.0.0-beta", "1.0.0;2.0.0-beta")]
            [InlineData("2.0.0-rc", "1.0.0;2.0.0-beta;2.0.0-rc")]
            [InlineData("2.0.0", "1.0.0;2.0.0-beta;2.0.0-rc;2.0.0")]
            [InlineData("2.0.1", "1.0.0;2.0.0-beta;2.0.0-rc;2.0.0;2.0.1")]
            public void ReturnsRecentVersionOfPackage(string expected, string versions)
            {
                var packages = StubPackageFactory.CreatePackages("Package", versions.Split(';'));
                var repository = StubPackageRepositoryFactory.Create(packages);

                var recentPackages = repository.GetRecentPackages();

                Assert.Equal(expected, recentPackages.Single().Version.ToString());
            }
        }
    }
}
