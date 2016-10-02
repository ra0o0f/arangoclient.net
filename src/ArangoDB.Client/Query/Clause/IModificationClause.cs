using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Query.Clause
{
    public interface IModificationClause
    {
        string ItemName { get; set; }

        Type CollectionType { get; set; }

        bool IgnoreSelect { get; set; }
    }
}
