using System;
using System.Runtime.Versioning;

namespace NuGet
{
    internal class StubFrameworkNameFactory
    {
        internal FrameworkName NET40() => NET("4.0");

        internal FrameworkName NET45() => NET("4.5");

        private static FrameworkName NET(string version) => NET(new Version(version));

        private static FrameworkName NET(Version version) => new FrameworkName(".NETFramework", version);
    }
}
