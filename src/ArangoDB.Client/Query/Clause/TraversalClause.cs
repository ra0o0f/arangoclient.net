using Remotion.Linq;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client.Data;

namespace ArangoDB.Client.Query.Clause
{
    public class TraversalClause : IBodyClause, ITraversalClause
    {
        public string Identifier { get; set; }

        public ConstantExpression Min { get; set; }

        public ConstantExpression Max { get; set; }

        public ConstantExpression Direction { get; set; }

        public Expression StartVertex { get; set; }

        public Expression TargetVertex { get; set; }

        public string GraphName { get; set; }

        public List<TraversalEdgeDefinition> EdgeCollections { get; set; }

        public ConstantExpression Options { get; set; }

        public TraversalClause(Expression startVertex, string identifier)
        {
            LinqUtility.CheckNotNull("startVertex", startVertex);
            LinqUtility.CheckNotNull("identifier", identifier);

            EdgeCollections = new List<TraversalEdgeDefinition>();

            StartVertex = startVertex;
            Identifier = identifier;
        }

        public TraversalClause(Expression startVertex, Expression targetVertex, string identifier)
            : this(startVertex, identifier)
        {
            LinqUtility.CheckNotNull("tagetVertex", targetVertex);

            TargetVertex = targetVertex;
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

        public virtual TraversalClause Clone(CloneContext cloneContext)
        {
            LinqUtility.CheckNotNull("cloneContext", cloneContext);

            var clone = new TraversalClause(StartVertex, Identifier);
            return clone;
        }

        IBodyClause IBodyClause.Clone(CloneContext cloneContext)
        {
            return Clone(cloneContext);
        }

        public void TransformExpressions(Func<Expression, Expression> transformation)
        {
            LinqUtility.CheckNotNull("transformation", transformation);
            StartVertex = transformation(StartVertex);
            if (TargetVertex != null)
                TargetVertex = transformation(TargetVertex);
        }
    }
}
