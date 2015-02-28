using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client.Utility;

namespace ArangoDB.Client.Cursor
{
    public class CursorEnumerator<T> : ICursorEnumerator<T>
    {
        IAsyncEnumerator<T> asyncEnumerator;

        public CursorEnumerator(IAsyncEnumerator<T> asyncEnumerator)
        {
            this.asyncEnumerator = asyncEnumerator;
        }

        public bool MoveNext()
        {
            return asyncEnumerator.MoveNextAsync().ResultSynchronizer();
        }

        T IEnumerator<T>.Current
        {
            get
            {
                return asyncEnumerator.Current;
            }
        }

        public object Current
        {
            get
            {
                return asyncEnumerator.Current;
            }
        }

        public void Reset()
        {
            throw new NotSupportedException("Cursors in arangodb can not be reset");
        }

        public void Dispose()
        {
            asyncEnumerator.Dispose();
        }

    }
}
