using ArangoDB.Client.Data;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ArangoDB.Client.Query.Clause
{
    public interface ITraversalClause
    {
        string GraphName { get; set; }

        List<TraversalEdgeDefinition> EdgeCollections { get; set; }
        
        string Identifier { get; set; }

        ConstantExpression Options { get; set; }

        ConstantExpression Min { get; set; }

        ConstantExpression Max { get; set; }

        ConstantExpression Direction { get; set; }

        Expression StartVertex { get; set; }

        Expression TargetVertex { get; set; }
    }
}