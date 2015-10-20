namespace NuGet.Dgml
{
    /// <summary>
    /// Provides instance methods to create instances of <see cref="DirectedGraph"/>.
    /// </summary>
    public class DirectedGraphFactory
    {
        /// <summary>
        /// Creates a <see cref="DirectedGraph"/> with the specified parameters.
        /// </summary>
        /// <param name="layout">The layout of the directed graph.</param>
        /// <param name="graphDirection">The direction of the graph.</param>
        /// <returns>The directed graph configured with the specified parameters.</returns>
        public DirectedGraph Create(LayoutEnum layout, GraphDirectionEnum graphDirection)
        {
            var graph = new DirectedGraph();
            graph.Layout = layout;
            graph.LayoutSpecified = true;
            graph.GraphDirection = graphDirection;
            graph.GraphDirectionSpecified = true;
            return graph;
        }

        /// <summary>
        /// Creates a <see cref="DirectedGraph"/> to visualize dependencies of NuGet packages.
        /// </summary>
        /// <returns></returns>
        public DirectedGraph CreateDependencyGraph() => Create(LayoutEnum.Sugiyama, GraphDirectionEnum.RightToLeft);
    }
}
