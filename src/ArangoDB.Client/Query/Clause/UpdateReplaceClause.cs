using Remotion.Linq;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Query.Clause
{
    public class UpdateReplaceClause : IBodyClause, IModificationClause
    {
        public string ItemName { get; set; }

        public Type CollectionType { get; set; }

        public bool IgnoreSelect { get; set; }

        public string Command { get; set; }


        public Expression WithSelector { get; set; }

        public Expression KeySelector { get; set; }

        public UpdateReplaceClause(Expression withSelector, string itemName, Type collectionType, Expression keySelector, string command)
        {
            LinqUtility.CheckNotNull("selector", withSelector);

            WithSelector = withSelector;
            KeySelector = keySelector;

            ItemName = itemName;

            CollectionType = collectionType;

            Command = command;
        }

        public virtual void Accept(IQueryModelVisitor visitor, QueryModel queryModel, int index)
        {
            LinqUtility.CheckNotNull("visitor", visitor);
            LinqUtility.CheckNotNull("queryModel", queryModel);

            var arangoVisitor = visitor as ArangoModelVisitor;

            if (arangoVisitor == null)
                throw new Exception("QueryModelVisitor should be type of ArangoModelVisitor");

            arangoVisitor.VisitUpdateReplaceClause(this, queryModel);
        }
        
        public UpdateReplaceClause Clone(CloneContext cloneContext)
        {
            LinqUtility.CheckNotNull("cloneContext", cloneContext);

            var result = new UpdateReplaceClause(WithSelector, ItemName, CollectionType, KeySelector, Command);
            return result;
        }
        
        public virtual void TransformExpressions(Func<Expression, Expression> transformation)
        {
            LinqUtility.CheckNotNull("transformation", transformation);
            WithSelector = transformation(WithSelector);
            KeySelector = transformation(KeySelector);
        }

        IBodyClause IBodyClause.Clone(CloneContext cloneContext)
        {
            return Clone(cloneContext);
        }
    }
}
