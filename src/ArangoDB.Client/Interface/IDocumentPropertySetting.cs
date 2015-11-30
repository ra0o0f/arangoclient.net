using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public interface IDocumentPropertySetting
    {
        string PropertyName { get; set; }

        bool IgnoreProperty { get; set; }

        NamingConvention Naming { get; set; }

        IdentifierType Identifier { get; set; }
    }

    public enum IdentifierType
    {
        None = 0,
        Key = 1,
        Handle = 2,
        Revision = 3,
        EdgeFrom = 4,
        EdgeTo = 5
    }

    public enum NamingConvention
    {
        UnChanged = 0,
        ToCamelCase = 1
    }
}
