using System;
using System.Linq;
using Xunit;

namespace NuGet.Dgml
{
    public class DirectedGraphExtensionsFacts
    {
        public class AsXDocument
        {
            /* Only basic tests were implemented. We assume that XmlSerializer
             * and the xsd tool are working properly.
             */

            [Fact]
            public void ThrowsOnNull()
            {
                DirectedGraph graph = null;
                Assert.Throws<ArgumentNullException>("graph", () => graph.AsXDocument());
            }

            [Fact]
            public void GeneratesXDocument()
            {
                DirectedGraph graph = new DirectedGraph();
                graph.Nodes = new[] { new DirectedGraphNode(), };
                var document = graph.AsXDocument();
                Assert.Equal(1, document.Root.Elements().Count());
            }
        }
    }
}
