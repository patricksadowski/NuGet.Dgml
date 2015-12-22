﻿using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace NuGet.Dgml
{
    /// <summary>
    /// Provides static extension methods for <see cref="IPackage"/>.
    /// </summary>
    public static class PackageExtensions
    {
        /// <summary>
        /// Visualizes the upgradeable dependencies of the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <param name="packageRepository">The repository to resolve package dependencies.</param>
        /// <returns>The graph of the upgradeable dependencies of the specified package.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="package"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="packageRepository"/> is <c>null</c>.</exception>
        /// <seealso cref="VisualizeUpgradeableDependencies(IPackage, IPackageRepository, FrameworkName)"/>
        public static DirectedGraph VisualizeUpgradeableDependencies(this IPackage package, IPackageRepository packageRepository)
            => VisualizeUpgradeableDependencies(package, packageRepository, null);

        /// <summary>
        /// Visualizes the upgradeable dependencies of the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <param name="packageRepository">The repository to resolve package dependencies.</param>
        /// <param name="targetFramework">The framework to find compatible package dependencies.</param>
        /// <returns>The graph of the upgradeable dependencies of the specified package.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="package"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="packageRepository"/> is <c>null</c>.</exception>
        /// <seealso cref="IEnumerableIPackageExtensions.VisualizeUpgradeableDependencies(IEnumerable{IPackage}, IPackageRepository, FrameworkName)"/>
        public static DirectedGraph VisualizeUpgradeableDependencies(
            this IPackage package,
            IPackageRepository packageRepository,
            FrameworkName targetFramework)
        {
            var packages = new[] { package, };
            return packages.VisualizeUpgradeableDependencies(packageRepository, targetFramework);
        }
    }
}
