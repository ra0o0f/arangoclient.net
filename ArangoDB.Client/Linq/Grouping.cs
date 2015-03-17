using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Linq
{
    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        public IEnumerable<TElement> Elements;

        private TKey key;

        public TKey Key { get { return key; } }

        public Grouping(TKey Key)
        {
            this.key = Key;
            Elements = new List<TElement>();
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return Elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
