using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class BulkImportResult : BaseResult
    {
        public int Created { get; set; }

        public int Errors { get; set; }

        public int Empty { get; set; }

        public int Updated { get; set; }

        public int Ignored { get; set; }

        public List<string> Details { get; set; }
    }
}
