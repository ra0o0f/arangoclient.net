﻿using ArangoDB.Client.Examples.Models;
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
                Assert.Equal(2, bs.Count);
                Assert.True(bs[0].HasError() == false);
                Assert.True(bs[1].HasError());
            });

            Assert.Equal(2, results.Count);
            Assert.True(string.IsNullOrEmpty(results[0].Key) == false);
            Assert.True(string.IsNullOrEmpty(results[1].Key));
        }

        [Fact]
        public void InsertMultiple()
        {
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
                Assert.Equal(2, bs.Count);
                Assert.True(bs.All(x => x.HasError() == false));
            });

            Assert.Equal(2, results.Count);
            Assert.True(results.All(x => string.IsNullOrEmpty(x.Key) == false));
        }

        [Fact]
        public void InsertFailed()
        {
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
            var person = InsertAPerson();

            Assert.NotNull(person.Key);
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
        public void DocumentNotFound()
        {
            var loadedPerson = db.Document<Person>("none");

            Assert.Null(loadedPerson);
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
        public void Replace()
        {
            var person = InsertAPerson();

            person.Age = 20;

            db.Replace<Person>(person);

            Assert.Equal(20, db.Document<Person>(person.Key).Age);
        }

        [Fact]
        public void ReplaceIfMatch()
        {
            var person = InsertAPerson();

            var personInfo = db.FindDocumentInfo(person);

            person.Age = 20;

            Assert.Throws<ArangoServerException>(() => db.Replace<Person>(person, ifMatchRev: $"{personInfo.Rev}1"));
        }

        [Fact]
        public void ReplaceById()
        {
            var person = InsertAPerson();

            db.ReplaceById<Person>(person.Key, new { Age = 20 });

            Assert.Null(db.Document<Person>(person.Key).Name);
            Assert.Equal(20, db.Document<Person>(person.Key).Age);
        }

        [Fact]
        public void ReplaceByIdIfMatchFailed()
        {
            var person = InsertAPerson();

            var personInfo = db.FindDocumentInfo(person);

            Assert.Throws<ArangoServerException>(() => db.ReplaceById<Person>(person.Key, new { Age = 20 }, ifMatchRev: $"{personInfo.Rev}1"));
        }

        [Fact]
        public void UpdateByIdIfMatchFailed()
        {
            var person = InsertAPerson();

            var personInfo = db.FindDocumentInfo(person);

            Assert.Throws<ArangoServerException>(() => db.UpdateById<Person>(person.Key, new { Age = 20 }, ifMatchRev: $"{personInfo.Rev}1"));
        }

        [Fact]
        public void UpdateById()
        {
            var person = InsertAPerson();

            db.UpdateById<Person>(person.Key, new { Age = 20 });

            Assert.Equal(20, db.Document<Person>(person.Key).Age);
            Assert.Equal(db.Document<Person>(person.Key).Name, person.Name);
        }

        [Fact]
        public void UpdateIfMatchFailed()
        {
            var person = InsertAPerson();

            person.Age = 20;

            var personInfo = db.FindDocumentInfo(person);

            Assert.Throws<ArangoServerException>(() => db.Update<Person>(person, ifMatchRev: $"{personInfo.Rev}1"));
        }

        [Fact]
        public void Update()
        {
            var person = InsertAPerson();

            person.Age = 20;

            db.Update<Person>(person);

            Assert.Equal(20, db.Document<Person>(person.Key).Age);
        }

        [Fact]
        public void RemoveByIdIfMatchFailed()
        {
            var person = InsertAPerson();

            var personInfo = db.FindDocumentInfo(person);

            Assert.Throws<ArangoServerException>(() => db.RemoveById<Person>(person.Key, ifMatchRev: $"{personInfo.Rev}1"));
        }

        [Fact]
        public void RemoveById()
        {
            var person = InsertAPerson();

            db.RemoveById<Person>(person.Key);

            Assert.Null(db.Document<Person>(person.Key));
        }

        [Fact]
        public void RemoveIfMatchFailed()
        {
            var person = InsertAPerson();

            var personInfo = db.FindDocumentInfo(person);

            Assert.Throws<ArangoServerException>(() => db.Remove<Person>(person, ifMatchRev: $"{personInfo.Rev}1"));
        }

        [Fact]
        public void Remove()
        {
            var person = InsertAPerson();

            db.Remove<Person>(person);

            Assert.Null(db.Document<Person>(person.Key));
        }

        [Fact]
        public void InsertEdge()
        {
            var person1 = new Person
            {
                Age = 22,
                Name = "A"
            };

            var person2 = new Person
            {
                Age = 25,
                Name = "B"
            };

            db.InsertMultiple<Person>(new Person[] { person1, person2 });

            var follow = new Follow
            {
                CreatedDate = DateTime.Now,
                Follower = person1.Id,
                Followee = person2.Id
            };

            db.Insert<Follow>(follow);

            Assert.NotNull(follow.Key);
        }

        [Fact]
        public void ReadEdges()
        {
            var person1 = new Person
            {
                Age = 22,
                Name = "A"
            };

            var person2 = new Person
            {
                Age = 25,
                Name = "B"
            };

            db.InsertMultiple<Person>(new Person[] { person1, person2 });

            var follow = new Follow
            {
                CreatedDate = DateTime.Now,
                Follower = person1.Id,
                Followee = person2.Id
            };

            db.Insert<Follow>(follow);

            var outboundEdge = db.Edges<Follow>(person1.Id, direction: EdgeDirection.Outbound);

            Assert.Single(outboundEdge);

            var inboundEdge = db.Edges<Follow>(person1.Id, direction: EdgeDirection.Inbound);

            Assert.Empty(inboundEdge);

            var anyEdge = db.Edges<Follow>(person2.Id);

            Assert.Single(anyEdge);

            Assert.Equal(anyEdge[0].Key, follow.Key);
        }
    }
}
