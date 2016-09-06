using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        IArangoDatabase db;

        public DocumentParser(IArangoDatabase db)
        {
            this.db = db;
        }

        /// <summary>
        /// Parse Merge and Distinct Results
        /// </summary>
        /// <returns></returns>
        public T ParseSingleResult<T>(JsonTextReader reader,out JObject jObject,bool readForObjectStart=false)
        {
            if (readForObjectStart == true)
                reader.Read();

            if (reader.TokenType != JsonToken.StartObject)
                throw new InvalidOperationException("Expecting JsonToken.StartObject");

            jObject = JObject.Load(reader);

            var serializer = new DocumentSerializer(db).CreateJsonSerializer();
            return jObject.ToObject<T>(serializer);
        }

        public List<T> ParseBatchResult<T>(JsonTextReader jsonTextReader,out BaseResult baseResult)
        {
            var readerState = new ReaderState();
            baseResult = new BaseResult();
            var result = new List<T>();

            jsonTextReader.Read();

            while (readerState.ReadNextProperty(jsonTextReader))
            {
                readerState.ReadNextPropertyValue(jsonTextReader);
                baseResult.SetFromJsonTextReader(readerState.PropertyName, readerState.Token, readerState.Value);
            }

            if (jsonTextReader.TokenType == JsonToken.EndObject)
            {
                // no need to check for base result
                //new BaseResultAnalyzer(db).ThrowIfNeeded(baseResult);
                return result;
            }

            while (readerState.ReadNextArrayValue(jsonTextReader))
            {
                JObject jObject = null;
                var document = ParseSingleResult<T>(jsonTextReader, out jObject);
                db.ChangeTracker.TrackChanges(document, jObject);
                result.Add(document);
            }

            while (readerState.ReadNextProperty(jsonTextReader))
            {
                readerState.ReadNextPropertyValue(jsonTextReader);
                baseResult.SetFromJsonTextReader(readerState.PropertyName, readerState.Token, readerState.Value);
            }

            return result;
        }

        class ReaderState
        {
            bool resultArrayStarted;

            bool resultSingleStarted;

            public string PropertyName { get; set; }
            public object Value { get; set; }
            public JsonToken Token { get; set; }

            public void InitiateRead(JsonTextReader reader)
            {
                reader.Read();

                if (reader.TokenType != JsonToken.StartObject)
                    throw new InvalidOperationException("Expected JsonToken.StartObject");
            }

            public bool ReadNextProperty(JsonTextReader reader)
            {
                if (resultArrayStarted)
                    return false;

                reader.Read();

                // reach end of object
                if (reader.TokenType == JsonToken.EndObject || reader.TokenType == JsonToken.None)
                    return false;

                if (reader.TokenType != JsonToken.PropertyName)
                    throw new InvalidOperationException("Expected JsonToken.PropertyName");

                PropertyName = reader.Value.ToString();

                if (PropertyName == "result" || PropertyName == "edges" || PropertyName== "document" || PropertyName == "vertex"
                     || PropertyName == "edge")
                {
                    reader.Read();

                    if (reader.TokenType == JsonToken.StartArray)
                        resultArrayStarted = true;
                    else if (reader.TokenType == JsonToken.StartObject)
                        resultSingleStarted = true;
                    else
                        throw new InvalidOperationException("Expected JsonToken.StartArray or JsonToken.StartObject");

                    return false;
                }

                return true;
            }

            public void ReadNextPropertyValue(JsonTextReader reader)
            {
                reader.Read();
                Token = reader.TokenType;

                if (reader.TokenType == JsonToken.StartObject)
                {
                    Value = JObject.Load(reader);
                }
                else
                    Value = reader.Value;
            }

            public bool ReadNextArrayValue(JsonTextReader reader)
            {
                if (resultArrayStarted)
                {
                    reader.Read();
                    if (reader.TokenType == JsonToken.EndArray)
                    {
                        resultArrayStarted = false;
                        return false;
                    }
                    return true;
                }
                else if (resultSingleStarted)
                {
                    resultSingleStarted = false;
                    return true;
                }
                else
                    return false;
            }
        }
    }
}
