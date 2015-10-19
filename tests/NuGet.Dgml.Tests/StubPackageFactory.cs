using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace NuGet
{
    internal static class StubPackageFactory
    {
        internal static IEnumerable<IPackage> CreatePackages(string id, params string[] versions)
            => versions.Select(version => CreatePackage(id, version));

        internal static IPackage CreatePackage(string id, string version)
            => CreatePackage(id, version, Enumerable.Empty<PackageDependency>());

        internal static IPackage CreatePackage(string id, string version, PackageDependency packageDependency)
            => CreatePackage(id, version, new[] { packageDependency, });

        internal static IPackage CreatePackage(string id, string version, IEnumerable<PackageDependency> packageDependencies)
        {
            var package = Substitute.For<IPackage>();
            package.Id.Returns(id);
            package.Version.Returns(new SemanticVersion(version));
            var packageDependencySet = new PackageDependencySet(null, packageDependencies);
            package.DependencySets.Returns(new[] { packageDependencySet, });
            return package;
        }
    }
}
