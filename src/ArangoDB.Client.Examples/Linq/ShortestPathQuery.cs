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
    public class ShortestPathQuery : TestDatabaseSetup
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
        public void ShortestPath()
        {
            InitiateGraph();

            var result = db.Query()
                .ShortestPath<Person, Follow>(alice.Id, eve.Id)
                .OutBound()
                .Graph(graphName)
                .Select(g => g)
                .ToList();

            Assert.Equal(result.Count, 4);
            Assert.Equal(result[0].Vertex.Key, alice.Key);
            Assert.Equal(result[1].Vertex.Key, bob.Key);
            Assert.Equal(result[2].Vertex.Key, charlie.Key);
            Assert.Equal(result[3].Vertex.Key, eve.Key);

            Assert.Null(result[0].Edge);
            Assert.Equal(result[1].Edge.Key, aliceBob.Key);
            Assert.Equal(result[2].Edge.Key, bobCharlie.Key);
            Assert.Equal(result[3].Edge.Key, charlieEve.Key);
        }

        [Fact]
        public void ShortestPathInbound()
        {
            InitiateGraph();

            var result = db.Query()
                .ShortestPath<Person, Follow>(eve.Id, alice.Id)
                .InBound()
                .Graph(graphName)
                .Select(g => g)
                .ToList();

            Assert.Equal(result.Count, 4);
        }

        [Fact]
        public void ShortestPathInvalidGraphName()
        {
            InitiateGraph();

            var query = db.Query()
                .ShortestPath<Person, Follow>(eve.Id, alice.Id)
                .InBound()
                .Graph("InvalidGraphName")
                .Select(g => g);

            Assert.Throws<ArangoServerException>(() => query.ToList());
        }

        [Fact]
        public void ShortestPathSelectVertices()
        {
            InitiateGraph();

            var result = db.Query()
                .ShortestPath<Person, Follow>(eve.Id, alice.Id)
                .Graph(graphName)
                .Select(g => g.Vertex.Key)
                .ToList();

            Assert.Equal(result.Count, 4);
            Assert.True(result.All(x => string.IsNullOrEmpty(x) == false));
        }

        [Fact]
        public void ShortestPathInEdges()
        {
            InitiateGraph();

            var result = db.Query()
                .ShortestPath<Person, Follow>(eve.Id, alice.Id)
                .InBound()
                .Edge(db.NameOf<Follow>())
                .Select(g => g)
                .ToList();

            Assert.Equal(result.Count, 4);
        }


        [Fact]
        public void ShortestPathInEdgesInBound()
        {
            InitiateGraph();

            var result = db.Query()
                .ShortestPath<Person, Follow>(eve.Id, alice.Id)
                .OutBound()
                .Edge(db.NameOf<Follow>(), EdgeDirection.Inbound)
                .Select(g => g)
                .ToList();

            Assert.Equal(result.Count, 4);
        }
    }
}
