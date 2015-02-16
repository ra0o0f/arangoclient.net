using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class DatabaseInformation
    {
        public string Id { get; set; }

        public bool IsSystem { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }
    }
}
