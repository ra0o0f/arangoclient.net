using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ArangoDB.Client.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Http
{
    // from http://stackoverflow.com/questions/25335897/using-json-net-to-serialize-object-into-httpclients-response-stream
    public class JsonContent : HttpContent
    {
        IArangoDatabase db;

        object data { get; set; }
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
                var jsonWriter = new JsonTextWriter(streamWriter);
                jObject.WriteTo(jsonWriter);
                await streamWriter.FlushAsync().ConfigureAwait(false);
            }
            else
            {
                var docSerializer = new DocumentSerializer(db);
                var streamWriter = new StreamWriter(stream);
                var jsonWriter = new JsonTextWriter(streamWriter);
                var serializer = docSerializer.CreateJsonSerializer();
                serializer.Serialize(jsonWriter, data);
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
