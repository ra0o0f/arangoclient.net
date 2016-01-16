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
    public class GenericStreamContent : HttpContent
    {
        IArangoDatabase db;
        Func<StreamWriter, Task> onStreamReady;

        public GenericStreamContent(IArangoDatabase db, Func<StreamWriter, Task> onStreamReady)
        {
            if (onStreamReady == null)
                throw new ArgumentNullException(nameof(onStreamReady));

            this.db = db;
            this.onStreamReady = onStreamReady;
            this.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            await onStreamReady(new StreamWriter(stream)).ConfigureAwait(false);
        }

        protected override bool TryComputeLength(out long length)
        {
            //we don't know. can't be computed up-front
            length = -1;
            return false;
        }
    }
}
