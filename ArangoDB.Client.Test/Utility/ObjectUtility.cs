using ArangoDB.Client.Common.Newtonsoft.Json.Linq;
using ArangoDB.Client.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Test.Utility
{
    public class ObjectUtility
    {
        public static JObject CreateJObject(object document,IArangoDatabase db)
        {
            return JObject.FromObject(document, new DocumentSerializer(db).CreateJsonSerializer());
        }
    }
}
