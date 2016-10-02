using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Query.Clause
{
    public sealed class GroupByClause : IBodyClause
    {
        private Expression _selector;

        LambdaExpression lambdaSelector;

        public string IntoName;

        public Func<string, string> FuncIntoName;

        string intoIdentifier;

        public string FromParameterName;

        public readonly bool GroupOnLastGroup;

        public bool Visited { get; set; }

        public string CollectVariableName { get; set; }

        public GroupByClause(Expression selector, LambdaExpression lambdaSelector, string intoIdentifier)
        {
            LinqUtility.CheckNotNull("selector", selector);

            var memberExoression = selector as MemberExpression;
            if (memberExoression != null && ((memberExoression.Expression as QuerySourceReferenceExpression).ReferencedQuerySource as MainFromClause).FromExpression.Type.Name == "IGrouping`2")
                GroupOnLastGroup = true;

            this.intoIdentifier = intoIdentifier;

            this.lambdaSelector = lambdaSelector;

            this.Visited = false;

            _selector = selector;
        }

        public string TranslateIntoName()
        {
            if (FuncIntoName != null)
                return FuncIntoName(intoIdentifier);
            else
                return IntoName;
        }

        public Expression Selector
        {
            get { return _selector; }
            set { _selector = LinqUtility.CheckNotNull("value", value); }
        }

        public void Accept(IQueryModelVisitor visitor, QueryModel queryModel, int index)
        {
            LinqUtility.CheckNotNull("visitor", visitor);
            LinqUtility.CheckNotNull("queryModel", queryModel);

            var arangoVisitor = visitor as ArangoModelVisitor;

            if (arangoVisitor == null)
                throw new Exception("QueryModelVisitor should be type of AqlModelVisitor");

            arangoVisitor.VisitGroupByClause(this, queryModel, index);
        }

        public void TransformExpressions(Func<Expression, Expression> transformation)
        {
            LinqUtility.CheckNotNull("transformation", transformation);
            Selector = transformation(Selector);
        }

        public GroupByClause Clone(CloneContext cloneContext)
        {
            LinqUtility.CheckNotNull("cloneContext", cloneContext);

            var clone = new GroupByClause(Selector, lambdaSelector, intoIdentifier);
            return clone;
        }

        IBodyClause IBodyClause.Clone(CloneContext cloneContext)
        {
            return Clone(cloneContext);
        }

        public override string ToString()
        {
            return "collect " + _selector.ToString();
        }
    }
}
