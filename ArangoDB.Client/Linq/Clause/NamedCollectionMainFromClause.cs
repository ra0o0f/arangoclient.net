using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client.Common.Remotion.Linq.Clauses;

namespace ArangoDB.Client.Linq.Clause
{
    public class NamedCollectionMainFromClause : MainFromClause
    {
        public string CollectionName { get; private set; }

        public NamedCollectionMainFromClause(MainFromClause mainFromClause, string collectionName)
            : this(mainFromClause.ItemName, mainFromClause.ItemType, mainFromClause.FromExpression, collectionName)
        {
        }

        public NamedCollectionMainFromClause(string itemName, Type itemType, Expression fromExpression, string collectionName)
            : base(itemName, itemType, fromExpression)
        {
            this.CollectionName = collectionName;
        }
    }
}
