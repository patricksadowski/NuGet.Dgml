using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;

namespace NuGet.Dgml
{
    /// <summary>
    /// Provides static extension methods for <see cref="SourceRepository"/>.
    /// </summary>
    public static class SourceRepositoryExtensions
    {
        /// <summary>
        /// Visualizes the upgradeable dependencies of the packages in the specified repository.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <returns>The graph of the upgradeable dependencies of the packages in the repository.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="repository"/> is <c>null</c>.</exception>
        /// <seealso cref="VisualizeUpgradeableDependenciesAsync(SourceRepository, NuGetFramework)"/>
        public static Task<DirectedGraph> VisualizeUpgradeableDependenciesAsync(this SourceRepository repository) 
            => VisualizeUpgradeableDependenciesAsync(repository, NuGetFramework.AnyFramework);

        /// <summary>
        /// Visualizes the upgradeable dependencies of the packages in the specified repository.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="targetFramework">The framework to find compatible package dependencies.</param>
        /// <returns>The graph of the upgradeable dependencies of the packages in the repository.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="repository"/> is <c>null</c>.</exception>
        /// <seealso cref="IEnumerablePackageIdentityExtensions.VisualizeUpgradeableDependenciesAsync(IEnumerable{PackageIdentity}, SourceRepository, NuGetFramework)"/>
        public static async Task<DirectedGraph> VisualizeUpgradeableDependenciesAsync(this SourceRepository repository, NuGetFramework targetFramework)
        {
            var packages = await repository.GetRecentPackagesAsync().ConfigureAwait(false);
            return await packages.VisualizeUpgradeableDependenciesAsync(repository, targetFramework).ConfigureAwait(false);
        }
    }
}
