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
            using(asyncEnumerator)
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

        public CursorResult Statistics
        {
            get { return asyncEnumerator.CursorResult; }
        }
    }
}
