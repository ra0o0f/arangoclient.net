using System.Linq.Expressions;

namespace ArangoDB.Client.Query.Clause
{
    public interface ITraversalClause
    {
        Expression GraphName { get; set; }
    }
}