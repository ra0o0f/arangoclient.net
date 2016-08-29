using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Http
{
    public interface IHttpConnection
    {
        Task<HttpResponseMessage> SendCommandAsync(HttpMethod method, Uri uri, object data, Func<StreamWriter, Task> onStreamReady, NetworkCredential credential, Dictionary<string, string> headers);
    }
}
