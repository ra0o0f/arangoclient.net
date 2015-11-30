using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Linq
{
    public class VisitorCrudState
    {
        public bool ModelVisitorHaveCrudOperation { get; set; }

        public bool ReturnResult { get; set; }

        public string ReturnResultKind { get; set; }

        public string Collection { get; set; }
    }
}
