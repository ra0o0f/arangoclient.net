using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public interface IArangoGraph
    {
        IArangoGraphVertex Vertex(string collection);

        IArangoGraphVertex<T> Vertex<T>();

        IArangoGraphEdge Edge(string collection);

        IArangoGraphEdge<T> Edge<T>();
        
        /// <summary>
        /// Name of the graph
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Create graph
        /// </summary>
        /// <param name="edgeDefinitions">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="orphanCollection">Whether or not the collection will be compacted</param>
        /// <returns>GraphIdentifierResult</returns>
        GraphIdentifierResult Create(IList<EdgeDefinitionData> edgeDefinitions, IList<string> orphanCollections = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Create graph
        /// </summary>
        /// <param name="edgeDefinitions">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="orphanCollection">Whether or not the collection will be compacted</param>
        /// <returns>GraphIdentifierResult</returns>
        Task<GraphIdentifierResult> CreateAsync(IList<EdgeDefinitionData> edgeDefinitions, IList<string> orphanCollections = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Create graph
        /// </summary>
        /// <param name="edgeDefinitions">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="orphanCollection">Whether or not the collection will be compacted</param>
        /// <returns>GraphIdentifierResult</returns>
        GraphIdentifierResult Create(IList<EdgeDefinitionTypedData> edgeDefinitions, IList<Type> orphanCollections = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Create graph
        /// </summary>
        /// <param name="edgeDefinitions">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="orphanCollection">Whether or not the collection will be compacted</param>
        /// <returns>GraphIdentifierResult</returns>
        Task<GraphIdentifierResult> CreateAsync(IList<EdgeDefinitionTypedData> edgeDefinitions, IList<Type> orphanCollections = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Delete graph
        /// </summary>
        /// <param name="dropCollections">Drop collections of this graph as well. Collections will only be dropped if they are not used in other graphs.</param>
        /// <returns></returns>
        bool Drop(bool? dropCollections = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Delete graph
        /// </summary>
        /// <param name="dropCollections">Drop collections of this graph as well. Collections will only be dropped if they are not used in other graphs.</param>
        /// <returns>Task</returns>
        Task<bool> DropAsync(bool? dropCollections = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Get graph info
        /// </summary>
        /// <returns>GraphIdentifierResult</returns>
        GraphIdentifierResult Info(Action<BaseResult> baseResult = null);

        /// <summary>
        /// Get graph info
        /// </summary>
        /// <returns>GraphIdentifierResult</returns>
        Task<GraphIdentifierResult> InfoAsync(Action<BaseResult> baseResult = null);
        
        /// <summary>
        /// Lists all vertex collections used in graph
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<List<string>> ListVertexCollectionsAsync(Action<BaseResult> baseResult = null);

        /// <summary>
        /// Lists all vertex collections used in graph
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        List<string> ListVertexCollections(Action<BaseResult> baseResult = null);
        
        /// <summary>
        /// Add an additional vertex collection to the graph
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> AddVertexCollectionAsync<T>(Action<BaseResult> baseResult = null);

        /// <summary>
        /// Add an additional vertex collection to the graph
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult AddVertexCollection<T>(Action<BaseResult> baseResult = null);
        
        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> RemoveVertexCollectionAsync<T>(bool? dropCollection = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult RemoveVertexCollection<T>(bool? dropCollection = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Lists all edge definitions
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<List<string>> ListEdgeDefinitionsAsync(Action<BaseResult> baseResult = null);

        /// <summary>
        /// Lists all edge definitions
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        List<string> ListEdgeDefinitions(Action<BaseResult> baseResult = null);
        
        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> ExtendEdgeDefinitionsAsync<T>(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult ExtendEdgeDefinitions<T>(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);
        
        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> EditEdgeDefinitionAsync<T, TCollection>(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult EditEdgeDefinition<T, TCollection>(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult DeleteEdgeDefinition<T>(bool? dropCollection = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> DeleteEdgeDefinitionAsync<T>(bool? dropCollection = null, Action<BaseResult> baseResult = null);
        
        /// <summary>
        /// Creates a new vertex
        /// </summary>
        /// <param name="document">The vertex document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        IDocumentIdentifierResult InsertVertex<T>(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a new vertex
        /// </summary>
        /// <param name="document">The vertex document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        Task<IDocumentIdentifierResult> InsertVertexAsync<T>(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Fetches an existing vertex
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        T GetVertex<T>(string id, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Fetches an existing vertex
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        Task<T> GetVertexAsync<T>(string id, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Partially updates the vertex with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        IDocumentIdentifierResult UpdateVertexById<T>(string id, object document
            , bool? waitForSync = null, bool? keepNull = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Partially updates the vertex with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<IDocumentIdentifierResult> UpdateVertexByIdAsync<T>(string id, object document
            , bool? waitForSync = null, bool? keepNull = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);


        ///<summary>
        ///Partially updates the vertex
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        IDocumentIdentifierResult UpdateVertex<T>(object document,
           bool? waitForSync = null, bool? keepNull = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        ///<summary>
        ///Partially updates the vertex
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> UpdateVertexAsync<T>(object document,
           bool? waitForSync = null, bool? keepNull = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        
        /// <summary>
        /// Completely updates the vertex with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        IDocumentIdentifierResult ReplaceVertexById<T>(string id, object document,
            bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the vertex with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> ReplaceVertexByIdAsync<T>(string id, object document,
            bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the vertex
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        IDocumentIdentifierResult ReplaceVertex<T>(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the vertex
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> ReplaceVertexAsync<T>(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the vertex without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        bool RemoveVertexById<T>(string id, bool? waitForSync = null, string ifMatchRev = null
            , Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the vertex without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        Task<bool> RemoveVertexByIdAsync<T>(string id,
            bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the vertex
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        bool RemoveVertex<T>(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the vertex
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        Task<bool> RemoveVertexAsync<T>(object document,
            bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a new edge
        /// </summary>
        /// <param name="document">The edge document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        IDocumentIdentifierResult InsertEdge<T>(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a new edge
        /// </summary>
        /// <param name="document">The edge document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        Task<IDocumentIdentifierResult> InsertEdgeAsync<T>(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Fetches an existing edge
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        T GetEdge<T>(string id, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Fetches an existing edge
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        Task<T> GetEdgeAsync<T>(string id, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Partially updates the edge with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        IDocumentIdentifierResult UpdateEdgeById<T>(string id, object document
            , bool? waitForSync = null, bool? keepNull = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Partially updates the edge with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<IDocumentIdentifierResult> UpdateEdgeByIdAsync<T>(string id, object document
            , bool? waitForSync = null, bool? keepNull = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);


        ///<summary>
        ///Partially updates the edge
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        IDocumentIdentifierResult UpdateEdge<T>(object document,
           bool? waitForSync = null, bool? keepNull = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        ///<summary>
        ///Partially updates the edge
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> UpdateEdgeAsync<T>(object document,
           bool? waitForSync = null, bool? keepNull = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the edge with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        IDocumentIdentifierResult ReplaceEdgeById<T>(string id, object document,
            bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the edge with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> ReplaceEdgeByIdAsync<T>(string id, object document,
            bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the edge
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        IDocumentIdentifierResult ReplaceEdge<T>(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the edge
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> ReplaceEdgeAsync<T>(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the edge without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        bool RemoveEdgeById<T>(string id, bool? waitForSync = null, string ifMatchRev = null
            , Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the edge without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        Task<bool> RemoveEdgeByIdAsync<T>(string id,
            bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the edge
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        bool RemoveEdge<T>(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the edge
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        Task<bool> RemoveEdgeAsync<T>(object document,
            bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);
    }
}
