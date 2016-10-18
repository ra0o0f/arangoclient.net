using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class TraversalPathData<TVertex, TEdge>
    {
        public IList<TVertex> Vertices { get; set; }

        public IList<TEdge> Edges { get; set; }
    }
}
