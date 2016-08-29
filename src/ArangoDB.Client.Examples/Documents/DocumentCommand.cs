using ArangoDB.Client.Examples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArangoDB.Client.Examples.Documents
{
    public class DocumentCommand : TestDatabaseSetup
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

            Assert.Equal(db.Document<Person>(person.Key).Age, 20);
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
        public void DocumentIfMatchRev()
        {
            var person = InsertAPerson();

            var personInfo = db.FindDocumentInfo(person);

            var loadedPerson = db.Document<Person>(person.Key, ifMatchRev: personInfo.Rev);

            Assert.Equal(person.Name, loadedPerson.Name);
            Assert.Equal(person.Key, loadedPerson.Key);
        }

        [Fact]
        public void DocumentIfMatchRevFailed()
        {
            var person = InsertAPerson();

            var personInfo = db.FindDocumentInfo(person);

            Assert.Throws<ArangoServerException>(() => db.Document<Person>(person.Key, ifMatchRev: $"{personInfo.Rev}1"));
        }

        [Fact]
        public void DocumentIfNotMatchRev()
        {
            var person = InsertAPerson();

            var personInfo = db.FindDocumentInfo(person);

            var loadedPerson = db.Document<Person>(person.Key, ifNoneMatchRev: personInfo.Rev);

            Assert.Null(loadedPerson);
        }

        [Fact]
        public void DocumentIfNotMatchRevFailed()
        {
            var person = InsertAPerson();

            var personInfo = db.FindDocumentInfo(person);

            var loadedPerson = db.Document<Person>(person.Key, ifNoneMatchRev: $"{personInfo.Rev}1");

            Assert.Equal(person.Name, loadedPerson.Name);
            Assert.Equal(person.Key, loadedPerson.Key);
        }

        [Fact]
        public void Remove()
        {
            var person = InsertAPerson();

            db.Remove<Person>(person);

            Assert.Null(db.Document<Person>(person.Key));
        }

        [Fact]
        public void Replace()
        {
            var person = InsertAPerson();

            person.Age = 20;

            db.Replace<Person>(person);

            Assert.Equal(db.Document<Person>(person.Key).Age, 20);
        }

        [Fact]
        public void UpdateById()
        {
            var person = InsertAPerson();

            db.UpdateById<Person>(person.Key, new { Age = 20 });

            Assert.Equal(db.Document<Person>(person.Key).Age, 20);
            Assert.Equal(db.Document<Person>(person.Key).Name, person.Name);
        }

        [Fact]
        public void ReplaceById()
        {
            var person = InsertAPerson();

            db.ReplaceById<Person>(person.Key, new { Age = 20 });

            Assert.Null(db.Document<Person>(person.Key).Name);
            Assert.Equal(db.Document<Person>(person.Key).Age, 20);
        }

        [Fact]
        public void RemoveById()
        {
            var person = InsertAPerson();

            db.RemoveById<Person>(person.Key);

            Assert.Null(db.Document<Person>(person.Key));
        }
    }
}
