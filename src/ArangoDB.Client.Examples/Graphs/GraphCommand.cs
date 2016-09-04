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

        [Fact]
        public void ListEdgeDefinitions()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var list = graph.ListEdgeDefinitions();

            Assert.Equal(list.Count, 1);
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

            Assert.Equal(result.EdgeDefinitions.Count, 2);
        }

        [Fact]
        public void EditEdgeDefinition()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var result = graph.EditEdgeDefinition<Follow, Follow>(new List<Type> { typeof(Host) }, new List<Type> { typeof(Host) });

            Assert.Equal(result.EdgeDefinitions.Count, 1);
            Assert.Equal(result.EdgeDefinitions[0].From[0], db.SharedSetting.Collection.ResolveCollectionName<Host>());
        }

        [Fact]
        public void DeleteEdgeDefinition()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var result = graph.DeleteEdgeDefinition<Follow>();

            Assert.Equal(result.EdgeDefinitions.Count, 0);
        }

        [Fact]
        public void ListVertexCollections()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            graph.AddVertexCollection<Host>();

            var result = graph.ListVertexCollections();

            Assert.Equal(result.Count, 2);
            Assert.Equal(result.Except(new string[] {
            db.SharedSetting.Collection.ResolveCollectionName<Host>(),
            db.SharedSetting.Collection.ResolveCollectionName<Person>()
            }).Count(), 0);
        }

        [Fact]
        public void AddVertexCollection()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var result = graph.AddVertexCollection<Host>();

            Assert.Equal(result.OrphanCollections.Count, 1);
        }

        [Fact]
        public void RemoveVertexCollection()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            graph.AddVertexCollection<Host>();

            var result = graph.RemoveVertexCollection<Host>();

            Assert.Equal(result.OrphanCollections.Count, 0);
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

            Assert.Equal(allGraphs.Count, 1);

            Assert.Equal(allGraphs[0].Key, graph.Name);
        }

        [Fact]
        public void Create()
        {
            var graph = Graph();

            var result = CreateNewGraph();

            Assert.NotNull(result.Id);

            Assert.NotNull(result.Rev);

            Assert.Equal(result.EdgeDefinitions.Count, 1);
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
