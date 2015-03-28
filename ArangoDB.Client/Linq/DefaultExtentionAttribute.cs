using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Linq
{
    public class DefaultExtentionAttribute : Attribute
    {
        public DefaultExtentionAttribute()
        {
            Name = "default";
        }

        public string Name { get; set; }
    }
}
