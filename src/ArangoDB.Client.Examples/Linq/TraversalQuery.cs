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

            Assert.Equal(result.Count, 4);
            Assert.Equal(result[0].Vertex.Key, bob.Key);
            Assert.Equal(result[1].Vertex.Key, charlie.Key);
            Assert.Equal(result[2].Vertex.Key, eve.Key);
            Assert.Equal(result[3].Vertex.Key, dave.Key);

            Assert.Equal(result[0].Edge.Key, aliceBob.Key);
            Assert.Equal(result[1].Edge.Key, bobCharlie.Key);
            Assert.Equal(result[2].Edge.Key, charlieEve.Key);
            Assert.Equal(result[3].Edge.Key, bobDave.Key);

            Assert.Equal(result[0].Path.Vertices.Count, 2);
            Assert.Equal(result[0].Path.Edges.Count, 1);

            Assert.Equal(result[0].Path.Vertices[1].Key, bob.Key);
            Assert.Equal(result[0].Path.Edges[0].Key, aliceBob.Key);

            Assert.Equal(result[2].Path.Vertices.Count, 4);
            Assert.Equal(result[2].Path.Edges.Count, 3);

            Assert.Equal(result[2].Path.Vertices[2].Key, charlie.Key);
            Assert.Equal(result[2].Path.Edges[1].Key, bobCharlie.Key);
        }

        [Fact]
        public void TraversalDepth()
        {
            InitiateGraph();

            var result = db.Query()
                .Traversal<Person, Follow>(alice.Id)
                .Depth(1, 2)
                .OutBound()
                .Graph(graphName)
                .Select(g => g)
                .ToList();

            Assert.Equal(result.Count, 3);
        }

        [Fact]
        public void TraversalWithoutDepth()
        {
            InitiateGraph();

            var result = db.Query()
                .Traversal<Person, Follow>(alice.Id)
                .OutBound()
                .Graph(graphName)
                .Select(g => g)
                .ToList();

            Assert.Equal(result.Count, 1);
        }

        [Fact]
        public void TraversalInbound()
        {
            InitiateGraph();

            var result = db.Query()
                .Traversal<Person, Follow>(eve.Id)
                .Depth(1, 5)
                .InBound()
                .Graph(graphName)
                .Select(g => g)
                .ToList();

            Assert.Equal(result.Count, 3);
        }

        [Fact]
        public void TraversalInvalidGraphName()
        {
            InitiateGraph();

            var query = db.Query()
                .Traversal<Person, Follow>(eve.Id)
                .Depth(1, 5)
                .InBound()
                .Graph("InvalidGraphName")
                .Select(g => g);

            Assert.Throws<ArangoServerException>(() => query.ToList());
        }

        [Fact]
        public void TraversalSelectVertices()
        {
            InitiateGraph();

            var result = db.Query()
                .Traversal<Person, Follow>(alice.Id)
                .Depth(1, 6)
                .Graph(graphName)
                .Select(g => g.Vertex.Key)
                .ToList();

            Assert.Equal(result.Count, 4);
            Assert.True(result.All(x => string.IsNullOrEmpty(x) == false));
        }

        [Fact]
        public void TraversalFilter()
        {
            InitiateGraph();

            var result = db.Query()
                .Traversal<Person, Follow>(alice.Id)
                .Depth(1, 6)
                .Graph(graphName)
                .Where(g => AQL.Length(
                    db.Query().For(_ => g.Path.Vertices)
                    .Where(v => v.Id == charlie.Id)
                    .Select(v => v)) 
                    == 0)
                .Select(g => g.Vertex)
                .ToList();

            Assert.Equal(result.Count, 2);
        }
    }
}
