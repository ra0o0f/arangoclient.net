using ArangoDB.Client.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Http
{

    //public class HttpConnectionOption
    //{
    //    //public Action<HttpRequestMessage> BeforeRequest { get; set; }

    //    //public Action<HttpResponseMessage> AfterResponse { get; set; }

    //    public Uri BaseUri { get; set; }

    //    public Action BeforeFirstRequest { get; set; }

    //    public static HttpConnectionOption CreateHttpOption(ArangoDatabase db,Action beforeFirstRequest)
    //    {
    //        return new HttpConnectionOption
    //        {
    //            //AfterResponse = db.Settings.AfterHttpResponse,
    //            //BeforeRequest = db.Settings.BeforeHttpRequest,
    //            BeforeFirstRequest = beforeFirstRequest,
    //            BaseUri = new Uri(db.Settings.Url)
    //        };
    //    }
    //}
}
