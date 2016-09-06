using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    [AttributeUsageAttribute(AttributeTargets.Class, AllowMultiple = false)]
    public class CollectionPropertyAttribute : Attribute, ICollectionPropertySetting
    {
        public string CollectionName { get; set; }

        public NamingConvention Naming { get; set; }
    }
}
