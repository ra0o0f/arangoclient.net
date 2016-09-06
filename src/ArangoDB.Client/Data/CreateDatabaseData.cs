using Newtonsoft.Json;
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
        /// <summary>
        /// The user name as a string. This attribute is mandatory
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The user password as a string. If not specified, then it defaults to an empty string
        /// </summary>
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public string Passwd { get; set; }

        /// <summary>
        /// A boolean flag indicating whether the user account should be active or not. The default value is true
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Active { get; set; }

        /// <summary>
        /// An optional JSON object with extra user information. The data contained in extra will be stored for the user but not be interpreted further by ArangoDB
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Extra { get; set; }
    }
}
