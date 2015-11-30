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
    public class FilterClause : IBodyClause
    {
        private Expression _predicate;

        public FilterClause(Expression predicate)
        {
            Utils.CheckNotNull("predicate", predicate);
            _predicate = predicate;
        }
        public Expression Predicate
        {
            get { return _predicate; }
            set { _predicate = Utils.CheckNotNull("value", value); }
        }

        public virtual void Accept(IQueryModelVisitor visitor, QueryModel queryModel, int index)
        {
            Utils.CheckNotNull("visitor", visitor);
            Utils.CheckNotNull("queryModel", queryModel);

            var aqlVisotor = visitor as AqlModelVisitor;

            if (aqlVisotor == null)
                throw new Exception("QueryModelVisitor should be type of AqlModelVisitor");

            aqlVisotor.VisitFilterClause(this, queryModel, index);
        }

        public virtual FilterClause Clone(CloneContext cloneContext)
        {
            Utils.CheckNotNull("cloneContext", cloneContext);

            var clone = new FilterClause(Predicate);
            return clone;
        }

        public void TransformExpressions(Func<Expression, Expression> transformation)
        {
            Utils.CheckNotNull("transformation", transformation);
            Predicate = transformation(Predicate);
        }

        

        IBodyClause IBodyClause.Clone(CloneContext cloneContext)
        {
            return Clone(cloneContext);
        }

        public override string ToString()
        {
            return "filter " + FormattingExpressionTreeVisitor.Format(Predicate);
        }
    }
}
