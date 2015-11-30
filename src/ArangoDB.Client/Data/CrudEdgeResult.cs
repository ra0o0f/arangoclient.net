using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class CrudEdgeResult : BaseResult
    {
        public DocumentIdentifierWithoutBaseResult Edge { get; set; }
    }
}
