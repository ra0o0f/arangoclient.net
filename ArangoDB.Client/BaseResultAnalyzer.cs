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
        ArangoDatabase db;

        public BaseResultAnalyzer(ArangoDatabase db)
        {
            this.db = db;
        }

        public void ThrowIfNeeded(BaseResult baseResult)
        {
            if(baseResult.Error)
            {
                throw new ArangoServerException(baseResult);
            }
        }
    }
}
