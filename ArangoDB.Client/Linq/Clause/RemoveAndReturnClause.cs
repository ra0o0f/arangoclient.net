using ArangoDB.Client.Common.Remotion.Linq;
using ArangoDB.Client.Common.Remotion.Linq.Clauses;
using ArangoDB.Client.Common.Remotion.Linq.Clauses.ExpressionTreeVisitors;
using ArangoDB.Client.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Linq.Clause
{
    public class RemoveAndReturnClause : IBodyClause, IModifyExpressionNode
    {
        public bool ReturnResult { get; set; }

        public bool ReturnNewResult { get; set; }

        public string ItemName { get; set; }

        public Type CollectionType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectClause"/> class.
        /// </summary>
        /// <param name="selector">The selector that projects the data items.</param>
        public RemoveAndReturnClause(string itemName, Type collectionType, Expression keySelector)
        {
            KeySelector = keySelector;

            ItemName = itemName;

            CollectionType = collectionType;
        }

        public Expression KeySelector { get; set; }

        /// <summary>
        /// Accepts the specified visitor by calling its <see cref="IQueryModelVisitor.VisitSelectClause"/> method.
        /// </summary>
        /// <param name="visitor">The visitor to accept.</param>
        /// <param name="queryModel">The query model in whose context this clause is visited.</param>
        public virtual void Accept(IQueryModelVisitor visitor, QueryModel queryModel, int index)
        {
            Utils.CheckNotNull("visitor", visitor);
            Utils.CheckNotNull("queryModel", queryModel);

            var aqlVisitor = visitor as AqlModelVisitor;

            if (aqlVisitor == null)
                throw new Exception("QueryModelVisitor should be type of AqlModelVisitor");

            aqlVisitor.VisitRemoveAndReturnClause(this, queryModel);
        }

        /// <summary>
        /// Clones this clause.
        /// </summary>
        /// <param name="cloneContext">The clones of all query source clauses are registered with this <see cref="CloneContext"/>.</param>
        /// <returns>A clone of this clause.</returns>
        public RemoveAndReturnClause Clone(CloneContext cloneContext)
        {
            Utils.CheckNotNull("cloneContext", cloneContext);

            var result = new RemoveAndReturnClause(ItemName, CollectionType, KeySelector);
            return result;
        }

        /// <summary>
        /// Transforms all the expressions in this clause and its child objects via the given <paramref name="transformation"/> delegate.
        /// </summary>
        /// <param name="transformation">The transformation object. This delegate is called for each <see cref="Expression"/> within this
        /// clause, and those expressions will be replaced with what the delegate returns.</param>
        public virtual void TransformExpressions(Func<Expression, Expression> transformation)
        {
            Utils.CheckNotNull("transformation", transformation);
            KeySelector = transformation(KeySelector);
        }

        IBodyClause IBodyClause.Clone(CloneContext cloneContext)
        {
            return Clone(cloneContext);
        }
    }
}
