﻿using ArangoDB.Client.Examples.Models;
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
                new Person { Name = "hojat",Age=27 }
            };

            db.InsertMultiple<Person>(persons);

            return persons;
        }

        [Fact]
        public void All()
        {
            var persons = InsertSomePerson();

            var cursor = db.All<Person>();

            var loadedPersons = cursor.ToList();

            Assert.Equal(1, cursor.Statistics.RequestCount);

            Assert.Equal(3, loadedPersons.Count);

            Assert.Empty(loadedPersons
                .Select(p => p.Key)
                .Except(persons.Select(p => p.Key)));
        }

        [Fact]
        public void AllWithBatch()
        {
            var persons = InsertSomePerson();

            var cursor = db.All<Person>(batchSize: 1);

            var loadedPersons = cursor.ToList();

            Assert.Equal(3, cursor.Statistics.RequestCount);

            Assert.Equal(3, loadedPersons.Count);

            Assert.Empty(loadedPersons
                .Select(p => p.Key)
                .Except(persons.Select(p => p.Key)));
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

            Assert.Single(loadedPersons);
            Assert.Equal(loadedPersons[0].Key, persons.First(p => p.Name == "Dr. frank celler").Key);
        }

        [Fact]
        public void FirstExample()
        {
            var persons = InsertSomePerson();

            var loadedPerson = db.FirstExample<Person>(new { Name = "Dr. frank celler" });

            Assert.NotNull(loadedPerson);
            Assert.Equal(loadedPerson.Key, persons.First(p => p.Name == "Dr. frank celler").Key);
        }

        [Fact]
        public void Range()
        {
            var persons = InsertSomePerson();

            var loadedPerson = db.Range<Person>(x => x.Age, 25, 28, closed: true).ToList();

            Assert.Equal(2, loadedPerson.Count);
        }
    }
}
