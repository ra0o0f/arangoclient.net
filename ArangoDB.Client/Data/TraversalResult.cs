using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class TraversalContainerResult<TVertex, TEdge> : BaseResult
    {
        public TraversalResult<TVertex, TEdge> Result { get; set; }
    }

    public class TraversalResult<TVertex,TEdge> : BaseResult
    {
        public TraversalVisitedResult<TVertex, TEdge> Visited { get; set; }
    }

    public class TraversalVisitedResult<TVertex, TEdge>
    {
        public List<TVertex> Vertices { get; set; }

        public List<TraversalVisitedPathResult<TVertex, TEdge>> Paths { get; set; }
    }

    public class TraversalVisitedPathResult<TVertex, TEdge>
    {
        public List<TVertex> Vertices { get; set; }

        public List<TEdge> Edges { get; set; }
    }
}
