using ArangoDB.Client.Examples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArangoDB.Client.Examples.SimpleQueries
{
    public class SimpleQueryCommand : TestDatabaseSetup
    {
        List<Person> InsertSomePerson()
        {
            List<Person> persons = new List<Person>()
            {
                new Person { Name = "raoof",Age = 28 },
                new Person { Name = "Dr. frank celler",Age=30 },
                new Person {Name = "hojat",Age=27 }
            };

            foreach (var p in persons)
                db.Insert<Person>(p);

            return persons;
        }

        [Fact]
        public void All()
        {
            var persons = InsertSomePerson();

            var loadedPersons = db.All<Person>().ToList();

            Assert.Equal(loadedPersons.Count, 3);

            Assert.Empty(loadedPersons.Select(p => p.Key)
                .Except(persons.Select(p=>p.Key)));
        }

        [Fact]
        public void Any()
        {
            var persons = InsertSomePerson();

            var person = db.Any<Person>();

            Assert.Contains(person.Key, persons.Select(p => p.Key));
        }

        [Fact]
        public void ByExample()
        {
            var persons = InsertSomePerson();

            var loadedPersons = db.ByExample<Person>(new { Name = "Dr. frank celler" }).ToList();

            Assert.Equal(loadedPersons.Count, 1);
            Assert.Equal(loadedPersons[0].Key, persons.First(p => p.Name == "Dr. frank celler").Key);
        }
    }
}
