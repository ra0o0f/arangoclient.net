using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public interface ICursorAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        CursorResult CursorResult { get; }
    }
}
