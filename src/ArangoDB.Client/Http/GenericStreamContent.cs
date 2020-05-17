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
        private readonly MemoryStream _stream;

        public GenericStreamContent(Func<StreamWriter, Task> onStreamReady)
        {
            if (onStreamReady == null)
                throw new ArgumentNullException(nameof(onStreamReady));

            _stream = new MemoryStream();
            Headers.ContentType = new MediaTypeHeaderValue("application/json");

            onStreamReady(new StreamWriter(_stream));
            _stream.Position = 0;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            _stream.CopyTo(stream);
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _stream.Length;
            return true;
        }
    }
}
