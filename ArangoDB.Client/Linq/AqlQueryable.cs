using ArangoDB.Client.Common.Remotion.Linq;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.Structure;
using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Linq
{
    public class AqlQueryable<T> : QueryableBase<T>, ICursor<T>, IAqlModifiable<T>, IAqlModifiable, IQueryableState
    {
        IArangoDatabase db;

        public Dictionary<string, string> StateValues { get; set; }

        public AqlQueryable<T> KeepState(IQueryableState state)
        {
            this.StateValues = state.StateValues;

            return this;
        }

        public AqlQueryable(IQueryParser queryParser, IQueryExecutor executor,IArangoDatabase db)
            : base(new AqlQueryProvider(typeof(AqlQueryable<>), queryParser, executor,db))
        {
            this.StateValues = new Dictionary<string, string>();
            this.db = db;
        }

        public AqlQueryable(IQueryProvider provider, Expression expression, IArangoDatabase db)
            : base(provider, expression)
        {
            this.StateValues = new Dictionary<string, string>();
            this.db = db;
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

        public QueryData GetQueryData()
        {
            var aqlQueryProvider = Provider as AqlQueryProvider;

            if (aqlQueryProvider == null)
                throw new NotSupportedException("AqlQueryable should be use with AqlQueryProvider");

            return aqlQueryProvider.GetQueryData(this.Expression);
        }
    }
}
