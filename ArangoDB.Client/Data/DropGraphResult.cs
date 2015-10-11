using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class DropGraphResult : BaseResult
    {
        public bool Removed { get; set; }
    }
}
