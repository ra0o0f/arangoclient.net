using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class ParseIdentifierResult
    {
        public string Collection { get; set; }

        public string Key { get; set; }
    }
}
