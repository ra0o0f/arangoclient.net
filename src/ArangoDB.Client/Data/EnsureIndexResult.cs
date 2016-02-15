using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class EnsureIndexResult : BaseResult
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public int? Size { get; set; }

        public int? ByteSize { get; set; }

        public bool? Unique { get; set; }

        public bool? IsNewlyCreated { get; set; }

        public int? MinLength { get; set; }

        public bool? Sparse { get; set; }

        public bool? GeoJson { get; set; }

        public bool? Constraint { get; set; }

        public bool? IgnoreNull { get; set; }

        public List<string> Fields { get; set; }

        public int? SelectivityEstimate { get; set; }
    }
}
