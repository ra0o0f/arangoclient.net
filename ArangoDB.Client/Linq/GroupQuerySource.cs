using ArangoDB.Client.Common.Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Linq
{
    public class GroupQuerySource : IQuerySource
    {
        public string ItemName { get; set; }

        public Type ItemType { get; set; }
    }
}
