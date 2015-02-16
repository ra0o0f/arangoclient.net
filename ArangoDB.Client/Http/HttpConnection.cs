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
    public class HttpConnection
    {
        ArangoDatabase db;

        private static Lazy<HttpClient> httpClientLazily = new Lazy<HttpClient>(() => {
            connectionHandler = new HttpConnectionHandler();
            var proxy = ArangoDatabase.ClientSetting.Proxy;
            connectionHandler.InnerHandler = new HttpClientHandler
            {
                UseProxy = proxy != null,
                Proxy = proxy
            };

            ArangoDatabase.ClientSetting.IsHttpClientInitialied = true;

            var httpClient = new HttpClient(connectionHandler, true);
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            
            return httpClient;
        });

        private static HttpClient httpClient
        { get { return httpClientLazily.Value; } }

        private static HttpConnectionHandler connectionHandler;

        private static HttpClientHandler clientHandler
        {
            get { return connectionHandler.InnerHandler as HttpClientHandler; }
        }

        public HttpConnection(ArangoDatabase db)
        {
            this.db = db;
        }

        public async Task<HttpResponseMessage> SendCommandAsync(HttpMethod method,Uri uri,object data)
        {
            if(!db.HttpInitialized)
            {
#if !PORTABLE
                Uri baseUri = new Uri(db.Settings.Url);
                var servicePoint = ArangoDatabase.ClientSetting.Proxy != null ? ServicePointManager.FindServicePoint(baseUri
                    , ArangoDatabase.ClientSetting.Proxy)
                : ServicePointManager.FindServicePoint(baseUri);
                servicePoint.UseNagleAlgorithm = false;
                servicePoint.Expect100Continue = false;
                servicePoint.ConnectionLimit = 256;
#endif

                db.HttpInitialized = true;
            }

            var requestMessage = new HttpRequestMessage(method,uri);

            if(data!=null)
                requestMessage.Content = new JsonContent(db,data);

            var responseMessage = await httpClient.SendAsync(requestMessage).ConfigureAwait(false);

            return responseMessage;
        }
    }
}
