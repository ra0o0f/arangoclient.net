using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class GraphCollectionResult : BaseResult
    {
        public List<string> Collections { get; set; }
    }
}
