using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ArangoDB.Client.Data;
using ArangoDB.Client.Http;
using ArangoDB.Client.Serialization;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ArangoDB.Client.Cursor
{
    public class CursorAsyncEnumerator<T> : ICursorAsyncEnumerator<T>
    {
        bool initialized;

        IArangoDatabase db;
        HttpCommand initiateCommand;
        object data;

        public CursorResult CursorResult { get; private set; }

        Stream stream;
        StreamReader streamReader;
        JsonTextReader jsonTextReader;

        ReaderState readerState;

        public CursorAsyncEnumerator(IArangoDatabase db, HttpCommand command, object data = null)
        {
            this.db = db;
            this.initiateCommand = command;
            this.CursorResult = new CursorResult { HasMore = true };
            this.data = data;
            readerState = new ReaderState();
        }

        public T Current { get; private set; }

        async Task MakeNextRequest()
        {
            HttpResponseMessage response;

            if (!initialized)
            {
                response = await initiateCommand.SendCommandAsync(data).ConfigureAwait(false);
                initialized = true;
            }
            else
            {
                HttpCommand command = new HttpCommand(db)
                {
                    Api = CommandApi.Cursor,
                    Command = CursorResult.Id,
                    Method = HttpMethod.Put
                };

                response = await command.SendCommandAsync().ConfigureAwait(false);
            }

            stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            streamReader = new StreamReader(stream);
            jsonTextReader = new ArangoJsonTextReader(streamReader);

            CursorResult.RequestCount++;
        }

        async Task<bool> SetCurrent()
        {
            while (readerState.ReadNextProperty(jsonTextReader))
            {
                readerState.ReadNextPropertyValue(jsonTextReader);
                CursorResult.SetFromJsonTextReader(readerState.PropertyName, readerState.Token, readerState.Value);
            }

            if (jsonTextReader.TokenType == JsonToken.EndObject)
            {
                new BaseResultAnalyzer(db).ThrowIfNeeded(CursorResult);

                if (db.Setting.ThrowForServerErrors == false && CursorResult.HasError())
                    return false;
            }

            if (readerState.ReadNextArrayValue(jsonTextReader))
            {
                var documentSerializer = new DocumentSerializer(db);
                if (db.Setting.DisableChangeTracking == true || jsonTextReader.TokenType != JsonToken.StartObject)
                {
                    Current = documentSerializer.Deserialize<T>(jsonTextReader);
                }
                else
                {
                    JObject jObject = null;
                    Current = documentSerializer.DeserializeSingleResult<T>(jsonTextReader, out jObject);
                    db.ChangeTracker.TrackChanges(Current, jObject);
                }
                return true;
            }
            else
            {
                while (readerState.ReadNextProperty(jsonTextReader))
                {
                    readerState.ReadNextPropertyValue(jsonTextReader);
                    CursorResult.SetFromJsonTextReader(readerState.PropertyName, readerState.Token, readerState.Value);
                }

                if (CursorResult.HasMore)
                {
                    Dispose();
                    await MakeNextRequest().ConfigureAwait(false);
                    readerState.InitiateRead(jsonTextReader);
                    return await SetCurrent().ConfigureAwait(false);
                }
                else
                {
                    Dispose();
                    return false;
                }
            }
        }

        public async Task<bool> MoveNextAsync()
        {
            if (!initialized)
            {
                await MakeNextRequest().ConfigureAwait(false);
                readerState.InitiateRead(jsonTextReader);
                initialized = true;
            }

            return await SetCurrent().ConfigureAwait(false);
        }

        class ReaderState
        {
            bool resultArrayStarted;

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
                if (reader.TokenType == JsonToken.EndObject)
                    return false;

                if (reader.TokenType != JsonToken.PropertyName)
                    throw new InvalidOperationException("Expected JsonToken.PropertyName");

                PropertyName = reader.Value.ToString();

                if (PropertyName == "result")
                {
                    reader.Read();
                    if (reader.TokenType != JsonToken.StartArray)
                        throw new InvalidOperationException("Expected JsonToken.StartArray");

                    resultArrayStarted = true;
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
                reader.Read();
                if (reader.TokenType == JsonToken.EndArray)
                {
                    resultArrayStarted = false;
                    return false;
                }

                return true;
            }
        }

        public void Dispose()
        {
            if (initialized)
            {
                jsonTextReader.Close();
                streamReader.Dispose();
                stream.Dispose();
            }
        }

    }
}
