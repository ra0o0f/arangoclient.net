using ArangoDB.Client.Data;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Query
{
    public class ArangoQueryable<T> : QueryableBase<T>, IAqlModifiable<T>, IAqlModifiable, 
        ITraversalQueryable<T>, ITraversalQueryable, 
        IShortestPathQueryable<T>, IShortestPathQueryable
    {
        IArangoDatabase db;

        public ArangoQueryable(IQueryParser queryParser, IQueryExecutor executor, IArangoDatabase db)
        : base(new ArangoQueryProvider(typeof(ArangoQueryable<>), queryParser, executor, db))
        {
            this.db = db;
        }

        public ArangoQueryable(IQueryProvider provider, Expression expression, IArangoDatabase db)
        : base(provider, expression)
        {
            this.db = db;
        }

        public QueryData GetQueryData()
        {
            var arangoQueryProvider = Provider as ArangoQueryProvider;

            if (arangoQueryProvider == null)
                throw new NotSupportedException("ArangoQueryable should be use with ArangoQueryProvider");

            return arangoQueryProvider.GetQueryData(Expression);
        }

        public ICursorAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return AsCursor().GetAsyncEnumerator();
        }

        public ICursorEnumerable<T> AsEnumerable()
        {
            return AsCursor().AsEnumerable();
        }

        public List<T> ToList()
        {
            return AsCursor().ToList();
        }

        public async Task<List<T>> ToListAsync()
        {
            return await AsCursor().ToListAsync().ConfigureAwait(false);
        }

        public void ForEach(Action<T> action)
        {
            AsCursor().ForEach(x => action(x));
        }

        public async Task ForEachAsync(Action<T> action)
        {
            await AsCursor().ForEachAsync(x => action(x)).ConfigureAwait(false);
        }

        public CursorResult Statistics
        {
            get
            {
                return AsCursor().Statistics;
            }
        }

        public ICursor<T> AsCursor()
        {
            var queryData = GetQueryData();
            return db.CreateStatement<T>(queryData.Query
                , bindVars: queryData.BindVars);
        }
    }
}
