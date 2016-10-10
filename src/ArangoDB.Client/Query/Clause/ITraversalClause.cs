using System.Linq.Expressions;

namespace ArangoDB.Client.Query.Clause
{
    public interface ITraversalClause
    {
        ConstantExpression GraphName { get; set; }

        string AssociatedIdentifier { get; set; }
    }
}