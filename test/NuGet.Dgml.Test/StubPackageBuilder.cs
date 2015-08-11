using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGet
{
    internal class StubPackageBuilder
    {
        private readonly IList<Tuple<string, string, IEnumerable<PackageDependency>>> packageDefinitions = new List<Tuple<string, string, IEnumerable<PackageDependency>>>();

        public void AddPackageDefinitions(string id, params string[] versions)
        {
            foreach (var version in versions)
            {
                AddPackageDefinition(id, version);
            }
        }

        public void AddPackageDefinition(string id, string version)
            => AddPackageDefinition(id, version, (IEnumerable<PackageDependency>)null);

        public void AddPackageDefinition(string id, string version, PackageDependency packageDependency)
             => AddPackageDefinition(id, version, new[] { packageDependency, });

        public void AddPackageDefinition(string id, string version, IEnumerable<PackageDependency> packageDependencies)
            => packageDefinitions.Add(Tuple.Create(id, version, packageDependencies));

        public IEnumerable<IPackage> BuildPackages()
            => packageDefinitions.Select(definition => BuildPackage(definition.Item1, definition.Item2, definition.Item3));

        private static IPackage BuildPackage(string id, string version, IEnumerable<PackageDependency> dependencies)
        {
            if (dependencies == null)
            {
                return StubPackageFactory.CreatePackage(id, version);
            }
            else
            {
                return StubPackageFactory.CreatePackage(id, version, dependencies);
            }
        }
    }
}
