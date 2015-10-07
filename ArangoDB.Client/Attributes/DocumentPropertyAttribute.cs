using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    [AttributeUsageAttribute(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class DocumentPropertyAttribute : Attribute, IDocumentPropertySetting
    {
        public string PropertyName { get; set; }

        public bool IgnoreProperty { get; set; }

        public NamingConvention Naming { get; set; }

        public IdentifierType Identifier { get; set; }
    }
}
