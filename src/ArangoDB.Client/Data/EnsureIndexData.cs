using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class EnsureIndexData
    {
        /// <summary>
        /// Type of the index
        /// </summary>
        public IndexType Type { get; set; }

        /// <summary>
        /// The maximal size of the active document data in the collection (in bytes)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ByteSize { get; set; }

        /// <summary>
        /// The maximal number of documents for the collection
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Size { get; set; }

        /// <summary>
        /// An array of attribute paths
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IList<string> Fields { get; set; }

        /// <summary>
        /// If true, then creates a unique index
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Unique { get; set; }

        /// <summary>
        /// If true, then create a sparse index
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Sparse { get; set; }

        /// <summary>
        /// If a geo-spatial index on a location is constructed and geoJson is true, then the order within the array is longitude followed by latitude
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? GeoJson { get; set; }

        /// <summary>
        /// Minimum character length of words to index
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MinLength { get; set; }
    }
}
