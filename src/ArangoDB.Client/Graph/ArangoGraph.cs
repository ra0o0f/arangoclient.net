using ArangoDB.Client.ChangeTracking;
using Newtonsoft.Json.Linq;
using ArangoDB.Client.Data;
using ArangoDB.Client.Http;
using ArangoDB.Client.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Graph
{
    public class ArangoGraph : IArangoGraph
    {
        IArangoDatabase db;
        string graphName;

        public ArangoGraph(IArangoDatabase db, string graphName)
        {
            this.db = db;
            this.graphName = graphName;
        }

        public string Name
        {
            get { return graphName; }
        }

        public IArangoGraphVertex Vertex(string collection)
        {
            return new ArangoGraphVertex(db, graphName, collection);
        }

        public IArangoGraphVertex<T> Vertex<T>()
        {
            return new ArangoGraphVertex<T>(db, graphName);
        }

        public IArangoGraphEdge Edge(string collection)
        {
            return new ArangoGraphEdge(db, graphName, collection);
        }

        public IArangoGraphEdge<T> Edge<T>()
        {
            return new ArangoGraphEdge<T>(db, graphName);
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

            var data = new CreateGraphData
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
        public bool Drop(bool? dropCollections = null, Action<BaseResult> baseResult = null)
        {
            return DropAsync(dropCollections, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Delete graph
        /// </summary>
        /// <param name="dropCollections">Drop collections of this graph as well. Collections will only be dropped if they are not used in other graphs.</param>
        /// <returns></returns>
        public async Task<bool> DropAsync(bool? dropCollections = null, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Delete,
                Command = graphName,
                Query = new Dictionary<string, string>()
            };

            if (dropCollections.HasValue)
                command.Query.Add("dropCollections", dropCollections.Value.ToString());

            var result = await command.RequestMergedResult<DropGraphResult>().ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Removed;
        }

        /// <summary>
        /// Get graph info
        /// </summary>
        /// <returns>GraphIdentifierResult</returns>
        public GraphIdentifierResult Info(Action<BaseResult> baseResult = null)
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
        public async Task<GraphIdentifierResult> AddVertexCollectionAsync<T>(Action<BaseResult> baseResult = null)
        {
            return await Vertex<T>().AddCollectionAsync(baseResult).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult RemoveVertexCollection<T>(bool? dropCollection = null, Action<BaseResult> baseResult = null)
        {
            return RemoveVertexCollectionAsync<T>(dropCollection, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> RemoveVertexCollectionAsync<T>(bool? dropCollection = null, Action<BaseResult> baseResult = null)
        {
            return await Vertex<T>().RemoveCollectionAsync(dropCollection, baseResult).ConfigureAwait(false);
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
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> ExtendEdgeDefinitionsAsync<T>(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null)
        {
            return await Edge<T>().ExtendDefinitionsAsync(from, to, baseResult);
        }

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult ExtendEdgeDefinitions<T>(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null)
        {
            return ExtendEdgeDefinitionsAsync<T>(from, to, baseResult).ResultSynchronizer();
        }
        
        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> EditEdgeDefinitionAsync<T,TCollection>(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null)
        {
            return await Edge<T>().EditDefinitionAsync<TCollection>(from, to, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult EditEdgeDefinition<T, TCollection>(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null)
        {
            return EditEdgeDefinitionAsync<T, TCollection>(from, to, baseResult).ResultSynchronizer();
        }


        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult DeleteEdgeDefinition<T>(bool? dropCollection = null, Action<BaseResult> baseResult = null)
        {
            return DeleteEdgeDefinitionAsync<T>(dropCollection, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> DeleteEdgeDefinitionAsync<T>(bool? dropCollection = null, Action<BaseResult> baseResult = null)
        {
            return await Edge<T>().DeleteDefinitionAsync(dropCollection, baseResult).ConfigureAwait(false);
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
        public async Task<IDocumentIdentifierResult> InsertVertexAsync<T>(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return await Vertex<T>().InsertAsync(document, waitForSync, baseResult).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Fetches an existing vertex
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        public T GetVertex<T>(string id, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return GetVertexAsync<T>(id, ifMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Fetches an existing vertex
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        public async Task<T> GetVertexAsync<T>(string id, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Vertex<T>().GetAsync(id, ifMatchRev, baseResult).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Partially updates the vertex with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public IDocumentIdentifierResult UpdateVertexById<T>(string id, object document
            , bool? waitForSync = null, bool? keepNull = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return UpdateVertexByIdAsync<T>(id, document, waitForSync, keepNull, ifMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Partially updates the vertex with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<IDocumentIdentifierResult> UpdateVertexByIdAsync<T>(string id, object document
            , bool? waitForSync = null, bool? keepNull = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Vertex<T>().UpdateByIdAsync(id, document, waitForSync, keepNull, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        ///<summary>
        ///Partially updates the vertex
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        public IDocumentIdentifierResult UpdateVertex<T>(object document,
           bool? waitForSync = null, bool? keepNull = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return UpdateVertexAsync<T>(document, waitForSync, keepNull, ifMatchRev, baseResult).ResultSynchronizer();
        }

        ///<summary>
        ///Partially updates the vertex
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        public async Task<IDocumentIdentifierResult> UpdateVertexAsync<T>(object document,
           bool? waitForSync = null, bool? keepNull = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Vertex<T>().UpdateAsync(document, waitForSync, keepNull, ifMatchRev, baseResult).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Completely updates the vertex with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public IDocumentIdentifierResult ReplaceVertexById<T>(string id, object document,
            bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return ReplaceVertexByIdAsync<T>(id, document, waitForSync, ifMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Completely updates the vertex with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<IDocumentIdentifierResult> ReplaceVertexByIdAsync<T>(string id, object document,
            bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Vertex<T>().ReplaceByIdAsync(id, document, waitForSync, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Completely updates the vertex
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public IDocumentIdentifierResult ReplaceVertex<T>(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return ReplaceVertexAsync<T>(document, waitForSync, ifMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Completely updates the vertex
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<IDocumentIdentifierResult> ReplaceVertexAsync<T>(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Vertex<T>().ReplaceAsync(document, waitForSync, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the document without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public bool RemoveVertexById<T>(string id, bool? waitForSync = null, string ifMatchRev = null
            , Action<BaseResult> baseResult = null)
        {
            return RemoveVertexByIdAsync<T>(id, waitForSync, ifMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Deletes the vertex without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task<bool> RemoveVertexByIdAsync<T>(string id,
            bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Vertex<T>().RemoveByIdAsync(id, waitForSync, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the vertex
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public bool RemoveVertex<T>(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return RemoveVertexAsync<T>(document, waitForSync, ifMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Deletes the vertex
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task<bool> RemoveVertexAsync<T>(object document,
            bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Vertex<T>().RemoveAsync(document, waitForSync, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new edge
        /// </summary>
        /// <param name="document">The edge document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        public IDocumentIdentifierResult InsertEdge<T>(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return InsertEdgeAsync<T>(document, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Creates a new edge
        /// </summary>
        /// <param name="document">The edge document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        public async Task<IDocumentIdentifierResult> InsertEdgeAsync<T>(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return await Edge<T>().InsertAsync(document, waitForSync, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Fetches an existing edge
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        public T GetEdge<T>(string id, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return GetEdgeAsync<T>(id, ifMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Fetches an existing edge
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        public async Task<T> GetEdgeAsync<T>(string id, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Edge<T>().GetAsync(id, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Partially updates the edge with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public IDocumentIdentifierResult UpdateEdgeById<T>(string id, object document
            , bool? waitForSync = null, bool? keepNull = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return UpdateEdgeByIdAsync<T>(id, document, waitForSync, keepNull, ifMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Partially updates the edge with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<IDocumentIdentifierResult> UpdateEdgeByIdAsync<T>(string id, object document
            , bool? waitForSync = null, bool? keepNull = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Edge<T>().UpdateByIdAsync(id, document, waitForSync, keepNull, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        ///<summary>
        ///Partially updates the edge
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        public IDocumentIdentifierResult UpdateEdge<T>(object document,
           bool? waitForSync = null, bool? keepNull = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return UpdateEdgeAsync<T>(document, waitForSync, keepNull, ifMatchRev, baseResult).ResultSynchronizer();
        }

        ///<summary>
        ///Partially updates the edge
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        public async Task<IDocumentIdentifierResult> UpdateEdgeAsync<T>(object document,
           bool? waitForSync = null, bool? keepNull = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Edge<T>().UpdateAsync(document, waitForSync, keepNull, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Completely updates the edge with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public IDocumentIdentifierResult ReplaceEdgeById<T>(string id, object document,
            bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return ReplaceEdgeByIdAsync<T>(id, document, waitForSync, ifMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Completely updates the edge with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<IDocumentIdentifierResult> ReplaceEdgeByIdAsync<T>(string id, object document,
            bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Edge<T>().ReplaceByIdAsync(id, document, waitForSync, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Completely updates the edge
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public IDocumentIdentifierResult ReplaceEdge<T>(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return ReplaceEdgeAsync<T>(document, waitForSync, ifMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Completely updates the edge
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<IDocumentIdentifierResult> ReplaceEdgeAsync<T>(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Edge<T>().ReplaceAsync(document, waitForSync, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the edge without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public bool RemoveEdgeById<T>(string id, bool? waitForSync = null, string ifMatchRev = null
            , Action<BaseResult> baseResult = null)
        {
            return RemoveEdgeByIdAsync<T>(id, waitForSync, ifMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Deletes the vertex without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task<bool> RemoveEdgeByIdAsync<T>(string id,
            bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Edge<T>().RemoveByIdAsync(id, waitForSync, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the edge
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public bool RemoveEdge<T>(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return RemoveEdgeAsync<T>(document, waitForSync, ifMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Deletes the vertex
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task<bool> RemoveEdgeAsync<T>(object document,
            bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Edge<T>().RemoveAsync(document, waitForSync, ifMatchRev, baseResult).ConfigureAwait(false);
        }
        
    }
}
