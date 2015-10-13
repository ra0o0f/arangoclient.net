using ArangoDB.Client.ChangeTracking;
using ArangoDB.Client.Common.Newtonsoft.Json.Linq;
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
        string graphName;

        public ArangoGraph(IArangoDatabase db,string graphName)
        {
            this.db = db;
            this.graphName = graphName;
        }

        /// <summary>
        /// Create graph
        /// </summary>
        /// <param name="edgeDefinitions">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="orphanCollection">Whether or not the collection will be compacted</param>
        /// <returns>GraphIdentifierResult</returns>
        public GraphIdentifierResult Create(IList<EdgeDefinitionData> edgeDefinitions
            , IList<string> orphanCollections = null, Action<BaseResult> baseResult = null)
        {
            return CreateAsync(edgeDefinitions, orphanCollections, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Create graph
        /// </summary>
        /// <param name="edgeDefinitions">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="orphanCollection">Whether or not the collection will be compacted</param>
        /// <returns>GraphIdentifierResult</returns>
        public GraphIdentifierResult Create(IList<EdgeDefinitionTypedData> edgeDefinitions, IList<Type> orphanCollections = null
            , Action<BaseResult> baseResult = null)
        {
            return CreateAsync(edgeDefinitions, orphanCollections, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Create graph
        /// </summary>
        /// <param name="edgeDefinitions">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="orphanCollection">Whether or not the collection will be compacted</param>
        /// <returns>GraphIdentifierResult</returns>
        public async Task<GraphIdentifierResult> CreateAsync(IList<EdgeDefinitionData> edgeDefinitions
            , IList<string> orphanCollections = null, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Post
            };

            var data = new GraphCollectionData
            {
                Name = graphName,
                EdgeDefinitions = edgeDefinitions,
                OrphanCollections = orphanCollections
            };

            var result = await command.RequestMergedResult<GraphResult>(data).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Graph;
        }

        /// <summary>
        /// Create graph
        /// </summary>
        /// <param name="edgeDefinitions">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="orphanCollection">Whether or not the collection will be compacted</param>
        /// <returns>GraphIdentifierResult</returns>
        public async Task<GraphIdentifierResult> CreateAsync(IList<EdgeDefinitionTypedData> edgeDefinitions, IList<Type> orphanCollections = null
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

            return await CreateAsync(graphEdgeDefinitions, graphOrphanCollections, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete graph
        /// </summary>
        /// <param name="dropCollections">Drop collections of this graph as well. Collections will only be dropped if they are not used in other graphs.</param>
        /// <returns></returns>
        public bool Drop(bool dropCollections = false, Action<BaseResult> baseResult = null)
        {
            return DropAsync(dropCollections, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Delete graph
        /// </summary>
        /// <param name="dropCollections">Drop collections of this graph as well. Collections will only be dropped if they are not used in other graphs.</param>
        /// <returns></returns>
        public async Task<bool> DropAsync(bool dropCollections = false, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Delete,
                Command = graphName
            };

            var data = new GraphCollectionData
            {
                DropCollections = dropCollections
            };

            var result = await command.RequestMergedResult<DropGraphResult>(data).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Removed;
        }

        /// <summary>
        /// Get graph info
        /// </summary>
        /// <returns>GraphIdentifierResult</returns>
        public GraphIdentifierResult Info( Action<BaseResult> baseResult = null)
        {
            return InfoAsync(baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Get graph info
        /// </summary>
        /// <returns>GraphIdentifierResult</returns>
        public async Task<GraphIdentifierResult> InfoAsync(Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Get,
                Command = graphName
            };

            var result = await command.RequestMergedResult<GraphResult>().ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Graph;
        }

        

        /// <summary>
        /// Lists all vertex collections used in graph
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public List<string> ListVertexCollections(Action<BaseResult> baseResult = null)
        {
            return ListVertexCollectionsAsync(baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Lists all vertex collections used in graph
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<List<string>> ListVertexCollectionsAsync(Action<BaseResult> baseResult = null)
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
        /// <param name="collection">The type of the collection</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult AddVertexCollection<T>(Action<BaseResult> baseResult = null)
        {
            return AddVertexCollectionAsync<T>(baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Add an additional vertex collection to the graph
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult AddVertexCollection(string collection, Action<BaseResult> baseResult = null)
        {
            return AddVertexCollectionAsync(collection, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Add an additional vertex collection to the graph
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> AddVertexCollectionAsync<T>(Action<BaseResult> baseResult = null)
        {
            var collectionName = db.SharedSetting.Collection.ResolveCollectionName<T>();

            return await AddVertexCollectionAsync(collectionName, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Add an additional vertex collection to the graph
        /// </summary>
        /// <param name="collection">The name of the collection</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> AddVertexCollectionAsync(string collection, Action<BaseResult> baseResult = null)
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
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult RemoveVertexCollection<T>(bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            return RemoveVertexCollectionAsync<T>(dropCollection, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> RemoveVertexCollectionAsync<T>(bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            var collectionName = db.SharedSetting.Collection.ResolveCollectionName<T>();

            return await RemoveVertexCollectionAsync(collectionName, dropCollection, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="collection">The name of the collection</param>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult RemoveVertexCollection(string collection, bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            return RemoveVertexCollectionAsync(collection, dropCollection, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="collection">The name of the collection</param>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> RemoveVertexCollectionAsync(string collection, bool dropCollection = false, Action<BaseResult> baseResult = null)
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
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public List<string> ListEdgeDefinitions(Action<BaseResult> baseResult = null)
        {
            return ListEdgeDefinitionsAsync(baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Lists all edge definitions
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<List<string>> ListEdgeDefinitionsAsync(Action<BaseResult> baseResult = null)
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
        /// <param name="collection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult ExtendEdgeDefinitions(string collection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null)
        {
            return ExtendEdgeDefinitionsAsync(collection, from, to, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="collection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> ExtendEdgeDefinitionsAsync(string collection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null)
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
        /// <param name="collection">The types of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> ExtendEdgeDefinitionsAsync(Type collection, IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null)
        {
            var collectionName = db.SharedSetting.Collection.ResolveCollectionName(collection);
            var fromNames = from.Select(f => db.SharedSetting.Collection.ResolveCollectionName(f)).ToList();
            var toNames = to.Select(t => db.SharedSetting.Collection.ResolveCollectionName(t)).ToList();

            return await ExtendEdgeDefinitionsAsync(collectionName, fromNames, toNames, baseResult);
        }

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="collection">The types of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult ExtendEdgeDefinitions(Type collection, IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null)
        {
            return ExtendEdgeDefinitionsAsync(collection, from, to, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="collection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult EditEdgeDefinition(string definitionName, string collection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null)
        {
            return EditEdgeDefinitionAsync(definitionName, collection, from, to, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="collection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> EditEdgeDefinitionAsync(string definitionName, string collection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null)
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
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="collection">The types of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> EditEdgeDefinitionAsync(Type definitionName, Type collection, IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null)
        {
            var collectionName = db.SharedSetting.Collection.ResolveCollectionName(collection);
            var definitionCollectionName = db.SharedSetting.Collection.ResolveCollectionName(definitionName);
            var fromNames = from.Select(f => db.SharedSetting.Collection.ResolveCollectionName(f)).ToList();
            var toNames = to.Select(t => db.SharedSetting.Collection.ResolveCollectionName(t)).ToList();

            return await EditEdgeDefinitionAsync(definitionCollectionName, collectionName, fromNames, toNames, baseResult);
        }

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="collection">The types of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult EditEdgeDefinition(Type definitionName, Type collection, IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null)
        {
            return EditEdgeDefinitionAsync(definitionName, collection, from, to, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult DeleteEdgeDefinition(string definitionName, bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            return DeleteEdgeDefinitionAsync(definitionName, dropCollection, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> DeleteEdgeDefinitionAsync(string definitionName, bool dropCollection = false, Action<BaseResult> baseResult = null)
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
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult DeleteEdgeDefinition<T>(bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            return DeleteEdgeDefinitionAsync<T>(dropCollection, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> DeleteEdgeDefinitionAsync<T>(bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            string definitionName = db.SharedSetting.Collection.ResolveCollectionName<T>();

            return await DeleteEdgeDefinitionAsync(definitionName, dropCollection, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new vertex
        /// </summary>
        /// <param name="document">The vertex document</param>
        /// <param name="collection">The name of the vertex collection the vertex belongs to</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        public IDocumentIdentifierResult InsertVertex(object document, string collection, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return InsertVertexAsync(document, collection, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Creates a new vertex
        /// </summary>
        /// <param name="document">The vertex document</param>
        /// <param name="collection">The name of the vertex collection the vertex belongs to</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        public async Task<IDocumentIdentifierResult> InsertVertexAsync(object document, string collection, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            waitForSync = waitForSync ?? db.Setting.WaitForSync;

            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Post,
                Command = $"{graphName}/vertex/{collection}",
                Query = new Dictionary<string, string>()
            };

            command.Query.Add("waitForSync", waitForSync.ToString());

            var result = await command.RequestMergedResult<InsertVertexResult>(document).ConfigureAwait(false);

            if (!result.BaseResult.Error)
            {
                if (db.Setting.DisableChangeTracking == false)
                    db.ChangeTracker.TrackChanges(document, result.Result.Vertex);

                db.SharedSetting.IdentifierModifier.Modify(document, result.Result.Vertex);
            }

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Vertex;
        }

        /// <summary>
        /// Creates a new vertex
        /// </summary>
        /// <param name="document">The vertex document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        public IDocumentIdentifierResult InsertVertex<T>(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return InsertVertexAsync<T>(document, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Creates a new vertex
        /// </summary>
        /// <param name="document">The vertex document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        public async Task<IDocumentIdentifierResult> InsertVertexAsync<T>( object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            var collectionName = db.SharedSetting.Collection.ResolveCollectionName<T>();

            return await InsertVertexAsync(document, collectionName, waitForSync, baseResult);
        }

        /// <summary>
        /// Fetches an existing vertex
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="collectionName">The name of the vertex collection the vertex belongs to</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        public T GetVertex<T>(string id, string collectionName, Action<BaseResult> baseResult = null)
        {
            return GetVertexAsync<T>(id, collectionName, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Fetches an existing vertex
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="collectionName">The name of the vertex collection the vertex belongs to</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        public async Task<T> GetVertexAsync<T>(string id, string collectionName, Action<BaseResult> baseResult = null)
        {
            var documentHandle = id.IndexOf("/") == -1 ? $"{collectionName}/{id}" : id;

            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Get,
                Command = $"{graphName}/vertex/{documentHandle}",
                EnableChangeTracking = !db.Setting.DisableChangeTracking
            };

            var defaultThrowForServerErrors = db.Setting.ThrowForServerErrors;
            db.Setting.ThrowForServerErrors = false;

            var result = await command.RequestGenericSingleResult<T, VertexInheritedCommandResult<T>>().ConfigureAwait(false);

            db.Setting.ThrowForServerErrors = defaultThrowForServerErrors;

            if (db.Setting.Document.ThrowIfDocumentDoesNotExists ||
                (result.BaseResult.Error && result.BaseResult.ErrorNum != 1202))
                new BaseResultAnalyzer(db).Throw(result.BaseResult);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result;
        }

        /// <summary>
        /// Fetches an existing vertex
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        public T GetVertex<T>(string id, Action<BaseResult> baseResult = null)
        {
            return GetVertexAsync<T>(id, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Fetches an existing vertex
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        public async Task<T> GetVertexAsync<T>(string id, Action<BaseResult> baseResult = null)
        {
            var collectionName = db.SharedSetting.Collection.ResolveCollectionName<T>();

            return await GetVertexAsync<T>(id, collectionName, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Partially updates the vertex with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="collectionName">The name of the vertex collection the vertex belongs to</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public IDocumentIdentifierResult UpdateVertexById(string id, object document, string collectionName
            , bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null)
        {
            return UpdateVertexByIdAsync(id, document, collectionName, waitForSync, keepNull, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Partially updates the vertex with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="collectionName">The name of the vertex collection the vertex belongs to</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<IDocumentIdentifierResult> UpdateVertexByIdAsync(string id, object document, string collectionName
            , bool? waitForSync = null, bool? keepNull=null, Action<BaseResult> baseResult = null)
        {
            var documentHandle = id.IndexOf("/") == -1 ? $"{collectionName}/{id}" : id;

            keepNull = keepNull ?? db.Setting.Document.KeepNullAttributesOnUpdate;
            waitForSync = waitForSync ?? db.Setting.WaitForSync;

            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = new HttpMethod("PATCH"),
                Query = new Dictionary<string, string>(),
                Command = $"{graphName}/vertex/{documentHandle}"
            };

            command.Query.Add("keepNull", keepNull.ToString());
            command.Query.Add("waitForSync", waitForSync.ToString());

            var result = await command.RequestMergedResult<UpdateVertexResult>(document).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Vertex;
        }

        ///<summary>
        ///Partially updates the vertex
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        public IDocumentIdentifierResult UpdateVertex<T>(object document,
           bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null)
        {
            return UpdateVertexAsync<T>(document, waitForSync, keepNull, baseResult).ResultSynchronizer();
        }

        ///<summary>
        ///Partially updates the vertex
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        public async Task<IDocumentIdentifierResult> UpdateVertexAsync<T>(object document,
           bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null)
        {
            if (db.Setting.DisableChangeTracking == true)
                throw new InvalidOperationException("Change tracking is disabled, use UpdateById() instead");
            
            DocumentContainer container = null;
            JObject jObject = null;
            var changed = db.ChangeTracker.GetChanges(document, out container, out jObject);

            string collectionName = db.SharedSetting.Collection.ResolveCollectionName<T>();
            
            if (changed.Count != 0)
            {
                BaseResult bResult = null;

                var result = await UpdateVertexByIdAsync(container.Id, changed, collectionName, waitForSync, keepNull, (b) => bResult = b).ConfigureAwait(false);

                if (!bResult.Error)
                {
                    container.Rev = result.Rev;
                    container.Document = jObject;
                    db.SharedSetting.IdentifierModifier.FindIdentifierMethodFor(document.GetType()).SetRevision(document, result.Rev);
                }

                return result;
            }
            else
                return new DocumentIdentifierWithoutBaseResult() { Id = container.Id, Key = container.Key, Rev = container.Rev };

        }
    }
}
