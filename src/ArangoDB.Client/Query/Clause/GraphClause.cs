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
        public Expression GraphName { get; set; }

        public GraphClause(Expression graphName)
        {
            LinqUtility.CheckNotNull("predicate", graphName);
            GraphName = graphName;
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

            var clone = new GraphClause(GraphName);
            return clone;
        }

        IBodyClause IBodyClause.Clone(CloneContext cloneContext)
        {
            return Clone(cloneContext);
        }

        public void TransformExpressions(Func<Expression, Expression> transformation)
        {
            LinqUtility.CheckNotNull("transformation", transformation);
            GraphName = transformation(GraphName);
        }
    }
}
