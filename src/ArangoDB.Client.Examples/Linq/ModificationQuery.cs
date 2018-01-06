using ArangoDB.Client.Examples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArangoDB.Client.Examples.Linq
{
    public class ModificationQuery : TestDatabaseSetup
    {
        Person alice = new Person
        {
            Name = "Alice",
            Age = 21
        };

        Person bob = new Person
        {
            Name = "Bob",
            Age = 22
        };

        Person charlie = new Person
        {
            Name = "Charlie",
            Age = 23
        };

        Person dave = new Person
        {
            Name = "Dave",
            Age = 24
        };

        Person eve = new Person
        {
            Name = "Eve",
            Age = 24
        };

        private void InsertSomePerson()
        {
            db.InsertMultiple<Person>(new Person[]
            {
                alice, bob, charlie, dave, eve
            });
        }

        [Fact]
        public void UpdateSingle()
        {
            InsertSomePerson();

            db.Query().Update(_ => new { Age = 25 }, _ => eve.Key).In<Person>().Execute();

            Assert.Equal(25, db.Document<Person>(eve.Key).Age);
        }

        [Fact]
        public void UpdateSingle_SelectOldNew()
        {
            InsertSomePerson();

            var result = db.Query()
                .Update(_ => new { Age = 25 }, _ => eve.Key)
                .In<Person>()
                .Select((n, o) => new { o, n })
                .First();

            Assert.Equal(25, result.n.Age);
            Assert.Equal(24, result.o.Age);
        }

        [Fact]
        public void Update()
        {
            InsertSomePerson();

            db.Query<Person>()
                .Update(p => new { Age = p.Age + 10 })
                .Execute();

            var loadedBob = db.Document<Person>(bob.Key);
            Assert.NotNull(loadedBob.Name);
            Assert.Equal(bob.Age + 10, loadedBob.Age);

            var loadedCharlie = db.Document<Person>(charlie.Key);
            Assert.NotNull(loadedCharlie.Name);
            Assert.Equal(charlie.Age + 10, loadedCharlie.Age);
        }

        [Fact]
        public void Update_SelectOldNew()
        {
            InsertSomePerson();

            var result = db.Query<Person>()
                .Update(p => new { Age = p.Age + 10 })
                .Select((n, o) => new { o, n })
                .ToList();

            foreach (var r in result)
                Assert.Equal(r.o.Age + 10, r.n.Age);
        }

        [Fact]
        public void Update_WithKey()
        {
            InsertSomePerson();

            var result = db.Query<Person>()
                .Update(p => new { Age = p.Age + 10 }, p => p.Key)
                .Select((n, o) => new { o, n })
                .ToList();

            foreach (var r in result)
                Assert.Equal(r.o.Age + 10, r.n.Age);
        }

        [Fact]
        public void ReplaceSingle()
        {
            InsertSomePerson();

            db.Query().Replace(_ => new Person { Age = 25 }, _ => eve.Key).In<Person>().Execute();

            Assert.Equal(25, db.Document<Person>(eve.Key).Age);
        }

        [Fact]
        public void ReplaceSingle_SelectOldNew()
        {
            InsertSomePerson();

            var result = db.Query()
                .Replace(_ => new Person { Age = 25 }, _ => eve.Key)
                .In<Person>()
                .Select((n, o) => new { o, n })
                .First();

            Assert.Equal(25, result.n.Age);
            Assert.Equal(24, result.o.Age);
        }

        [Fact]
        public void Replace()
        {
            InsertSomePerson();

            db.Query<Person>()
                .Replace(p => new Person { Age = p.Age + 10 })
                .Execute();

            var loadedBob = db.Document<Person>(bob.Key);
            Assert.Null(loadedBob.Name);
            Assert.Equal(bob.Age + 10, loadedBob.Age);

            var loadedCharlie = db.Document<Person>(charlie.Key);
            Assert.Null(loadedCharlie.Name);
            Assert.Equal(charlie.Age + 10, loadedCharlie.Age);
        }

        [Fact]
        public void Replace_SelectOldNew()
        {
            InsertSomePerson();

            var result = db.Query<Person>()
                .Replace(p => new Person { Age = p.Age + 10 })
                .Select((n, o) => new { o, n })
                .ToList();

            foreach (var r in result)
                Assert.Equal(r.o.Age + 10, r.n.Age);
        }

        [Fact]
        public void Replace_WithKey()
        {
            InsertSomePerson();

            var result = db.Query<Person>()
                .Replace(p => new Person { Age = p.Age + 10 }, p => p.Key)
                .Select((n, o) => new { o, n })
                .ToList();

            foreach (var r in result)
                Assert.Equal(r.o.Age + 10, r.n.Age);
        }

        [Fact]
        public void RemoveSingle()
        {
            InsertSomePerson();

            db.Query().Remove(_ => alice.Key).In<Person>().Execute();

            Assert.Equal(4, db.Query<Person>().Count());
            Assert.Null(db.Query<Person>().FirstOrDefault(p => p.Key == alice.Key));
        }

        [Fact]
        public void RemoveSingle_SelectOldMember()
        {
            InsertSomePerson();

            var result = db.Query().Remove(_ => alice.Key).In<Person>().Select((n, o) => o.Age).ToList();

            Assert.Equal(4, db.Query<Person>().Count());
            Assert.Equal(result[0], alice.Age);
        }

        [Fact]
        public void Remove_All()
        {
            InsertSomePerson();

            db.Query<Person>().Remove().Execute();

            Assert.Equal(0, db.Query<Person>().Count());
        }

        [Fact]
        public void Remove_All_SelectOldKey()
        {
            InsertSomePerson();

            var result = db.Query<Person>()
                .Remove()
                .Select((n, o) => o.Key)
                .ToList();

            Assert.Equal(5, result.Count);
            Assert.True(result.All(k => string.IsNullOrEmpty(k) == false));
            Assert.Empty(result.Except(new string[]
            {
                alice.Key, bob.Key, charlie.Key, dave.Key, eve.Key
            }));
            Assert.Equal(0, db.Query<Person>().Count());
        }

        [Fact]
        public void Remove_A_Preson()
        {
            InsertSomePerson();

            db.Query<Person>()
                .Where(x => x.Key == bob.Key)
                .Remove(x => x.Key)
                .Execute();

            Assert.Equal(4, db.Query<Person>().Count());
            Assert.Null(db.Query<Person>().FirstOrDefault(x => x.Key == bob.Key));
        }

        [Fact]
        public void Remove_A_Preson_SelectAge()
        {
            InsertSomePerson();

            var age = db.Query<Person>()
                .Where(x => x.Key == bob.Key)
                .Remove(x => x.Key)
                .Select((n, o) => o.Age)
                .First();

            Assert.Equal(4, db.Query<Person>().Count());
            Assert.Equal(age, bob.Age);
        }

        [Fact]
        public void InsertSingle()
        {
            var person = new Person
            {
                Age = 22
            };

            db.Query().Insert(_ => person).In<Person>().Execute();

            Assert.Equal(1, db.Query<Person>().Count());
            Assert.NotNull(db.Query<Person>().FirstOrDefault(p => p.Age == person.Age));
        }

        [Fact]
        public void InsertSingle_SelectNewMember()
        {
            var person = new Person
            {
                Age = 22
            };

            var result = db.Query().Insert(_ => person).In<Person>().Select((n, o) => n).ToList();

            Assert.NotNull(result[0].Key);
            Assert.Equal(result[0].Age, person.Age);
        }

        [Fact]
        public void Insert_All()
        {
            InsertSomePerson();

            db.Query<Person>().Insert().In<Host>().Execute();

            Assert.Equal(db.Query<Person>().Count(), db.Query<Host>().Count());
        }

        [Fact]
        public void Insert_All_SelectNew()
        {
            InsertSomePerson();

            var result = db.Query<Person>().Insert().In<Host>().Select((n, o) => n).ToList();

            Assert.Equal(result.Count(), db.Query<Host>().Count());
        }

        [Fact]
        public void Upsert()
        {
            var query = db.Query()
                .Upsert<Host>(_ => new Host { Ip = "192.168.173.94" },
                                  _ => new Host { Ip = "192.168.173.94", Name = "chorweiler", Tags = new string[] { "development" } },
                                 (_, o) => new Host { Tags = AQL.Push(o.Tags, "production", true) })
                                 .In<Host>();

            // execute query for the first time
            var firstResult = query.Select((n, o) => n).ToList();

            Assert.Single(firstResult);
            Assert.Equal(1, firstResult[0].Tags.Count);
            Assert.Equal("development", firstResult[0].Tags[0]);

            // execute query for the second time
            var secondResult = query.Select((n, o) => n).ToList();

            Assert.Single(secondResult);
            Assert.Equal(2, secondResult[0].Tags.Count);
            Assert.Equal("development", secondResult[0].Tags[0]);
            Assert.Equal("production", secondResult[0].Tags[1]);
        }

        [Fact]
        public void UpsertOnCollection()
        {
            // insert a host
            db.Insert<Host>(new Host { Key = "123", Tags = new string[] { "1", "2", "3" } });

            var result = db.Query<Host>()
                .Upsert(h => new Host { Key = h.Key },
                h => new Host { },
                (h, old) => new Host { Tags = AQL.Append(h.Tags, old.Tags) }).Select((n, o) => n).ToList();

            Assert.Single(result);
            Assert.Equal(6, result[0].Tags.Count);
            Assert.Equal(2, result[0].Tags.Count(t => t == "2"));
        }
    }
}
