using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class DocumentIdentifierBaseResult : BaseResult, IDocumentIdentifierResult
    {
        [DocumentProperty(Identifier=IdentifierType.Handle)]
        public string Id { get; set; }

        [DocumentProperty(Identifier = IdentifierType.Revision)]
        public string Rev { get; set; }

        [DocumentProperty(Identifier = IdentifierType.Key)]
        public string Key { get; set; }
    }

    public class DocumentIdentifierWithoutBaseResult : IDocumentIdentifierResult
    {
        [DocumentProperty(Identifier = IdentifierType.Handle)]
        public string Id { get; set; }

        [DocumentProperty(Identifier = IdentifierType.Revision)]
        public string Rev { get; set; }

        [DocumentProperty(Identifier = IdentifierType.Key)]
        public string Key { get; set; }
    }
}
