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

        public IWebProxy Proxy 
        {
            get { return _proxy; }
            set
            {
                if (IsHttpClientInitialied)
                    throw new InvalidOperationException("Proxy should be set before first request to server");

                _proxy = value;
            }
        }

        internal volatile bool IsHttpClientInitialied;
    }
}
