using ArangoDB.Client.Data;
using ArangoDB.Client.Http;
using ArangoDB.Client.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public partial class ArangoDatabase
    {
        /// <summary>
        /// Creates a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="edgeDefinitions">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="orphanCollection">Whether or not the collection will be compacted</param>
        /// <returns>CreateGraphResult</returns>
        public CreateGraphResult CreateGraph(string name, List<EdgeDefinitionData> edgeDefinitions, List<string> orphanCollections = null)
        {
            return CreateGraphAsync(name, edgeDefinitions, orphanCollections).ResultSynchronizer();
        }

        /// <summary>
        /// Creates a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="edgeDefinitions">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="orphanCollection">Whether or not the collection will be compacted</param>
        /// <returns>CreateGraphResult</returns>
        public async Task<CreateGraphResult> CreateGraphAsync(string name, List<EdgeDefinitionData> edgeDefinitions, List<string> orphanCollections = null)
        {
            var command = new HttpCommand(this)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Post
            };

            var data = new GraphCollectionData
            {
                Name = name,
                EdgeDefinitions = edgeDefinitions,
                OrphanCollections = orphanCollections
            };

            var result = await command.RequestMergedResult<CreateGraphResult>(data).ConfigureAwait(false);

            return result.Result;
        }

        /// <summary>
        /// Deletes a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="dropCollections">Drop collections of this graph as well. Collections will only be dropped if they are not used in other graphs.</param>
        /// <returns></returns>
        public void DeleteGraph(string name, bool dropCollections = false)
        {
            DeleteGraphAsync(name, dropCollections).WaitSynchronizer();
        }

        /// <summary>
        /// Deletes a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="dropCollections">Drop collections of this graph as well. Collections will only be dropped if they are not used in other graphs.</param>
        /// <returns>Task</returns>
        public async Task DeleteGraphAsync(string name, bool dropCollections = false)
        {
            var command = new HttpCommand(this)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Delete,
                Command = name
            };

            var data = new GraphCollectionData
            {
                Name = name,
                DropCollections = dropCollections
            };

            var result = await command.RequestGenericSingleResult<bool, InheritedCommandResult<bool>>(data).ConfigureAwait(false);
        }

        /// <summary>
        /// Get a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <returns>GraphIdentifierResult</returns>
        public GraphIdentifierResult GetGraph(string name)
        {
            return GetGraphAsync(name).ResultSynchronizer();
        }

        /// <summary>
        /// Deletes a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <returns>GraphIdentifierResult</returns>
        public async Task<GraphIdentifierResult> GetGraphAsync(string name)
        {
            var command = new HttpCommand(this)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Get,
                Command = name
            };

            var result = await command.RequestMergedResult<CreateGraphResult>().ConfigureAwait(false);

            return result.Result.Graph;
        }
    }
}
