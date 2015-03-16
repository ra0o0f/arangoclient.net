using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Test.Model
{
    public class Person
    {
        public string Fullname { get; set; }

        public int Age { get; set; }

        public int Height { get; set; }

        public Outfit Outfit { get; set; }
    }

    public class Outfit
    {
        public string Color { get; set; }
    }

    // for merged result test that cant be tracked by client
    public class Flight : BaseResult
    {
        [DocumentProperty(Identifier = IdentifierType.Key)]
        public string Key { get; set; }

        [DocumentProperty(Identifier = IdentifierType.Handle)]
        public string Id { get; set; }

        [DocumentProperty(Identifier = IdentifierType.Revision)]
        public string Rev { get; set; }

        public string FlightNumber { get; set; }

        public string Airline { get; set; }
    }
}
