using ArangoDB.Client.Common.Remotion.Linq;
using ArangoDB.Client.Common.Remotion.Linq.Clauses;
using ArangoDB.Client.Common.Remotion.Linq.Clauses.Expressions;
using ArangoDB.Client.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Linq.Clause
{
    public class GroupByClause : IBodyClause
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

        public GroupByClause(Expression selector,LambdaExpression lambdaSelector,string intoIdentifier)
        {
            Utils.CheckNotNull("selector", selector);

            var memberExoression = selector as MemberExpression;
            if (memberExoression!=null && ((memberExoression.Expression as QuerySourceReferenceExpression).ReferencedQuerySource as MainFromClause).FromExpression.Type.Name == "IGrouping`2")
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
            set { _selector = Utils.CheckNotNull("value", value); }
        }

        public virtual void Accept(IQueryModelVisitor visitor, QueryModel queryModel,int index)
        {
            Utils.CheckNotNull("visitor", visitor);
            Utils.CheckNotNull("queryModel", queryModel);

            var aqlVisotor = visitor as AqlModelVisitor;

            if (aqlVisotor == null)
                throw new Exception("QueryModelVisitor should be type of AqlModelVisitor");

            aqlVisotor.VisitGroupByClause(this, queryModel, index);
        }

        public void TransformExpressions(Func<Expression, Expression> transformation)
        {
            Utils.CheckNotNull("transformation", transformation);
            Selector = transformation(Selector);
        }

        public virtual GroupByClause Clone(CloneContext cloneContext)
        {
            Utils.CheckNotNull("cloneContext", cloneContext);

            var clone = new GroupByClause(Selector,lambdaSelector,intoIdentifier);
            return clone;
        }

        IBodyClause IBodyClause.Clone(CloneContext cloneContext)
        {
            return Clone(cloneContext);
        }

        //public override string ToString()
        //{
        //    return base.ToString();//;"filter " + FormattingExpressionTreeVisitor.Format(Predicate);
        //}
    }
}
