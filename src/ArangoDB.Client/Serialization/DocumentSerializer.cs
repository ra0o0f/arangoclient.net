using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
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
            using (var jsonReader = new ArangoJsonTextReader(streamReader))
            {
                return Deserialize<T>(jsonReader);
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
            using (var jsonReader = new ArangoJsonTextReader(streamReader))
            {
                var serializer = CreateJsonSerializer();
                return new DocumentParser(db).ParseBatchResult<T>(jsonReader, out baseResult);
            }
        }

        public T DeserializeSingleResult<T>(Stream stream, out BaseResult baseResult)
        {
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new ArangoJsonTextReader(streamReader))
            {
                var serializer = CreateJsonSerializer();
                return new DocumentParser(db).ParseBatchResult<T>(jsonReader, out baseResult).FirstOrDefault();
            }
        }

        public T DeserializeSingleResult<T>(Stream stream, out JObject jObject)
        {
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new ArangoJsonTextReader(streamReader))
            {
                return new DocumentParser(db).ParseSingleResult<T>(jsonReader, out jObject, true);
            }
        }

        public T DeserializeSingleResult<T>(JsonTextReader reader, out JObject jObject)
        {
            var serializer = CreateJsonSerializer();
            return new DocumentParser(db).ParseSingleResult<T>(reader, out jObject);
        }

        public JsonSerializer CreateJsonSerializer()
        {
            var jsonSerializer = JsonSerializer.Create(SerializerSetting);

            return jsonSerializer;
        }

        public JsonSerializerSettings SerializerSetting
        {
            get
            {
                var convertes = new List<JsonConverter>
                {
                    new DateTimeConverter(),
                    new QueryParameterConverter(),
                    new EnumValueConverter()
                }.Concat(db.Setting.Serialization.Converters).ToList();

                if (db.Setting.Serialization.SerializeEnumAsInteger == false)
                    convertes.Add(new StringEnumConverter());

                return new JsonSerializerSettings
                {
                    ContractResolver = DocumentContractResolver.GetContractResolver(db),
                    Converters = convertes,
                    DateParseHandling = DateParseHandling.None,
                    MetadataPropertyHandling = db.Setting.Serialization.MetadataPropertyHandling
                };
            }
        }

        public JObject FromObject(object document)
        {
            return JObject.FromObject(document, CreateJsonSerializer());
        }

        public string SerializeWithoutReader(object document)
        {
            return JsonConvert.SerializeObject(document, SerializerSetting);
        }

        public T DeserializeWithoutReader<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, SerializerSetting);
        }
    }
}
