using ArangoDB.Client.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public interface IAqlModifiable : IQueryable, IOrderedQueryable
    {

    }

    public interface IAqlModifiable<T> : IAqlModifiable, IQueryable<T>, IOrderedQueryable<T>
    {

    }

    public interface IQueryableState
    {
        Dictionary<string, string> StateValues { get; set; }
    }
}
