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
        bool Drop(bool dropCollections = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Delete graph
        /// </summary>
        /// <param name="dropCollections">Drop collections of this graph as well. Collections will only be dropped if they are not used in other graphs.</param>
        /// <returns>Task</returns>
        Task<bool> DropAsync(bool dropCollections = false, Action<BaseResult> baseResult = null);

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
        /// <param name="collection">The name of the collection</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> AddVertexCollectionAsync(string collection, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Add an additional vertex collection to the graph
        /// </summary>
        /// <param name="collection">The name of the collection</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult AddVertexCollection(string collection, Action<BaseResult> baseResult = null);

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
        /// <param name="collection">The name of the collection</param>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult RemoveVertexCollection(string collection, bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="collection">The name of the collection</param>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> RemoveVertexCollectionAsync(string collection, bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> RemoveVertexCollectionAsync<T>(bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult RemoveVertexCollection<T>(bool dropCollection = false, Action<BaseResult> baseResult = null);

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
        /// <param name="collection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> ExtendEdgeDefinitionsAsync(string collection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="collection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult ExtendEdgeDefinitions(string collection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="collection">The types of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> ExtendEdgeDefinitionsAsync(Type collection, IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="collection">The types of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult ExtendEdgeDefinitions(Type collection, IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="collection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult EditEdgeDefinition(string definitionName, string collection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null);


        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="collection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> EditEdgeDefinitionAsync(string definitionName, string collection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="collection">The types of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> EditEdgeDefinitionAsync(Type definitionName, Type collection, IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="collection">The types of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult EditEdgeDefinition(Type definitionName, Type collection, IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> DeleteEdgeDefinitionAsync(string definitionName, bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult DeleteEdgeDefinition(string definitionName, bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult DeleteEdgeDefinition<T>(bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> DeleteEdgeDefinitionAsync<T>(bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a new vertex
        /// </summary>
        /// <param name="document">The vertex document</param>
        /// <param name="collection">The name of the vertex collection the vertex belongs to</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        IDocumentIdentifierResult InsertVertex(object document, string collection, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a new vertex
        /// </summary>
        /// <param name="document">The vertex document</param>
        /// <param name="collection">The name of the vertex collection the vertex belongs to</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        Task<IDocumentIdentifierResult> InsertVertexAsync(object document, string collection, bool? waitForSync = null, Action<BaseResult> baseResult = null);

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
        /// <param name="collectionName">The name of the vertex collection the vertex belongs to</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        T GetVertex<T>(string id, string collectionName, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Fetches an existing vertex
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="collectionName">The name of the vertex collection the vertex belongs to</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        Task<T> GetVertexAsync<T>(string id, string collectionName, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Fetches an existing vertex
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        T GetVertex<T>(string id, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Fetches an existing vertex
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        Task<T> GetVertexAsync<T>(string id, Action<BaseResult> baseResult = null);

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
        IDocumentIdentifierResult UpdateVertexById(string id, object document, string collectionName
            , bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null);

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
        Task<IDocumentIdentifierResult> UpdateVertexByIdAsync(string id, object document, string collectionName
            , bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null);


        ///<summary>
        ///Partially updates the vertex
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        IDocumentIdentifierResult UpdateVertex<T>(object document,
           bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null);

        ///<summary>
        ///Partially updates the vertex
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> UpdateVertexAsync<T>(object document,
           bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null);
    }
}
