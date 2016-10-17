using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public interface IShortestPathQueryable : IQueryable, IOrderedQueryable
    {

    }

    public interface IShortestPathQueryable<T> : IShortestPathQueryable, IQueryable<T>, IOrderedQueryable<T>
    {

    }
}
