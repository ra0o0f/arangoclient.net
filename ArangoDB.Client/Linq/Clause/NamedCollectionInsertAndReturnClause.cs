using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client.Common.Remotion.Linq.Clauses;

namespace ArangoDB.Client.Linq.Clause
{
    public class NamedCollectionInsertAndReturnClause : InsertAndReturnClause
    {
        public string CollectionName { get; private set; }

        public NamedCollectionInsertAndReturnClause(InsertAndReturnClause insertAndReturnClause, string collectionName)
            : this(insertAndReturnClause.WithSelector, insertAndReturnClause.ItemName, insertAndReturnClause.CollectionType, collectionName)
        {
            ReturnResult = insertAndReturnClause.ReturnResult;
            ReturnNewResult = insertAndReturnClause.ReturnNewResult;
        }

        public NamedCollectionInsertAndReturnClause(Expression withSelector, string itemName, Type collectionType, string collectionName)
            : base(withSelector, itemName, collectionType)
        {
            this.CollectionName = collectionName;
        }
    }
}
