using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Examples.Models
{
    public class Follow
    {
        public string Key { get; set; }

        [DocumentProperty(Identifier = IdentifierType.EdgeFrom)]
        public string Follower { get; set; }

        [DocumentProperty(Identifier = IdentifierType.EdgeTo)]
        public string Followee { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Label { get; set; }
    }
}
