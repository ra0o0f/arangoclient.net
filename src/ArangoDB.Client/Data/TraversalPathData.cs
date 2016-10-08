using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class TraversalPathData<TVertex, TEdge>
    {
        public TVertex Vertices { get; set; }

        public TEdge Edges { get; set; }
    }
}
