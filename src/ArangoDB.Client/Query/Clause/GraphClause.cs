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
    public class GraphClause : IBodyClause, ITraversalClause
    {
        public ConstantExpression GraphName { get; set; }

        public string Identifier { get; set; }

        public ConstantExpression Min { get; set; }

        public ConstantExpression Max { get; set; }

        public ConstantExpression Direction { get; set; }

        public Expression StartVertex { get; set; }

        public GraphClause(ConstantExpression graphName, string identifier)
        {
            LinqUtility.CheckNotNull("graphName", graphName);
            LinqUtility.CheckNotNull("identifier", identifier);

            GraphName = graphName;
            Identifier = identifier;
        }

        public virtual void Accept(IQueryModelVisitor visitor, QueryModel queryModel, int index)
        {
            LinqUtility.CheckNotNull("visitor", visitor);
            LinqUtility.CheckNotNull("queryModel", queryModel);

            var arangoVisotor = visitor as ArangoModelVisitor;

            if (arangoVisotor == null)
                throw new Exception("QueryModelVisitor should be type of ArangoModelVisitor");

            arangoVisotor.VisitTraversalClause(this, queryModel, index);
        }

        public virtual GraphClause Clone(CloneContext cloneContext)
        {
            LinqUtility.CheckNotNull("cloneContext", cloneContext);

            var clone = new GraphClause(GraphName, Identifier);
            return clone;
        }

        IBodyClause IBodyClause.Clone(CloneContext cloneContext)
        {
            return Clone(cloneContext);
        }

        public void TransformExpressions(Func<Expression, Expression> transformation)
        {
            LinqUtility.CheckNotNull("transformation", transformation);
            GraphName = transformation(GraphName) as ConstantExpression;
        }
    }
}
