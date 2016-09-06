using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Serialization.Converters
{
    public class DateTimeConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dateOffset = value as DateTimeOffset?;
            if (dateOffset.HasValue)
            {
                writer.WriteValue(dateOffset.Value.ToString("o", CultureInfo.InvariantCulture));
                return;
            }

            var date = value as DateTime?;
            if(date.HasValue)
            {
                writer.WriteValue(date.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture));
                return;
            }

            writer.WriteNull();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.Date)
            {
                if (objectType == typeof(DateTimeOffset))
                    return reader.Value is DateTimeOffset ? reader.Value : new DateTimeOffset((DateTime)reader.Value);

                return reader.Value;
            }

            if (reader.TokenType != JsonToken.String)
                throw new Exception(string.Format("Unexpected token parsing date. Expected String, got {0}.",reader.TokenType.ToString()));


            var time = reader.Value as string;

            if (time == null)
                return null;
            
            if (objectType == typeof(DateTime) || objectType == typeof(DateTime?))
            {
                return DateTime.Parse(time,CultureInfo.InvariantCulture,DateTimeStyles.RoundtripKind);
            }

            if (objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?))
            {
                return DateTimeOffset.Parse(time, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }

            return null;
        }
    }
}
