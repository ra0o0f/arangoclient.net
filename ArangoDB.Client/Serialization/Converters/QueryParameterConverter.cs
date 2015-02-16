using ArangoDB.Client.Common.Newtonsoft.Json;
using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Serialization.Converters
{
    public class QueryParameterConverter : JsonConverter
    {
        public override bool CanConvert(Type type)
        {
            throw new NotImplementedException("should use only with attribute");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            List<QueryParameter> parameters = value as List<QueryParameter>;

            if (parameters != null && parameters.Count != 0)
            {
                writer.WriteStartObject();
                foreach (var p in parameters)
                {
                    writer.WritePropertyName(p.Name);

                    var jsonSerializer = JsonSerializer.Create();
                    jsonSerializer.Serialize(writer, p.Value);
                    //writer.WriteValue(p.Value);
                }
                writer.WriteEnd();
            }
            else
                writer.WriteNull();
                
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("just need serialization");
        }
    }
}
