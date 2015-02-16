using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public class ArangoServerException : Exception
    {
        public ArangoServerException(BaseResult baseResult)
            : base(string.Format("{0}. ErrorNumber: {1} HttpStatusCode: {2}",baseResult.ErrorMessage,baseResult.ErrorNum,baseResult.Code))
        {
        }

        public ArangoServerException(string message)
            : base(message)
        {
        }

        public ArangoServerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
