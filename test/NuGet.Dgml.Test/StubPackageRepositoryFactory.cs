using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace NuGet
{
    internal static class StubPackageRepositoryFactory
    {
        internal static IPackageRepository Create(StubPackageBuilder packageBuilder)
        {
            var packages = packageBuilder.BuildPackages();
            return Create(packages);
        }

        internal static IPackageRepository Create(IEnumerable<IPackage> packages)
        {
            var repository = Substitute.For<IPackageRepository>();
            repository.GetPackages().Returns(packages.AsQueryable());
            return repository;
        }
    }
}
