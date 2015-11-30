using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class GraphResult : BaseResult
    {
        public GraphIdentifierResult Graph { get; set; }
    }
}
