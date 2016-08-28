using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class DatabaseInformation
    {
        /// <summary>
        /// The name of the database
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Whether or not the database is the _system database
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// The id of the database
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///  The filesystem path of the database
        /// </summary>
        public string Path { get; set; }
    }
}
