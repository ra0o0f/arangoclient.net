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
    public class VertexResult<TVertex>
    {
        public TVertex Vertex { get; set; }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class PathResult<TVertex, TEdge>
    {
        public IList<TVertex> Vertices { get; set; }

        public IList<TEdge> Edges { get; set; }

        public TVertex Source { get; set; }

        public TVertex Destination { get; set; }
    }
}
