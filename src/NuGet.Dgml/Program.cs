using System.Threading.Tasks;
using NuGet.Dgml;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace NuGet
{
    internal static class Program
    {
        private static async Task Main()
        {
            var repository = Repository.Factory.GetCoreV3("https://nexus.nsc-gmbh.de/repository/nuget-public/");
            var directedGraph = await repository.VisualizeUpgradeableDependenciesAsync().ConfigureAwait(false);
            directedGraph.AsXDocument().Save(@"C:\My Package Repository.dgml");
        }
    }
}
