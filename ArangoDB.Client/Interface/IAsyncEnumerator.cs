using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public interface IAsyncEnumerator<T> : IDisposable
    {
        Task<bool> MoveNextAsync();
        T Current { get; }
    }
}
