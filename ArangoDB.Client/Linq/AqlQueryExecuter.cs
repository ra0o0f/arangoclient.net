using ArangoDB.Client.Common.Remotion.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Linq
{
    public class AqlQueryExecuter : IQueryExecutor
    {
        IArangoDatabase db;

        public AqlQueryExecuter(IArangoDatabase db)
        {
            this.db = db;
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            return ExecuteCollection<T>(queryModel).Single();
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            return returnDefaultWhenEmpty ? ExecuteCollection<T>(queryModel).SingleOrDefault() : ExecuteCollection<T>(queryModel).Single();
        }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            return GetCursor<T>(queryModel).AsEnumerable();
        }

        ICursor<T> GetCursor<T>(QueryModel queryModel)
        {
            var visitor = new AqlModelVisitor(db);
            visitor.VisitQueryModel(queryModel);

            visitor.QueryData.Query = visitor.QueryText.ToString();

            return db.CreateStatement<T>(visitor.QueryData.Query,
                bindVars: visitor.QueryData.BindVars);
        }
    }
}
