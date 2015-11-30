using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Test.Model
{
    public class Friend
    {
        [DocumentProperty(Identifier = IdentifierType.Key)]
        public string Key { get; set; }

        [DocumentProperty(Identifier = IdentifierType.Handle)]
        public string Id { get; set; }

        [DocumentProperty(Identifier = IdentifierType.Revision)]
        public string Rev { get; set; }

        [DocumentProperty(Identifier=IdentifierType.EdgeFrom)]
        public string Follower { get; set; }

        [DocumentProperty(Identifier = IdentifierType.EdgeTo)]
        public string Followee { get; set; }

        public DateTime Creation { get; set; }

        public string Message { get; set; }
    }
}
