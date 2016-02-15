using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public enum IndexType
    {
        CapConstraint = 0,
        Hash = 1,
        Skiplist = 2,
        Geo = 3,
        Fulltext = 4
    }
}
