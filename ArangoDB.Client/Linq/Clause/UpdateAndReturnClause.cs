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
    public class UpdateAndReturnClause : IBodyClause
    {
        private Expression _withSelector;

        private Expression _returnNewResult;

        private Expression _returnModifiedResult;

        private Expression _inCollection;

        public string ItemName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectClause"/> class.
        /// </summary>
        /// <param name="selector">The selector that projects the data items.</param>
        public UpdateAndReturnClause(Expression withSelector,string itemName, Expression keySelector
            , Expression returnModifiedResult, Expression returnNewResult,Expression inCollection)
        {
            Utils.CheckNotNull("selector", withSelector);
            Utils.CheckNotNull("returnNewResult", returnNewResult);

            _withSelector = withSelector;
            _returnNewResult = returnNewResult;
            _returnModifiedResult = returnModifiedResult;
            _inCollection = inCollection;

            KeySelector = keySelector;

            ItemName = itemName;
        }

        public Expression ReturnNewResult
        {
            get { return _returnNewResult; }
            set
            {
                Utils.CheckNotNull("value", value);

                var lambda = value as LambdaExpression;
                if (lambda == null && value.Type != typeof(bool))
                {
                    var message = string.Format("The value expression returns '{0}', an expression returning 'System.Boolean' was expected.", value.Type);
                    throw new ArgumentException(message, "value");
                }

                _returnNewResult = value;
            }
        }

        public Expression ReturnModifiedResult
        {
            get { return _returnModifiedResult; }
            set
            {
                Utils.CheckNotNull("value", value);

                var lambda = value as LambdaExpression;
                if (lambda == null && value.Type != typeof(bool))
                {
                    var message = string.Format("The value expression returns '{0}', an expression returning 'System.Boolean' was expected.", value.Type);
                    throw new ArgumentException(message, "value");
                }

                _returnModifiedResult = value;
            }
        }

        public Expression InCollection
        {
            get { return _inCollection; }
            set
            {
                Utils.CheckNotNull("value", value);

                var lambda = value as LambdaExpression;
                if (lambda == null && value.Type != typeof(Type))
                {
                    var message = string.Format("The value expression returns '{0}', an expression returning 'System.Type' was expected.", value.Type);
                    throw new ArgumentException(message, "value");
                }

                _inCollection = value;
            }
        }

        public Expression WithSelector
        {
            get { return _withSelector; }
            set { _withSelector = Utils.CheckNotNull("value", value); }
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

            aqlVisitor.VisitUpdateAndReturnClause(this, queryModel);
        }

        /// <summary>
        /// Clones this clause.
        /// </summary>
        /// <param name="cloneContext">The clones of all query source clauses are registered with this <see cref="CloneContext"/>.</param>
        /// <returns>A clone of this clause.</returns>
        public UpdateAndReturnClause Clone(CloneContext cloneContext)
        {
            Utils.CheckNotNull("cloneContext", cloneContext);

            var result = new UpdateAndReturnClause(WithSelector,ItemName,KeySelector,ReturnModifiedResult,ReturnNewResult,InCollection);
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
            return "update " + FormattingExpressionTreeVisitor.Format(WithSelector);
        }
    }
}
