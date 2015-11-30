using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]   
    public class EdgeVertexResult<TVertex, TEdge>
    {
        public TEdge Edge { get; set; }

        public TVertex Vertex { get; set; }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class AQLTraversalResult<TVertex, TEdge>
    {
        public TVertex Vertex { get; set; }

        public TraversalVisitedPathResult<TVertex, TEdge> Path { get; set; }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class ShortestPathResult<TVertex, TEdge>
    {
        public List<TVertex> Vertices { get; set; }

        public List<TEdge> Edges { get; set; }

        public int Distance { get; set; }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class PathResult<TVertex, TEdge>
    {
        public List<TVertex> Vertices { get; set; }

        public List<TEdge> Edges { get; set; }

        public TVertex Source { get; set; }

        public TVertex Destination { get; set; }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class GraphCommonNeighborsResult<TVertex>
    {
        public string Left { get; set; }

        public string Right { get; set; }

        public List<TVertex> Neighbors { get; set; }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class GraphDistanceToResult
    {
        public string StartVertex { get; set; }

        public string Vertex { get; set; }

        public int Distance { get; set; }
    }
}
