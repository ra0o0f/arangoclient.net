using Newtonsoft.Json.Linq;
using ArangoDB.Client.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArangoDB.Client.Test.Utility
{
    public class ObjectUtility
    {
        public static JObject CreateJObject(object document,IArangoDatabase db)
        {
            return JObject.FromObject(document, new DocumentSerializer(db).CreateJsonSerializer());
        }

        public static string Serialize(object document, IArangoDatabase db)
        {
            return new DocumentSerializer(db).SerializeWithoutReader(document);
        }

        public static void AssertSerialize(object document1,object document2, IArangoDatabase db)
        {
            Assert.Equal(Serialize(document1,db), Serialize(document2,db));
        }
    }
}
