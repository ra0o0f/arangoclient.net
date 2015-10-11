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
    public class ArangoGraph : IArangoGraph
    {
        IArangoDatabase db;

        public ArangoGraph(IArangoDatabase db)
        {
            this.db = db;
        }

        /// <summary>
        /// Creates a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="edgeDefinitions">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="orphanCollection">Whether or not the collection will be compacted</param>
        /// <returns>CreateGraphResult</returns>
        public GraphResult Create(string name, IList<EdgeDefinitionData> edgeDefinitions
            , IList<string> orphanCollections = null, Action<BaseResult> baseResult = null)
        {
            return CreateAsync(name, edgeDefinitions, orphanCollections, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Creates a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="edgeDefinitions">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="orphanCollection">Whether or not the collection will be compacted</param>
        /// <returns>CreateGraphResult</returns>
        public GraphResult Create(string name, IList<EdgeDefinitionTypedData> edgeDefinitions, IList<Type> orphanCollections = null
            , Action<BaseResult> baseResult = null)
        {
            return CreateAsync(name, edgeDefinitions, orphanCollections, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Creates a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="edgeDefinitions">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="orphanCollection">Whether or not the collection will be compacted</param>
        /// <returns>CreateGraphResult</returns>
        public async Task<GraphResult> CreateAsync(string name, IList<EdgeDefinitionData> edgeDefinitions
            , IList<string> orphanCollections = null, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
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

            var result = await command.RequestMergedResult<GraphResult>(data).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result;
        }

        /// <summary>
        /// Creates a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="edgeDefinitions">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="orphanCollection">Whether or not the collection will be compacted</param>
        /// <returns>CreateGraphResult</returns>
        public async Task<GraphResult> CreateAsync(string name, IList<EdgeDefinitionTypedData> edgeDefinitions, IList<Type> orphanCollections = null
            , Action<BaseResult> baseResult = null)
        {
            List<EdgeDefinitionData> graphEdgeDefinitions = edgeDefinitions == null ? null : new List<EdgeDefinitionData>();
            foreach (var e in edgeDefinitions)
            {
                graphEdgeDefinitions.Add(new EdgeDefinitionData
                {
                    Collection = db.SharedSetting.Collection.ResolveCollectionName(e.Collection),
                    From = e.From.Select(f => db.SharedSetting.Collection.ResolveCollectionName(f)).ToList(),
                    To = e.To.Select(t => db.SharedSetting.Collection.ResolveCollectionName(t)).ToList()
                });
            }

            List<string> graphOrphanCollections = orphanCollections?.Select(o => db.SharedSetting.Collection.ResolveCollectionName(o)).ToList();

            return await CreateAsync(name, graphEdgeDefinitions, graphOrphanCollections, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="dropCollections">Drop collections of this graph as well. Collections will only be dropped if they are not used in other graphs.</param>
        /// <returns></returns>
        public DropGraphResult Drop(string name, bool dropCollections = false, Action<BaseResult> baseResult = null)
        {
            return DropAsync(name, dropCollections, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Deletes a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="dropCollections">Drop collections of this graph as well. Collections will only be dropped if they are not used in other graphs.</param>
        /// <returns>Task</returns>
        public async Task<DropGraphResult> DropAsync(string name, bool dropCollections = false, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Delete,
                Command = name
            };

            var data = new GraphCollectionData
            {
                DropCollections = dropCollections
            };

            var result = await command.RequestMergedResult<DropGraphResult>(data).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result;
        }

        /// <summary>
        /// Get a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <returns>GraphIdentifierResult</returns>
        public GraphIdentifierResult Info(string name, Action<BaseResult> baseResult = null)
        {
            return InfoAsync(name, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Deletes a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <returns>GraphIdentifierResult</returns>
        public async Task<GraphIdentifierResult> InfoAsync(string name, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Get,
                Command = name
            };

            var result = await command.RequestMergedResult<GraphResult>().ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Graph;
        }

        /// <summary>
        /// Lists all graphs
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns>List<GraphIdentifierResult></returns>
        public List<GraphIdentifierResult> List(Action<BaseResult> baseResult = null)
        {
            return ListAsync(baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Lists all graphs
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns>List<GraphIdentifierResult></returns>
        public async Task<List<GraphIdentifierResult>> ListAsync(Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Get
            };

            var result = await command.RequestMergedResult<GraphListResult>().ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Graphs;
        }

        /// <summary>
        /// Lists all vertex collections used in graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public List<string> ListVertexCollections(string graphName, Action<BaseResult> baseResult = null)
        {
            return ListVertexCollectionsAsync(graphName, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Lists all vertex collections used in graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<List<string>> ListVertexCollectionsAsync(string graphName, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Get,
                Command = $"{graphName}/vertex"
            };

            var result = await command.RequestMergedResult<GraphCollectionResult>().ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Collections;
        }

        /// <summary>
        /// Add an additional vertex collection to the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="collection">The type of the collection</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult AddVertexCollection<T>(string graphName, Action<BaseResult> baseResult = null)
        {
            return AddVertexCollectionAsync<T>(graphName, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Add an additional vertex collection to the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult AddVertexCollection(string graphName, string collection, Action<BaseResult> baseResult = null)
        {
            return AddVertexCollectionAsync(graphName, collection, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Add an additional vertex collection to the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> AddVertexCollectionAsync<T>(string graphName, Action<BaseResult> baseResult = null)
        {
            var collectionName = db.SharedSetting.Collection.ResolveCollectionName<T>();

            return await AddVertexCollectionAsync(graphName, collectionName, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Add an additional vertex collection to the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="collection">The name of the collection</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> AddVertexCollectionAsync(string graphName, string collection, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Post,
                Command = $"{graphName}/vertex"
            };

            var data = new AddVertexCollectionData
            {
                Collection = collection
            };

            var result = await command.RequestMergedResult<GraphResult>(data).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Graph;
        }

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult RemoveVertexCollection<T>(string graphName, bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            return RemoveVertexCollectionAsync<T>(graphName, dropCollection, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> RemoveVertexCollectionAsync<T>(string graphName, bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            var collectionName = db.SharedSetting.Collection.ResolveCollectionName<T>();

            return await RemoveVertexCollectionAsync(graphName, collectionName, dropCollection, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="collection">The name of the collection</param>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult RemoveVertexCollection(string graphName, string collection, bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            return RemoveVertexCollectionAsync(graphName, collection, dropCollection, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="collection">The name of the collection</param>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> RemoveVertexCollectionAsync(string graphName, string collection, bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Delete,
                Command = $"{graphName}/vertex/{collection}"
            };

            var data = new DropGraphCollectionData
            {
                DropCollection = dropCollection
            };

            var result = await command.RequestMergedResult<GraphResult>(data).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Graph;
        }

        /// <summary>
        /// Lists all edge definitions
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public List<string> ListEdgeDefinitions(string graphName, Action<BaseResult> baseResult = null)
        {
            return ListEdgeDefinitionsAsync(graphName, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Lists all edge definitions
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<List<string>> ListEdgeDefinitionsAsync(string graphName, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Get,
                Command = $"{graphName}/edge"
            };

            var result = await command.RequestMergedResult<GraphCollectionResult>().ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Collections;
        }

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="collection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult ExtendEdgeDefinitions(string graphName, string collection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null)
        {
            return ExtendEdgeDefinitionsAsync(graphName, collection, from, to, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="collection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> ExtendEdgeDefinitionsAsync(string graphName, string collection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Post,
                Command = $"{graphName}/edge"
            };

            EdgeDefinitionData data = new EdgeDefinitionData
            {
                Collection = collection,
                From = from,
                To = to
            };

            var result = await command.RequestMergedResult<GraphResult>(data).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Graph;
        }

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="collection">The types of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> ExtendEdgeDefinitionsAsync(string graphName, Type collection, IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null)
        {
            var collectionName = db.SharedSetting.Collection.ResolveCollectionName(collection);
            var fromNames = from.Select(f => db.SharedSetting.Collection.ResolveCollectionName(f)).ToList();
            var toNames = to.Select(t => db.SharedSetting.Collection.ResolveCollectionName(t)).ToList();

            return await ExtendEdgeDefinitionsAsync(graphName, collectionName, fromNames, toNames, baseResult);
        }

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="collection">The types of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult ExtendEdgeDefinitions(string graphName, Type collection, IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null)
        {
            return ExtendEdgeDefinitionsAsync(graphName, collection, from, to, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="collection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult EditEdgeDefinition(string graphName, string definitionName, string collection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null)
        {
            return EditEdgeDefinitionAsync(graphName, definitionName, collection, from, to, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="collection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> EditEdgeDefinitionAsync(string graphName, string definitionName, string collection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Post,
                Command = $"{graphName}/edge/{definitionName}"
            };

            EdgeDefinitionData data = new EdgeDefinitionData
            {
                Collection = collection,
                From = from,
                To = to
            };

            var result = await command.RequestMergedResult<GraphResult>(data).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Graph;
        }

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="collection">The types of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> EditEdgeDefinitionAsync(string graphName, Type definitionName, Type collection, IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null)
        {
            var collectionName = db.SharedSetting.Collection.ResolveCollectionName(collection);
            var definitionCollectionName = db.SharedSetting.Collection.ResolveCollectionName(definitionName);
            var fromNames = from.Select(f => db.SharedSetting.Collection.ResolveCollectionName(f)).ToList();
            var toNames = to.Select(t => db.SharedSetting.Collection.ResolveCollectionName(t)).ToList();

            return await EditEdgeDefinitionAsync(graphName, definitionCollectionName, collectionName, fromNames, toNames, baseResult);
        }

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="collection">The types of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult EditEdgeDefinition(string graphName, Type definitionName, Type collection, IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null)
        {
            return EditEdgeDefinitionAsync(graphName, definitionName, collection, from, to, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult DeleteEdgeDefinition(string graphName, string definitionName, bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            return DeleteEdgeDefinitionAsync(graphName, definitionName, dropCollection, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> DeleteEdgeDefinitionAsync(string graphName, string definitionName, bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Delete,
                Command = $"{graphName}/edge/{definitionName}"
            };

            DropGraphCollectionData data = new DropGraphCollectionData
            {
                DropCollection = dropCollection
            };

            var result = await command.RequestMergedResult<GraphResult>(data).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Graph;
        }

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult DeleteEdgeDefinition<T>(string graphName, bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            return DeleteEdgeDefinitionAsync<T>(graphName, dropCollection, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> DeleteEdgeDefinitionAsync<T>(string graphName, bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            string definitionName = db.SharedSetting.Collection.ResolveCollectionName<T>();

            return await DeleteEdgeDefinitionAsync(graphName, definitionName, dropCollection, baseResult).ConfigureAwait(false);
        }
    }
}
