using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public enum ImportDuplicatePolicy
    {
        Error = 0,
        Update = 1,
        Replace = 2,
        Ignore = 3
    }
}
