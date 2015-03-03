using ArangoDB.Client.Common.Newtonsoft.Json;
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
        ArangoDatabase db;
        public DocumentSerializer(ArangoDatabase db)
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

        public Stream Serialize(object value,Stream stream)
        {
            using (var streamWriter = new StreamWriter(stream))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                var serializer = CreateJsonSerializer();

                serializer.Serialize(jsonWriter, value);
            }

            return stream;
        }

        public T DeserializeFromJsonTextReader<T>(JsonTextReader reader)
        {
            var serializer = CreateJsonSerializer();

            return serializer.Deserialize<T>(reader);
        }

        public void SerializeFromJsonWriter(JsonWriter writer,object value)
        {
            var serializer = CreateJsonSerializer();

            serializer.Serialize(writer, value);
        }

        public JsonSerializer CreateJsonSerializer()
        {
            var jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings 
            { 
                ContractResolver = new DocumentContractResolver(db)
            });

            return jsonSerializer;
        }
    }
}
