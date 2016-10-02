using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Query
{
    public class VisitorModificationData
    {
        public bool HasModificationClause { get; set; }

        public string Collection { get; set; }
    }
}
