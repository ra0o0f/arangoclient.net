using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Linq
{
    public class GroupingGenerator
    {
        public static IGrouping<TKey,TElement> Get<TKey,TElement>(TKey key,TElement element)
        {
            return new Grouping<TKey, TElement>(key);
        }
    }

    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        public IEnumerable<TElement> Elements;

        private TKey key;

        public TKey Key { get { return key; } }

        //public Grouping()
        //{
        //    key = default(TKey);
        //    Elements = new List<TElement>();
        //}

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
