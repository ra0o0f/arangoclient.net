using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Examples.SetupClient
{
    public class ClientSetting
    {
        public void SettingProxy()
        {
            // all http requests goes through http://localhost:10099
            ArangoDatabase.ClientSetting.Proxy = new WebProxy("http://localhost:10099");
        }
    }
}
