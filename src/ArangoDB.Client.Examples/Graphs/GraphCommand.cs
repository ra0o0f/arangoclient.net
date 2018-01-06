using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client.Data;
using ArangoDB.Client.Examples.Models;
using Xunit;

namespace ArangoDB.Client.Examples.Graphs
{
    public class GraphCommand : TestDatabaseSetup
    {
        IArangoGraph Graph()
        {
            return db.Graph("SocialGraph");
        }

        GraphIdentifierResult CreateNewGraph()
        {
            var graph = Graph();

            return graph.Create(new List<EdgeDefinitionTypedData>
            {
                new EdgeDefinitionTypedData
                {
                    Collection = typeof(Follow),
                    From = new List<Type> { typeof(Person) },
                    To = new List<Type> { typeof(Person) }
                }
            });
        }

        // edge examples

        [Fact]
        public void RemoveEdgeIfMatchFailed()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var v1 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var v2 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var follow = new Follow
            {
                Followee = v1.Id,
                Follower = v2.Id,
                CreatedDate = DateTime.Now
            };

            var inserted = graph.InsertEdge<Follow>(follow);

            Assert.Throws<ArangoServerException>(() => graph.RemoveEdge<Follow>(follow, ifMatchRev: $"{inserted.Rev}0"));
        }

        [Fact]
        public void RemoveEdge()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var v1 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var v2 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var follow = new Follow
            {
                Followee = v1.Id,
                Follower = v2.Id,
                CreatedDate = DateTime.Now
            };

            var inserted = graph.InsertEdge<Follow>(follow);

            graph.RemoveEdge<Follow>(follow, ifMatchRev: inserted.Rev);

            var removed = graph.GetEdge<Follow>(inserted.Key);

            Assert.Null(removed);
        }

        [Fact]
        public void RemoveEdgeById()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var v1 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var v2 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var follow = new Follow
            {
                Followee = v1.Id,
                Follower = v2.Id,
                CreatedDate = DateTime.Now
            };

            var inserted = graph.InsertEdge<Follow>(follow);

            graph.RemoveEdgeById<Follow>(inserted.Key);

            var removed = graph.GetEdge<Follow>(inserted.Key);

            Assert.Null(removed);
        }

        [Fact]
        public void ReplaceEdgeIfMatchFailed()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var v1 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var v2 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var follow = new Follow
            {
                Followee = v1.Id,
                Follower = v2.Id,
                CreatedDate = DateTime.Now
            };

            var inserted = graph.InsertEdge<Follow>(follow);

            Assert.Throws<ArangoServerException>(() => graph.ReplaceEdge<Follow>(follow, ifMatchRev: $"{inserted.Rev}0"));
        }

        [Fact]
        public void ReplaceEdge()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var v1 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var v2 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var follow = new Follow
            {
                Followee = v1.Id,
                Follower = v2.Id,
                CreatedDate = DateTime.Now
            };

            var inserted = graph.InsertEdge<Follow>(follow);

            follow.CreatedDate = new DateTime(1900, 1, 1);

            graph.ReplaceEdge<Follow>(follow, ifMatchRev: inserted.Rev);

            var replaced = graph.GetEdge<Follow>(inserted.Key);

            Assert.Equal(1900, replaced.CreatedDate.Year);
        }

        [Fact]
        public void ReplaceEdgeById()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var v1 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var v2 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var follow = new Follow
            {
                Followee = v1.Id,
                Follower = v2.Id,
                CreatedDate = DateTime.Now
            };

            var inserted = graph.InsertEdge<Follow>(follow);

            graph.ReplaceEdgeById<Follow>(inserted.Key, new
            {
                _from = v1.Id,
                _to = v2.Id,
                CreatedDate = new DateTime(1900, 1, 1)
            });

            var replaced = graph.GetEdge<Follow>(inserted.Key);

            Assert.Equal(1900, replaced.CreatedDate.Year);
        }

        [Fact]
        public void UpdateEdgeIfMatchFailed()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var v1 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var v2 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var follow = new Follow
            {
                Followee = v1.Id,
                Follower = v2.Id,
                CreatedDate = DateTime.Now
            };

            var inserted = graph.InsertEdge<Follow>(follow);

            follow.CreatedDate = new DateTime(1900, 1, 1);

            Assert.Throws<ArangoServerException>(() => graph.UpdateEdge<Follow>(follow, ifMatchRev: $"{inserted.Rev}0"));
        }

        [Fact]
        public void UpdateEdge()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var v1 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var v2 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var follow = new Follow
            {
                Followee = v1.Id,
                Follower = v2.Id,
                CreatedDate = DateTime.Now
            };

            var inserted = graph.InsertEdge<Follow>(follow);

            follow.CreatedDate = new DateTime(1900, 1, 1);

            graph.UpdateEdge<Follow>(follow, ifMatchRev: inserted.Rev);

            var updated = graph.GetEdge<Follow>(inserted.Key);

            Assert.Equal(1900, updated.CreatedDate.Year);
        }

        [Fact]
        public void UpdateEdgeById()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var v1 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var v2 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var inserted = graph.InsertEdge<Follow>(new Follow
            {
                Followee = v1.Id,
                Follower = v2.Id,
                CreatedDate = DateTime.Now
            });

            graph.UpdateEdgeById<Follow>(inserted.Key, new { CreatedDate = new DateTime(1900, 1, 1) });

            var updated = graph.GetEdge<Follow>(inserted.Key);

            Assert.Equal(1900, updated.CreatedDate.Year);
        }

        [Fact]
        public void InsertEdge()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var v1 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var v2 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var inserted = graph.InsertEdge<Follow>(new Follow
            {
                Followee = v1.Id,
                Follower = v2.Id
            });

            Assert.NotNull(inserted.Key);
        }

        [Fact]
        public void GetEdge()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var v1 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var v2 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var inserted = graph.InsertEdge<Follow>(new Follow
            {
                Followee = v1.Id,
                Follower = v2.Id
            });

            var result = graph.GetEdge<Follow>(inserted.Key);

            Assert.NotNull(result);
            Assert.NotNull(result.Key);
        }

        [Fact]
        public void GetEdgeNotFound()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var result = graph.GetEdge<Follow>("none");

            Assert.Null(result);
        }

        [Fact]
        public void GetEdgeIfMatchFailed()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var v1 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var v2 = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var inserted = graph.InsertEdge<Follow>(new Follow
            {
                Followee = v1.Id,
                Follower = v2.Id
            });

            var edgeInfo = db.FindDocumentInfo(inserted.Id);

            Assert.NotNull(edgeInfo);
            Assert.NotNull(edgeInfo.Rev);

            Assert.Throws<ArangoServerException>(() => graph.GetEdge<Follow>(inserted.Key, ifMatchRev: $"{edgeInfo.Rev}0"));
        }

        // vertex examples

        [Fact]
        public void RemoveVertexIfMatchFailed()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var person = new Person
            {
                Age = 21,
                Name = "raoof hojat"
            };

            var inserted = graph.InsertVertex<Person>(person);

            Assert.Throws<ArangoServerException>(() => graph.RemoveVertex<Person>(person, ifMatchRev: $"{inserted.Rev}0"));
        }

        [Fact]
        public void RemoveVertex()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var person = new Person
            {
                Age = 21,
                Name = "raoof hojat"
            };

            var inserted = graph.InsertVertex<Person>(person);

            graph.RemoveVertex<Person>(person, ifMatchRev: inserted.Rev);

            var removed = graph.GetVertex<Person>(inserted.Key);

            Assert.Null(removed);
        }

        [Fact]
        public void RemoveVertexById()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var inserted = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            graph.RemoveVertexById<Person>(inserted.Key);

            var removed = graph.GetVertex<Person>(inserted.Key);

            Assert.Null(removed);
        }

        [Fact]
        public void ReplaceVertexIfMatchFailed()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var person = new Person
            {
                Age = 21,
                Name = "raoof hojat"
            };

            var inserted = graph.InsertVertex<Person>(person);

            person.Age = 33;

            Assert.Throws<ArangoServerException>(() => graph.ReplaceVertex<Person>(person, ifMatchRev: $"{inserted.Rev}0"));
        }

        [Fact]
        public void ReplaceVertex()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var person = new Person
            {
                Age = 21,
                Name = "raoof hojat"
            };

            var inserted = graph.InsertVertex<Person>(person);

            person.Age = 33;

            graph.ReplaceVertex<Person>(person, ifMatchRev: inserted.Rev);

            var replaced = graph.GetVertex<Person>(inserted.Key);

            Assert.Equal(33, replaced.Age);
        }

        [Fact]
        public void ReplaceVertexById()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var inserted = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            graph.ReplaceVertexById<Person>(inserted.Key, new { Age = 22 });

            var replaced = graph.GetVertex<Person>(inserted.Key);

            Assert.Null(replaced.Name);
            Assert.Equal(22, replaced.Age);
        }

        [Fact]
        public void UpdateVertexIfMatchFailed()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var person = new Person
            {
                Age = 21,
                Name = "raoof hojat"
            };

            var inserted = graph.InsertVertex<Person>(person);

            person.Age = 33;

            Assert.Throws<ArangoServerException>(() => graph.UpdateVertex<Person>(person, ifMatchRev: $"{inserted.Rev}0"));
        }

        [Fact]
        public void UpdateVertex()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var person = new Person
            {
                Age = 21,
                Name = "raoof hojat"
            };

            var inserted = graph.InsertVertex<Person>(person);

            person.Age = 33;

            graph.UpdateVertex<Person>(person, ifMatchRev: inserted.Rev);

            var updated = graph.GetVertex<Person>(inserted.Key);

            Assert.Equal(33, updated.Age);
        }

        [Fact]
        public void UpdateVertexById()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var inserted = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            graph.UpdateVertexById<Person>(inserted.Key, new { Age = 22 });

            var updated = graph.GetVertex<Person>(inserted.Key);

            Assert.Equal(22, updated.Age);
        }

        [Fact]
        public void InsertVertex()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var result = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            Assert.NotNull(result.Key);
        }

        [Fact]
        public void GetVertex()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var inserted = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var result = graph.GetVertex<Person>(inserted.Key);

            Assert.NotNull(result);
            Assert.NotNull(result.Key);
        }

        [Fact]
        public void GetVertexNotFound()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var result = graph.GetVertex<Person>("none");

            Assert.Null(result);
        }

        [Fact]
        public void GetVertexIfMatchFailed()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var inserted = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var vertexInfo = db.FindDocumentInfo(inserted.Id);

            Assert.NotNull(vertexInfo);
            Assert.NotNull(vertexInfo.Rev);

            Assert.Throws<ArangoServerException>(() => graph.GetVertex<Person>(inserted.Key, ifMatchRev: $"{vertexInfo.Rev}0"));
        }

        // management examples

        [Fact]
        public void ListEdgeDefinitions()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var list = graph.ListEdgeDefinitions();

            Assert.Single(list);
            Assert.Equal(list[0], db.SharedSetting.Collection.ResolveCollectionName<Follow>());
        }

        [Fact]
        public void ExtendEdgeDefinitions()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var result = graph.Edge("Relation").ExtendDefinitions(
                new string[] { db.SharedSetting.Collection.ResolveCollectionName<Host>() },
                new string[] { db.SharedSetting.Collection.ResolveCollectionName<Host>() });

            Assert.Equal(2, result.EdgeDefinitions.Count);
        }

        [Fact]
        public void EditEdgeDefinition()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var result = graph.EditEdgeDefinition<Follow, Follow>(new List<Type> { typeof(Host) }, new List<Type> { typeof(Host) });

            Assert.Single(result.EdgeDefinitions);
            Assert.Equal(result.EdgeDefinitions[0].From[0], db.SharedSetting.Collection.ResolveCollectionName<Host>());
        }

        [Fact]
        public void DeleteEdgeDefinition()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var result = graph.DeleteEdgeDefinition<Follow>();

            Assert.Empty(result.EdgeDefinitions);
        }

        [Fact]
        public void ListVertexCollections()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            graph.AddVertexCollection<Host>();

            var result = graph.ListVertexCollections();

            Assert.Equal(2, result.Count);
            Assert.Empty(result.Except(new string[] {
                db.SharedSetting.Collection.ResolveCollectionName<Host>(),
                db.SharedSetting.Collection.ResolveCollectionName<Person>()
            }));
        }

        [Fact]
        public void AddVertexCollection()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var result = graph.AddVertexCollection<Host>();

            Assert.Single(result.OrphanCollections);
        }

        [Fact]
        public void RemoveVertexCollection()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            graph.AddVertexCollection<Host>();

            var result = graph.RemoveVertexCollection<Host>();

            Assert.Empty(result.OrphanCollections);
        }

        [Fact]
        public void Info()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var info = graph.Info();

            Assert.Equal(info.Key, createdGraph.Key);
            Assert.Equal(info.Id, createdGraph.Id);
        }

        [Fact]
        public void ListGraphs()
        {
            var graph = Graph();

            CreateNewGraph();

            var allGraphs = db.ListGraphs();

            Assert.Single(allGraphs);

            Assert.Equal(allGraphs[0].Key, graph.Name);
        }

        [Fact]
        public void Create()
        {
            var graph = Graph();

            var result = CreateNewGraph();

            Assert.NotNull(result.Id);

            Assert.NotNull(result.Rev);

            Assert.Single(result.EdgeDefinitions);
        }

        [Fact]
        public void Drop()
        {
            var graph = Graph();

            CreateNewGraph();

            var dropped = graph.Drop();

            Assert.True(dropped);
        }
    }
}
