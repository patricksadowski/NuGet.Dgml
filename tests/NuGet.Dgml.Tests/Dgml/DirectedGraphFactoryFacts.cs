using Xunit;

namespace NuGet.Dgml
{
    public class DirectedGraphFactoryFacts
    {
        private readonly DirectedGraphFactory _factory;

        protected DirectedGraphFactory Factory
        {
            get
            {
                return _factory;
            }
        }

        public DirectedGraphFactoryFacts()
        {
            _factory = new DirectedGraphFactory();
        }

        public class Create : DirectedGraphFactoryFacts
        {
            [Fact]
            public void AppliesParametersToDirectedGraph()
            {
                LayoutEnum layout = LayoutEnum.DependencyMatrix;
                GraphDirectionEnum graphDirection = GraphDirectionEnum.BottomToTop;

                var directedGraph = Factory.Create(layout, graphDirection);

                Assert.Equal(layout, directedGraph.Layout);
                Assert.True(directedGraph.LayoutSpecified);
                Assert.Equal(graphDirection, directedGraph.GraphDirection);
                Assert.True(directedGraph.GraphDirectionSpecified);
            }
        }

        public class CreateDependencyGraph : DirectedGraphFactoryFacts
        {
            private readonly DirectedGraph _directedGraph;

            public CreateDependencyGraph()
            {
                _directedGraph = Factory.CreateDependencyGraph();
            }

            [Fact]
            public void LayoutIsSugiyama()
            {
                Assert.Equal(LayoutEnum.Sugiyama, _directedGraph.Layout);
                Assert.True(_directedGraph.LayoutSpecified);
            }

            [Fact]
            public void GraphDirectionIsRightToLeft()
            {
                Assert.Equal(GraphDirectionEnum.RightToLeft, _directedGraph.GraphDirection);
                Assert.True(_directedGraph.GraphDirectionSpecified);
            }
        }
    }
}
