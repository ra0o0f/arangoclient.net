using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client.Common.Remotion.Linq.Clauses;

namespace ArangoDB.Client.Linq.Clause
{
    public class NamedCollectionUpdateAndReturnClause : UpdateAndReturnClause
    {
        public string CollectionName { get; private set; }

        public NamedCollectionUpdateAndReturnClause(UpdateAndReturnClause updateAndReturnClause, string collectionName)
            : this(updateAndReturnClause.WithSelector, updateAndReturnClause.ItemName, updateAndReturnClause.CollectionType, updateAndReturnClause.KeySelector, updateAndReturnClause.Command, collectionName)
        {
            ReturnResult = updateAndReturnClause.ReturnResult;
            ReturnNewResult = updateAndReturnClause.ReturnNewResult;
        }

        public NamedCollectionUpdateAndReturnClause(Expression withSelector, string itemName, Type collectionType, Expression keySelector, string command, string collectionName)
            : base(withSelector, itemName, collectionType, keySelector, command)
        {
            this.CollectionName = collectionName;
        }
    }
}
