using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client.Common.Remotion.Linq.Clauses;

namespace ArangoDB.Client.Linq.Clause
{
    public class NamedCollectionAdditionalFromClause : AdditionalFromClause
    {
        public string CollectionName { get; private set; }

        public NamedCollectionAdditionalFromClause(AdditionalFromClause additionalFromClause, string collectionName)
            : this(additionalFromClause.ItemName, additionalFromClause.ItemType, additionalFromClause.FromExpression, collectionName)
        {
        }

        public NamedCollectionAdditionalFromClause(string itemName, Type itemType, Expression fromExpression, string collectionName)
            : base(itemName, itemType, fromExpression)
        {
            this.CollectionName = collectionName;
        }
    }
}
