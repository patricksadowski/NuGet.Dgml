using NSubstitute;
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
                var repository = Substitute.For<IPackageRepository>();
                repository.GetPackages().Returns(Enumerable.Empty<IPackage>().AsQueryable());
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
                var packages = versions.Split(';').Select(version =>
                {
                    var package = Substitute.For<IPackage>();
                    package.Id.Returns("Package");
                    package.Version.Returns(new SemanticVersion(version));
                    return package;
                });

                var repository = Substitute.For<IPackageRepository>();
                repository.GetPackages().Returns(packages.AsQueryable());

                var recentPackages = repository.GetRecentPackages();
                Assert.Equal(expected, recentPackages.Single().Version.ToString());
            }
        }
    }
}
