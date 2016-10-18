using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class TraversalData<TVertex, TEdge>
    {
        public TVertex Vertex { get; set; }

        public TEdge Edge { get; set; }

        public TraversalPathData<TVertex, TEdge> Path { get; set; }
    }
}
