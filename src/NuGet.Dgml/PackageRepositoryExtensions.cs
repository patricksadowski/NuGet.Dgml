using System;
using System.Linq;

namespace NuGet
{
    /// <summary>
    /// Provides static extension methods for <see cref="IPackageRepository"/>.
    /// </summary>
    public static class PackageRepositoryExtensions
    {
        /// <summary>
        /// Gets the recent packages of the specified repository.
        /// </summary>
        /// <param name="packageRepository">The repository.</param>
        /// <returns>The recent packages of the repository.</returns>
        public static IQueryable<IPackage> GetRecentPackages(this IPackageRepository packageRepository)
        {
            if (packageRepository == null)
            {
                throw new ArgumentNullException(nameof(packageRepository));
            }

            return packageRepository.GetPackages()
                .Distinct(PackageEqualityComparer.Id)
                .Select(name => packageRepository.FindPackage(name.Id));
        }
    }
}
