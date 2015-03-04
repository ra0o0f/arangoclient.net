using ArangoDB.Client.Common.Newtonsoft.Json;
using ArangoDB.Client.Common.Newtonsoft.Json.Linq;
using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Serialization
{
    public class DocumentParser
    {
        ArangoDatabase db;

        public DocumentParser(ArangoDatabase db)
        {
            this.db = db;
        }

        /// <summary>
        /// Parse Merge and Distinct Results
        /// </summary>
        /// <returns></returns>
        public T ParseSingleResult<T>(JsonTextReader reader,out JObject jObject)
        {
            reader.Read();

            if (reader.TokenType != JsonToken.StartObject)
                throw new InvalidOperationException("Expecting an object in parsing");

            jObject = JObject.Load(reader);

            var serializer = new DocumentSerializer(db).CreateJsonSerializer();
            return jObject.ToObject<T>(serializer);
        }
    }
}
