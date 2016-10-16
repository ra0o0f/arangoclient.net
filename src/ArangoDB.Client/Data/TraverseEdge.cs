using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{

    public class TraversalEdgeDefinition
    {
        internal List<TraverseEdge.IEdge> Edges { get; set; }

        public TraversalEdgeDefinition()
        {
            Edges = new List<TraverseEdge.IEdge>();
        }

        public TraversalEdgeDefinition Collection<T>(EdgeDirection direction)
        {
            Edges.Add(new TraverseEdge.Edge<T> { Direction = direction });

            return this;
        }

        public TraversalEdgeDefinition Collection<T>()
        {
            Edges.Add(new TraverseEdge.Edge<T>());

            return this;
        }

        public TraversalEdgeDefinition Collection(string collection, EdgeDirection direction)
        {
            Edges.Add(new TraverseEdge.Edge { Collection = collection, Direction = direction });

            return this;
        }

        public TraversalEdgeDefinition Collection(string collection)
        {
            Edges.Add(new TraverseEdge.Edge { Collection = collection });

            return this;
        }
    }

    public class TraverseEdge
    {
        private TraverseEdge()
        {
        }

        internal interface IEdge
        {
            object GetCollection();

            EdgeDirection? Direction { get; set; }
        }

        internal class Edge : IEdge
        {
            public string Collection { get; set; }

            public EdgeDirection? Direction { get; set; }

            public object GetCollection()
            {
                return Collection;
            }
        }

        internal class Edge<T> : IEdge
        {
            public EdgeDirection? Direction { get; set; }

            public object GetCollection()
            {
                return typeof(T);
            }
        }
        
        public static TraversalEdgeDefinition Collection<T>(EdgeDirection direction)
        {
            var edgeDefinition = new TraversalEdgeDefinition();

            edgeDefinition.Edges.Add(new Edge<T> { Direction = direction });

            return edgeDefinition;
        }

        public static TraversalEdgeDefinition Collection<T>()
        {
            var edgeDefinition = new TraversalEdgeDefinition();

            edgeDefinition.Edges.Add(new Edge<T>());

            return edgeDefinition;
        }

        public static TraversalEdgeDefinition Collection(string collection, EdgeDirection direction)
        {
            var edgeDefinition = new TraversalEdgeDefinition();

            edgeDefinition.Edges.Add(new Edge { Collection = collection, Direction = direction });

            return edgeDefinition;
        }

        public static TraversalEdgeDefinition Collection(string collection)
        {
            var edgeDefinition = new TraversalEdgeDefinition();

            edgeDefinition.Edges.Add(new Edge { Collection = collection });

            return edgeDefinition;
        }
    }
}
