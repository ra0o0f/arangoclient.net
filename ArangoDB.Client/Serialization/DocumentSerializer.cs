using ArangoDB.Client.Common.Newtonsoft.Json;
using ArangoDB.Client.Common.Newtonsoft.Json.Linq;
using ArangoDB.Client.Data;
using ArangoDB.Client.Serialization.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Serialization
{
    public class DocumentSerializer
    {
        IArangoDatabase db;
        public DocumentSerializer(IArangoDatabase db)
        {
            this.db = db;
        }

        public T Deserialize<T>(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var serializer = CreateJsonSerializer();

                return serializer.Deserialize<T>(jsonReader);
            }
        }

        public T Deserialize<T>(JsonTextReader reader)
        {
            var serializer = CreateJsonSerializer();

            return serializer.Deserialize<T>(reader);
        }

        public List<T> DeserializeBatchResult<T>(Stream stream, out BaseResult baseResult)
        {
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var serializer = CreateJsonSerializer();
                return new DocumentParser(db).ParseBatchResult<T>(jsonReader, out baseResult);
            }
        }

        public T DeserializeSingleResult<T>(Stream stream, out BaseResult baseResult)
        {
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var serializer = CreateJsonSerializer();
                return new DocumentParser(db).ParseBatchResult<T>(jsonReader, out baseResult).FirstOrDefault();
            }
        }

        public T DeserializeSingleResult<T>(Stream stream,out JObject jObject)
        {
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var serializer = CreateJsonSerializer();
                return new DocumentParser(db).ParseSingleResult<T>(jsonReader,out jObject,true);
            }
        }

        public T DeserializeSingleResult<T>(JsonTextReader reader, out JObject jObject)
        {
            var serializer = CreateJsonSerializer();
            return new DocumentParser(db).ParseSingleResult<T>(reader, out jObject);
        }

        public JsonSerializer CreateJsonSerializer()
        {
            var jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings 
            { 
                ContractResolver = new DocumentContractResolver(db),
                Converters = new JsonConverter[] 
                {
                    new DateTimeConverter(db)
                },
                DateParseHandling = DateParseHandling.None
            });

            return jsonSerializer;
        }
    }
}
