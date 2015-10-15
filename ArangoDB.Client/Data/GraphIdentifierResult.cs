using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class GraphIdentifierResult
    {
        [DocumentProperty(Identifier = IdentifierType.Handle)]
        public string Id { get; set; }

        [DocumentProperty(Identifier = IdentifierType.Revision)]
        public string Rev { get; set; }

        public List<EdgeDefinitionData> EdgeDefinitions { get; set; }

        public List<string> OrphanCollections { get; set; }
    }
}
