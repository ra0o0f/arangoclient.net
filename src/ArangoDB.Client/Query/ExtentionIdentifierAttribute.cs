using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Query
{
    public class ExtentionIdentifierAttribute : Attribute
    {
        public ExtentionIdentifierAttribute(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; set; }
    }
}
