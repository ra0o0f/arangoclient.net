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
    public class InsertClause : IBodyClause, IModificationClause
    {
        public string ItemName { get; set; }

        public Type CollectionType { get; set; }

        public bool IgnoreSelect { get; set; }

        public Expression WithSelector { get; set; }

        public InsertClause(Expression withSelector, string itemName, Type collectionType)
        {
            WithSelector = withSelector;

            ItemName = itemName;

            CollectionType = collectionType;
        }
        
        public virtual void Accept(IQueryModelVisitor visitor, QueryModel queryModel, int index)
        {
            LinqUtility.CheckNotNull("visitor", visitor);
            LinqUtility.CheckNotNull("queryModel", queryModel);

            var arangoVisitor = visitor as ArangoModelVisitor;

            if (arangoVisitor == null)
                throw new Exception("QueryModelVisitor should be type of ArangoModelVisitor");

            arangoVisitor.VisitInsertClause(this, queryModel);
        }
        
        public InsertClause Clone(CloneContext cloneContext)
        {
            LinqUtility.CheckNotNull("cloneContext", cloneContext);

            var result = new InsertClause(WithSelector, ItemName, CollectionType);
            return result;
        }
        
        public virtual void TransformExpressions(Func<Expression, Expression> transformation)
        {
            LinqUtility.CheckNotNull("transformation", transformation);
            WithSelector = transformation(WithSelector);
        }

        IBodyClause IBodyClause.Clone(CloneContext cloneContext)
        {
            return Clone(cloneContext);
        }
    }
}
