using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.ChangeTracking
{
    public class DocumentContainer
    {
        public string Id { get; internal set; }

        public string Rev { get; internal set; }

        public string Key { get; internal set; }

        public string From { get; internal set; }

        public string To { get; internal set; }

        public JObject Document { get; internal set; }
    }
}
