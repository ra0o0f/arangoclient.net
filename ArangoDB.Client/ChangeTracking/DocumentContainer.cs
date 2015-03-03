using ArangoDB.Client.Common.Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.ChangeTracking
{
    public class DocumentContainer
    {
        public string Id { get; set; }

        public string Rev { get; set; }

        public string Key { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public JObject Document { get; set; }
    }
}
