using ArangoDB.Client.Common.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class BaseResult
    {
        public int Code { get; set; }

        public bool Error { get; set; }

        public string ErrorMessage { get; set; }

        public int ErrorNum { get; set; }   
     
        internal virtual void SetFromJsonTextReader(string name, JsonToken token, object value)
        {
            if (name == "code" && token == JsonToken.Integer)
                this.Code = Convert.ToInt32(value);

            if (name == "error" && token == JsonToken.Boolean)
                this.Error = Convert.ToBoolean(value);

            if (name == "errorMessage" && token == JsonToken.String)
                this.ErrorMessage = value.ToString();

            if (name == "errorNum" && token == JsonToken.Integer)
                this.ErrorNum = Convert.ToInt32(value);
        }
    }
}
