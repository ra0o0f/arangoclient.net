using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class EdgeVertexResult<TVertex, TEdge>
    {
        public TEdge Edge { get; set; }

        public TVertex Vertex { get; set; }
    }
    
    public class AQLTraversalResult<TVertex, TEdge>
    {
        public TVertex Vertex { get; set; }

        public TraversalVisitedPathResult<TVertex, TEdge> Path { get; set; }
    }
    
    public class ShortestPathResult<TVertex, TEdge>
    {
        public IList<TVertex> Vertices { get; set; }

        public IList<TEdge> Edges { get; set; }

        public int Distance { get; set; }
    }

    public class PathResult<TVertex, TEdge>
    {
        public IList<TVertex> Vertices { get; set; }

        public IList<TEdge> Edges { get; set; }

        public TVertex Source { get; set; }

        public TVertex Destination { get; set; }
    }
}
