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

            graph.RemoveVertexCollection<Host>();
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
