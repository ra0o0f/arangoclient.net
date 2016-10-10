using System.Linq.Expressions;

namespace ArangoDB.Client.Query.Clause
{
    public interface ITraversalClause
    {
        ConstantExpression GraphName { get; set; }

        string AssociatedIdentifier { get; set; }

        ConstantExpression Min { get; set; }

        ConstantExpression Max { get; set; }

        ConstantExpression Direction { get; set; }

        Expression StartVertex { get; set; }
    }
}