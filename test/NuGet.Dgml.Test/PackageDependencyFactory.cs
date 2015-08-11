using NSubstitute;

namespace NuGet
{
    internal static class PackageDependencyFactory
    {
        internal static PackageDependency Create(string id, string version)
        {
            var versionSpec = CreateVersionSpec(version);
            return new PackageDependency(id, versionSpec);
        }

        private static IVersionSpec CreateVersionSpec(string version)
        {
            var versionSpec = Substitute.For<IVersionSpec>();
            versionSpec.IsMinInclusive.Returns(true);
            versionSpec.MinVersion.Returns(new SemanticVersion(version));
            return versionSpec;
        }
    }
}
