using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArangoDB.Client.Http
{
    public class HttpConnectionHandler : DelegatingHandler
    {
        //Action<HttpRequestMessage> beforeRequest = (HttpRequestMessage request) => { };
        //Action<HttpResponseMessage> afterRespone = (HttpResponseMessage request) => { };

        public HttpConnectionHandler(
            //Action<HttpRequestMessage> BeforeRequest = null, Action<HttpResponseMessage> AfterRequest=null
            )
        {
            //if (BeforeRequest != null)
            //    beforeRequest = BeforeRequest;

            //if (AfterRequest != null)
            //    afterRespone = AfterRequest;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //beforeRequest(request);

            var responseMessage = await base.SendAsync(request,cancellationToken);

            //afterRespone(responseMessage);

            return responseMessage;
        }
    }
}
