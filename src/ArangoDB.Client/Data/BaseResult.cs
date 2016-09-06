using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class BaseResult
    {
        public int? Code { get; set; }

        public bool? Error { get; set; }

        public string ErrorMessage { get; set; }

        public int? ErrorNum { get; set; } 

        public bool HasError()
        {
            return Error == true;
        }

        protected void SetProperties(BaseResult baseResult)
        {
            Code = baseResult.Code;
            Error = baseResult.Error;
            ErrorMessage = baseResult.ErrorMessage;
            ErrorNum = baseResult.ErrorNum;
        }
     
        internal virtual void SetFromJsonTextReader(string name, JsonToken token, object value)
        {
            if (name == "code" && token == JsonToken.Integer)
                Code = Convert.ToInt32(value);

            if (name == "error" && token == JsonToken.Boolean)
                Error = Convert.ToBoolean(value);

            if (name == "errorMessage" && token == JsonToken.String)
                ErrorMessage = value.ToString();

            if (name == "errorNum" && token == JsonToken.Integer)
                ErrorNum = Convert.ToInt32(value);
        }
    }
}
