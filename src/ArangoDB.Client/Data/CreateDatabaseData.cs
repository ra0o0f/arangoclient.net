using ArangoDB.Client.Common.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    [CollectionProperty(Naming=NamingConvention.ToCamelCase)]
    public class CreateDatabaseData
    {
        public string Name { get; set; }

        public List<DatabaseUser> Users { get; set; }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class DatabaseUser
    {
        public string Username { get; set; }

        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public string Passwd { get; set; }

        public bool Active { get; set; }

        public object Extra { get; set; }

        public DatabaseUser()
        {
            this.Active = true;
        }
    }
}
