using ArangoDB.Client.Cursor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class Cursor<T> : ICursor<T>
    {
        ICursorAsyncEnumerator<T> asyncEnumerator;

        public Cursor(ICursorAsyncEnumerator<T> asyncEnumerator)
        {
            this.asyncEnumerator = asyncEnumerator;
        }

        public ICursorAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return asyncEnumerator;
        }

        public ICursorEnumerable<T> AsEnumerable()
        {
            return new CursorEnumerable<T>(asyncEnumerator);
        }

        public List<T> ToList()
        {
            return AsEnumerable().ToList();
        }

        public void ForEach(Action<T> action)
        {
            foreach (var item in AsEnumerable())
                action(item);
        }

        public async Task<List<T>> ToListAsync()
        {
            List<T> list = new List<T>();
            using (asyncEnumerator)
            {
                while (await asyncEnumerator.MoveNextAsync().ConfigureAwait(false))
                {
                    list.Add(asyncEnumerator.Current);
                }
            }
            return list;
        }

        public async Task ForEachAsync(Action<T> action)
        {
            using (asyncEnumerator)
            {
                while (await asyncEnumerator.MoveNextAsync().ConfigureAwait(false))
                {
                    action(asyncEnumerator.Current);
                }
            }
        }

        internal async Task<T> ExecuteScalar(bool returnDefaultWhenEmpty = false, bool throwIfNotSingle = false)
        {
            using (asyncEnumerator)
            {
                var hasNext = await asyncEnumerator.MoveNextAsync().ConfigureAwait(false);
                if (hasNext)
                {
                    if (throwIfNotSingle && (await asyncEnumerator.MoveNextAsync().ConfigureAwait(false)))
                    {
                        throw new InvalidOperationException("Sequence contains more than one element");
                    }

                    return asyncEnumerator.Current;
                }
                else if (returnDefaultWhenEmpty)
                    return default(T);
                else
                    throw new InvalidOperationException("Sequence contains no elements");
            }
        }

        public CursorResult Statistics
        {
            get { return asyncEnumerator.CursorResult; }
        }
    }
}
