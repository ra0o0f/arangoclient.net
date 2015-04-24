using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Linq.Clause
{
    public interface IModifyExpressionNode
    {
        bool ReturnResult { get; set; }

        bool ReturnNewResult { get; set; }

        string ItemName { get; set; }

        Type CollectionType{get;set;}
    }
}
