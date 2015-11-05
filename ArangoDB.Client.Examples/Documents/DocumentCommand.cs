using ArangoDB.Client.Examples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArangoDB.Client.Examples.Documents
{
    public class DocumentCommand : DatabaseSetup
    {
        Person InsertAPerson()
        {
            Person person = new Person
            {
                Name = "raoof hojat",
                Age = 28
            };

            db.Insert<Person>(person);

            return person;
        }

        [Fact]
        public void Insert()
        {
            var person = InsertAPerson();

            Assert.NotNull(person.Key);
        }
        
        [Fact]
        public void Update()
        {
            var person = InsertAPerson();

            person.Age = 20;

            db.Update<Person>(person);

            Assert.Equal(db.Document<Person>(person.Key).Age,20);
        }

        [Fact]
        public void Document()
        {
            var person = InsertAPerson();

            var loadedPerson = db.Document<Person>(person.Key);

            Assert.Equal(person.Name, loadedPerson.Name);
            Assert.Equal(person.Key, loadedPerson.Key);
        }

        [Fact]
        public void Remove()
        {
            var person = InsertAPerson();

            db.Remove<Person>(person);

            Assert.Equal(db.Document<Person>(person.Key), null);
        }
    }
}
