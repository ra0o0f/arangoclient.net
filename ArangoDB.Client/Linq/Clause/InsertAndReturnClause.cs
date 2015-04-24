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
    public class InsertAndReturnClause : IBodyClause, IModifyExpressionNode
    {
        private Expression _withSelector;

        public bool ReturnResult { get; set; }

        public bool ReturnNewResult { get; set; }

        public string ItemName { get; set; }

        public Type CollectionType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectClause"/> class.
        /// </summary>
        /// <param name="selector">The selector that projects the data items.</param>
        public InsertAndReturnClause(Expression withSelector, string itemName, Type collectionType)
        {
            _withSelector = withSelector;

            ItemName = itemName;

            CollectionType = collectionType;
        }

        public Expression WithSelector
        {
            get { return _withSelector; }
            set { _withSelector = Utils.CheckNotNull("value", value); }
        }

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

            aqlVisitor.VisitInsertAndReturnClause(this, queryModel);
        }

        /// <summary>
        /// Clones this clause.
        /// </summary>
        /// <param name="cloneContext">The clones of all query source clauses are registered with this <see cref="CloneContext"/>.</param>
        /// <returns>A clone of this clause.</returns>
        public InsertAndReturnClause Clone(CloneContext cloneContext)
        {
            Utils.CheckNotNull("cloneContext", cloneContext);

            var result = new InsertAndReturnClause(WithSelector, ItemName, CollectionType);
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
            WithSelector = transformation(WithSelector);
        }

        IBodyClause IBodyClause.Clone(CloneContext cloneContext)
        {
            return Clone(cloneContext);
        }

        public override string ToString()
        {
            return "insert " + FormattingExpressionTreeVisitor.Format(WithSelector);
        }
    }
}
