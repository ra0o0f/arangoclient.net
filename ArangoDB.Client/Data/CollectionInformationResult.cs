using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class CollectionInformationResult : BaseResult
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public CollectionStatus Status { get; set; }

        public CollectionType Type { get; set; }

        public bool IsSystem { get; set; }

    }
}
