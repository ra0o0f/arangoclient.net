using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Test.Model
{
    public class ComplexModel
    {
        public string Name { get; set; }

        public NestedModel Nested { get; set; }

        [CollectionProperty(CollectionName = "nested")]
        public class NestedModel
        {
            [DocumentProperty(PropertyName = "fullname")]
            public string Name { get; set; }

            public NestedNestedModel NestedNested { get; set; }
        }

        [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
        public class NestedNestedModel
        {
            public string Name { get; set; }
        }
    }
}
