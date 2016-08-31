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
        public void InsertMultipleFailed()
        {
            ClearDatabase();

            List<Person> persons = new List<Person>
            {
                new Person
                {
                    Key = "K",
                    Age = 21,
                    Name = "A"
                },
                new Person
                {
                    Key = "K",
                    Age = 22,
                    Name = "B"
                }
            };

            var results = db.InsertMultiple<Person>(persons, baseResults: (bs) =>
            {
                Assert.Equal(bs.Count, 2);
                Assert.True(bs[0].HasError() == false);
                Assert.True(bs[1].HasError());
            });

            Assert.Equal(results.Count, 2);
            Assert.True(string.IsNullOrEmpty(results[0].Key) == false);
            Assert.True(string.IsNullOrEmpty(results[1].Key));
        }

        [Fact]
        public void InsertMultiple()
        {
            ClearDatabase();

            List<Person> persons = new List<Person>
            {
                new Person
                {
                    Age = 21,
                    Name = "A"
                },
                new Person
                {
                    Age = 22,
                    Name = "B"
                }
            };

            var results = db.InsertMultiple<Person>(persons, baseResults: (bs) =>
            {
                Assert.Equal(bs.Count, 2);
                Assert.True(bs.All(x => x.HasError() == false));
            });

            Assert.Equal(results.Count, 2);
            Assert.True(results.All(x => string.IsNullOrEmpty(x.Key) == false));
        }

        [Fact]
        public void InsertFailed()
        {
            ClearDatabase();

            var person = InsertAPerson();

            db.Setting.ThrowForServerErrors = false;

            var result = db.Insert<Person>(person, baseResult: (b) =>
             {
                 Assert.True(b.HasError());
             });

            db.Setting.ThrowForServerErrors = true;

            Assert.NotNull(result);

            Assert.Null(result.Key);
        }

        [Fact]
        public void Insert()
        {
            ClearDatabase();

            var person = InsertAPerson();

            Assert.NotNull(person.Key);
        }

        [Fact]
        public void Document()
        {
            ClearDatabase();

            var person = InsertAPerson();

            var loadedPerson = db.Document<Person>(person.Key);

            Assert.Equal(person.Name, loadedPerson.Name);
            Assert.Equal(person.Key, loadedPerson.Key);
        }

        [Fact]
        public void DocumentIfMatchRev()
        {
            ClearDatabase();

            var person = InsertAPerson();

            var personInfo = db.FindDocumentInfo(person);

            var loadedPerson = db.Document<Person>(person.Key, ifMatchRev: personInfo.Rev);

            Assert.Equal(person.Name, loadedPerson.Name);
            Assert.Equal(person.Key, loadedPerson.Key);
        }

        [Fact]
        public void DocumentIfMatchRevFailed()
        {
            ClearDatabase();

            var person = InsertAPerson();

            var personInfo = db.FindDocumentInfo(person);

            Assert.Throws<ArangoServerException>(() => db.Document<Person>(person.Key, ifMatchRev: $"{personInfo.Rev}1"));
        }

        [Fact]
        public void DocumentIfNotMatchRev()
        {
            ClearDatabase();

            var person = InsertAPerson();

            var personInfo = db.FindDocumentInfo(person);

            var loadedPerson = db.Document<Person>(person.Key, ifNoneMatchRev: personInfo.Rev);

            Assert.Null(loadedPerson);
        }

        [Fact]
        public void DocumentIfNotMatchRevFailed()
        {
            ClearDatabase();

            var person = InsertAPerson();

            var personInfo = db.FindDocumentInfo(person);

            var loadedPerson = db.Document<Person>(person.Key, ifNoneMatchRev: $"{personInfo.Rev}1");

            Assert.Equal(person.Name, loadedPerson.Name);
            Assert.Equal(person.Key, loadedPerson.Key);
        }
        
        [Fact]
        public void Replace()
        {
            ClearDatabase();

            var person = InsertAPerson();

            person.Age = 20;

            db.Replace<Person>(person);

            Assert.Equal(db.Document<Person>(person.Key).Age, 20);
        }

        [Fact]
        public void ReplaceIfMatch()
        {
            ClearDatabase();

            var person = InsertAPerson();

            var personInfo = db.FindDocumentInfo(person);

            person.Age = 20;

            Assert.Throws<ArangoServerException>(() => db.Replace<Person>(person, ifMatchRev: $"{personInfo.Rev}1"));
        }

        [Fact]
        public void ReplaceById()
        {
            ClearDatabase();

            var person = InsertAPerson();

            db.ReplaceById<Person>(person.Key, new { Age = 20 });

            Assert.Null(db.Document<Person>(person.Key).Name);
            Assert.Equal(db.Document<Person>(person.Key).Age, 20);
        }

        [Fact]
        public void ReplaceByIdIfMatchFailed()
        {
            ClearDatabase();

            var person = InsertAPerson();

            var personInfo = db.FindDocumentInfo(person);

            Assert.Throws<ArangoServerException>(() => db.ReplaceById<Person>(person.Key, new { Age = 20 }, ifMatchRev:$"{personInfo.Rev}1"));
        }

        [Fact]
        public void UpdateByIdIfMatchFailed()
        {
            ClearDatabase();

            var person = InsertAPerson();

            var personInfo = db.FindDocumentInfo(person);

            Assert.Throws<ArangoServerException>(() => db.UpdateById<Person>(person.Key, new { Age = 20 }, ifMatchRev: $"{personInfo.Rev}1"));
        }

        [Fact]
        public void UpdateById()
        {
            ClearDatabase();

            var person = InsertAPerson();

            db.UpdateById<Person>(person.Key, new { Age = 20 });

            Assert.Equal(db.Document<Person>(person.Key).Age, 20);
            Assert.Equal(db.Document<Person>(person.Key).Name, person.Name);
        }

        [Fact]
        public void UpdateIfMatchFailed()
        {
            ClearDatabase();

            var person = InsertAPerson();

            person.Age = 20;

            var personInfo = db.FindDocumentInfo(person);

            Assert.Throws<ArangoServerException>(() => db.Update<Person>(person, ifMatchRev: $"{personInfo.Rev}1"));
        }

        [Fact]
        public void Update()
        {
            ClearDatabase();

            var person = InsertAPerson();

            person.Age = 20;

            db.Update<Person>(person);

            Assert.Equal(db.Document<Person>(person.Key).Age, 20);
        }

        [Fact]
        public void RemoveByIdIfMatchFailed()
        {
            ClearDatabase();

            var person = InsertAPerson();

            var personInfo = db.FindDocumentInfo(person);

            Assert.Throws<ArangoServerException>(() => db.RemoveById<Person>(person.Key, ifMatchRev: $"{personInfo.Rev}1"));
        }

        [Fact]
        public void RemoveById()
        {
            ClearDatabase();

            var person = InsertAPerson();

            db.RemoveById<Person>(person.Key);

            Assert.Null(db.Document<Person>(person.Key));
        }
        
        [Fact]
        public void RemoveIfMatchFailed()
        {
            ClearDatabase();

            var person = InsertAPerson();

            var personInfo = db.FindDocumentInfo(person);

            Assert.Throws<ArangoServerException>(() => db.Remove<Person>(person, ifMatchRev: $"{personInfo.Rev}1"));
        }

        [Fact]
        public void Remove()
        {
            ClearDatabase();

            var person = InsertAPerson();

            db.Remove<Person>(person);

            Assert.Null(db.Document<Person>(person.Key));
        }
    }
}
