using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public class BaseResultAnalyzer
    {
        IArangoDatabase db;

        public BaseResultAnalyzer(IArangoDatabase db)
        {
            this.db = db;
        }

        public void ThrowIfNeeded(BaseResult baseResult)
        {
            if (baseResult.HasError() && db.Setting.ThrowForServerErrors == true)
            {
                throw new ArangoServerException(baseResult);
            }
        }

        public void Throw(BaseResult baseResult)
        {
            if (baseResult.HasError())
                throw new ArangoServerException(baseResult);
        }
    }
}
