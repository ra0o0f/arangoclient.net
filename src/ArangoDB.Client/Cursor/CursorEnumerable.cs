using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Cursor
{
    public class CursorEnumerable<T> : ICursorEnumerable<T>
    {
        IAsyncEnumerator<T> asyncEnumerator;

        public CursorEnumerable(IAsyncEnumerator<T> asyncEnumerator)
        {
            this.asyncEnumerator = asyncEnumerator;
        }

        public IEnumerator GetEnumerator()
        {
            return new CursorEnumerator<T>(asyncEnumerator);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new CursorEnumerator<T>(asyncEnumerator);
        }
    }
}
