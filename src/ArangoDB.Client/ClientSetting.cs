using ArangoDB.Client.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public class ClientSetting
    {
        private IWebProxy _proxy;
        private TimeSpan? _httpRequestTimeout;

        public TimeSpan? HttpRequestTimeout
        {
            get { return _httpRequestTimeout; }
            set
            {
                if (HttpClientInitialized)
                    throw new InvalidOperationException("Timout should be set before first request to server");

                _httpRequestTimeout = value;
            }
        }

        public IWebProxy Proxy 
        {
            get { return _proxy; }
            set
            {
                if (HttpClientInitialized)
                    throw new InvalidOperationException("Proxy should be set before first request to server");

                _proxy = value;
            }
        }

        internal bool HttpClientInitialized;
    }
}
