using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class CreateCollectionResult : BaseResult
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool WaitForSync { get; set; }

        public bool IsVolatile { get; set; }

        public bool IsSystem { get; set; }

        public int Status { get; set; }

        public int Type { get; set; }
    }
}
