namespace NuGet
{
    internal static class StubPackageDependencyFactory
    {
        internal static PackageDependency CreateExact(string id, string version)
        {
            var versionSpec = new VersionSpec(new SemanticVersion(version));
            return new PackageDependency(id, versionSpec);
        }

        internal static PackageDependency Create(string id, string minVersion)
        {
            var versionSpec = CreateVersionSpec(minVersion);
            return new PackageDependency(id, versionSpec);
        }

        internal static PackageDependency Create(string id, string minVersion, string maxVersion)
        {
            var versionSpec = CreateVersionSpec(minVersion, maxVersion);
            return new PackageDependency(id, versionSpec);
        }

        internal static PackageDependency Create(string id, string minVersion, string maxVersion, bool isMinInclusive, bool isMaxInclusive)
        {
            var versionSpec = CreateVersionSpec(minVersion, maxVersion, isMinInclusive, isMaxInclusive);
            return new PackageDependency(id, versionSpec);
        }

        private static IVersionSpec CreateVersionSpec(
            string minVersion = null,
            string maxVersion = null,
            bool isMinInclusive = true,
            bool isMaxInclusive = false)
        {
            var versionSpec = new VersionSpec();
            versionSpec.IsMaxInclusive = isMaxInclusive;
            versionSpec.IsMinInclusive = isMinInclusive;
            if (!string.IsNullOrWhiteSpace(maxVersion))
            {
                versionSpec.MaxVersion = new SemanticVersion(maxVersion);
            }
            if (!string.IsNullOrWhiteSpace(minVersion))
            {
                versionSpec.MinVersion = new SemanticVersion(minVersion);
            }
            return versionSpec;
        }
    }
}
