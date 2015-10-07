using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    [AttributeUsageAttribute(AttributeTargets.Method, AllowMultiple = false)]
    public class UserFunctionAttribute:Attribute
    {
        public string Name { get; set; }
    }
}
