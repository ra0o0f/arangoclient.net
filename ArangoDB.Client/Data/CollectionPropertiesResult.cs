using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class CollectionPropertiesResult : CollectionInformationResult
    {
        public bool WaitForSync { get; set; }

        public bool DoCompact { get; set; }

        public int JournalSize { get; set; }

        public CollectionKeyOptions KeyOptions { get; set; }

        public bool IsVolatile { get; set; }

        public int? NumberOfShards { get; set; }

        public IEnumerable<string> ShardKeys { get; set; }

    }
}
