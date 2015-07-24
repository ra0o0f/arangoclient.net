using ArangoDB.Client.Common.Newtonsoft.Json;
using ArangoDB.Client.Common.Newtonsoft.Json.Linq;
using ArangoDB.Client.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client.Serialization.Converters;

namespace ArangoDB.Client.Http
{
    public enum HttpSerializationMethod
    {
        Documents = 1,
        Array = 2
    }

    // from http://stackoverflow.com/questions/25335897/using-json-net-to-serialize-object-into-httpclients-response-stream
    public class JsonContent : HttpContent
    {
        IArangoDatabase db;

        object data { get; set; }

        public HttpSerializationMethod? SerializationMethod { get; set; }

        public JsonContent(IArangoDatabase db,object data)
        {
            this.db = db;
            this.data = data;
            this.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            var jObject = data as JObject;
            if (jObject != null)
            {
                var streamWriter = new StreamWriter(stream);
                await streamWriter.WriteAsync(jObject.ToString(Formatting.None)).ConfigureAwait(false);
                await streamWriter.FlushAsync().ConfigureAwait(false);
            }
            else
            {
                var docSerializer = new DocumentSerializer(db);
                var streamWriter = new StreamWriter(stream);
                var jsonWriter = new JsonTextWriter(streamWriter);
                var serializer = docSerializer.CreateJsonSerializer();
                if (data is IEnumerable && SerializationMethod == HttpSerializationMethod.Documents)
                {
                    var first = true;
                    foreach (var dataDocument in (IEnumerable)data)
                    {
                        if (!first)
                        {
                            streamWriter.Write('\n');
                        }
                        serializer.Serialize(jsonWriter, dataDocument);
                        first = false;
                    }
                }
                else
                {
                    serializer.Serialize(jsonWriter, data);
                }
                await streamWriter.FlushAsync().ConfigureAwait(false);
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            //we don't know. can't be computed up-front
            length = -1;
            return false;
        }
    }
}
