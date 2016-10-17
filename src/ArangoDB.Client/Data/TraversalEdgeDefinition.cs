using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class TraversalEdgeDefinition
    {
        public string CollectionName { get; set; }

        public EdgeDirection? Direction { get; set; }
    }
}
