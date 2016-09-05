using ArangoDB.Client.Data;
using ArangoDB.Client.Examples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArangoDB.Client.Examples.Traversal
{
    public class TraversalCommand : TestDatabaseSetup
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

        IArangoGraph Graph()
        {
            return db.Graph("SocialGraph");
        }

        void InitiateGraph()
        {
            db.InsertMultiple<Person>(new Person[]
            {
                alice, bob, charlie, dave, eve
            });

            db.InsertMultiple<Follow>(new Follow[]
            {
                new Follow
                {
                    Follower = alice.Id,
                    Followee = bob.Id
                },
                new Follow
                {
                    Follower = bob.Id,
                    Followee = charlie.Id
                },
                new Follow
                {
                    Follower = bob.Id,
                    Followee = dave.Id
                },
                new Follow
                {
                    Follower = eve.Id,
                    Followee = alice.Id
                },
                new Follow
                {
                    Follower = eve.Id,
                    Followee = bob.Id
                }
            });

            Graph().Create(new EdgeDefinitionTypedData[]
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
        public void OutboundEdges()
        {
            InitiateGraph();

            var result = db.Traverse<Person, Follow>(new TraversalConfig
            {
                StartVertex = alice.Id,
                GraphName = Graph().Name,
                Direction = EdgeDirection.Outbound
            });

            Assert.Equal(result.Visited.Vertices.Count, 4);
            Assert.Equal(result.Visited.Vertices
                .Select(x => x.Id)
                .Except(new string[] { alice.Id, bob.Id, charlie.Id, dave.Id })
                .Count(), 0);
        }
    }
}
