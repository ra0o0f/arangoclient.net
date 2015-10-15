using ArangoDB.Client.Common.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client.Utility;

namespace ArangoDB.Client.Serialization.Converters
{
    public class EnumValueConverter : JsonConverter
    {
        private static Dictionary<Type, Func<object,string>> enumToStrings = new Dictionary<Type, Func<object, string>>
        {
            [typeof(UniquenessType?)] = (v) => { return Utils.UniquenessTypeToString((UniquenessType)v); },
            [typeof(TraversalStrategy?)] = (v) => { return Utils.TraversalStrategyToString((TraversalStrategy)v); },
            [typeof(TraversalOrder?)] = (v) => { return Utils.TraversalOrderToString((TraversalOrder)v); },
            [typeof(TraversalItemOrder?)] = (v) => { return Utils.TraversalItemOrderToString((TraversalItemOrder)v); },
            [typeof(EdgeDirection?)] = (v) => { return Utils.EdgeDirectionToString((EdgeDirection)v); }
        };

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(UniquenessType) || objectType == typeof(UniquenessType?)
                || objectType == typeof(TraversalStrategy) || objectType == typeof(TraversalStrategy?)
                || objectType == typeof(TraversalOrder) || objectType == typeof(TraversalOrder?)
                || objectType == typeof(TraversalItemOrder) || objectType == typeof(TraversalItemOrder?)
                || objectType == typeof(EdgeDirection) || objectType == typeof(EdgeDirection?))
                return true;

            return false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if(value == null)
            {
                writer.WriteNull();
                return;
            }

            if ((value as UniquenessType?).HasValue)
                writer.WriteValue(enumToStrings[typeof(UniquenessType?)](value));

            else if ((value as TraversalStrategy?).HasValue)
                writer.WriteValue(enumToStrings[typeof(TraversalStrategy?)](value));

            else if ((value as TraversalOrder?).HasValue)
                writer.WriteValue(enumToStrings[typeof(TraversalOrder?)](value));

            else if ((value as TraversalItemOrder?).HasValue)
                writer.WriteValue(enumToStrings[typeof(TraversalItemOrder?)](value));

            else if ((value as EdgeDirection?).HasValue)
                writer.WriteValue(enumToStrings[typeof(EdgeDirection?)](value));

            else
                throw new InvalidOperationException($"cant find binding to serialize type {value.GetType()}, this is client bug");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("just need serialization");
        }
    }
}
