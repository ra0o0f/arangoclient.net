using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public interface ICursor<T>
    {
        ICursorAsyncEnumerator<T> GetAsyncEnumerator();

        ICursorEnumerable<T> AsEnumerable();

        List<T> ToList();

        Task<List<T>> ToListAsync();

        void ForEach(Action<T> action);

        Task ForEachAsync(Action<T> action);

        CursorResult Statistics { get; }
    }
}
