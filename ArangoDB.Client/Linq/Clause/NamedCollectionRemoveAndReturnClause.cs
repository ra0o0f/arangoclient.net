using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client.Common.Remotion.Linq.Clauses;

namespace ArangoDB.Client.Linq.Clause
{
    public class NamedCollectionRemoveAndReturnClause : RemoveAndReturnClause
    {
        public string CollectionName { get; private set; }

        public NamedCollectionRemoveAndReturnClause(RemoveAndReturnClause removeAndReturnClause, string collectionName)
            : this(removeAndReturnClause.ItemName, removeAndReturnClause.CollectionType, removeAndReturnClause.KeySelector, collectionName)
        {
            ReturnResult = removeAndReturnClause.ReturnResult;
            ReturnNewResult = removeAndReturnClause.ReturnNewResult;
        }

        public NamedCollectionRemoveAndReturnClause(string itemName, Type collectionType, Expression keySelector, string collectionName)
            : base(itemName, collectionType, keySelector)
        {
            this.CollectionName = collectionName;
        }
    }
}
