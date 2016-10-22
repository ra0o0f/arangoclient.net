using ArangoDB.Client.Data;
using ArangoDB.Client.Serialization.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArangoDB.Client.Serialization
{
    public class DocumentSerializer
    {
        private IArangoDatabase db;

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

        public T DeserializeSingleResult<T>(Stream stream, out JObject jObject)
        {
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var serializer = CreateJsonSerializer();
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
            if (db.Setting.SharedSetting.CustomJsonSerializer == null)
            {
                var jsonSerializer = JsonSerializer.Create(SerializerSetting);

                return jsonSerializer;
            }
            return db.Setting.SharedSetting.CustomJsonSerializer(db, GetJsonConverters());
        }

        public JsonSerializerSettings SerializerSetting
        {
            get
            {
                List<JsonConverter> convertes = GetJsonConverters();

                return new JsonSerializerSettings
                {
                    ContractResolver = DocumentContractResolver.GetContractResolver(db),
                    Converters = convertes,
                    DateParseHandling = DateParseHandling.None
                };
            }
        }

        private List<JsonConverter> GetJsonConverters()
        {
            var convertes = new List<JsonConverter>
                {
                    new DateTimeConverter(),
                    new QueryParameterConverter(),
                    new EnumValueConverter()
                }.Concat(db.Setting.Serialization.Converters).ToList();

            if (db.Setting.Serialization.SerializeEnumAsInteger == false)
                convertes.Add(new StringEnumConverter());
            return convertes;
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
