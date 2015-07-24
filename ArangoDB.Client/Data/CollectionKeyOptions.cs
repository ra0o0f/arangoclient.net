using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class CollectionKeyOptions
    {
        public KeyGenerationType Type { get; set; }

        public bool AllowUserKeys { get; set; }
    }
}
