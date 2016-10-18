using ArangoDB.Client.Data;
using ArangoDB.Client.Examples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArangoDB.Client.Examples.Linq
{
    public class TraversalQuery : TestDatabaseSetup
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

        Follow aliceBob;
        Follow bobCharlie;
        Follow bobDave;
        Follow charlieEve;

        string graphName = "SocialGraph";

        private void InitiateGraph()
        {
            db.InsertMultiple<Person>(new Person[]
            {
                alice, bob, charlie, dave, eve
            });

            aliceBob = new Follow
            {
                Follower = alice.Id,
                Followee = bob.Id,
                Label = nameof(aliceBob)
            };
            bobCharlie = new Follow
            {
                Follower = bob.Id,
                Followee = charlie.Id,
                Label = nameof(bobCharlie)
            };
            bobDave = new Follow
            {
                Follower = bob.Id,
                Followee = dave.Id,
                Label = nameof(bobDave)
            };
            charlieEve = new Follow
            {
                Follower = charlie.Id,
                Followee = eve.Id,
                Label = nameof(charlieEve)
            };

            db.InsertMultiple<Follow>(new Follow[]
            {
                aliceBob, bobCharlie, bobDave, charlieEve
            });

            db.Graph(graphName).Create(new EdgeDefinitionTypedData[]
            {
                new EdgeDefinitionTypedData
                {
                    Collection = typeof(Follow),
                    From = new Type[] { typeof(Person) },
                    To = new Type[] { typeof(Person) }
                }
            });
        }

        [Fact]
        public void Traversal()
        {
            InitiateGraph();

            var result = db.Query()
                .Traversal<Person, Follow>(alice.Id)
                .Depth(1, 4)
                .OutBound()
                .Graph(graphName)
                .Select(g => g)
                .ToList();
            
        }
    }
}
