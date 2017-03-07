using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Examples.Models
{
    public class Person
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        [DocumentProperty(Identifier = IdentifierType.Handle)]
        public string Id { get; set; }

        public override string ToString()
        {
            return Name?.ToString();
        }
    }
}
