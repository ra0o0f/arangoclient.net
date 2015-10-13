using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public interface IArangoGraphEdge
    {
        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> ExtendDefinitionsAsync(IList<string> from, IList<string> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult ExtendDefinitions(IList<string> from, IList<string> to, Action<BaseResult> baseResult = null);
        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="newCollection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult EditDefinition(string newCollection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null);
        
        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="newCollection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> EditDefinitionAsync(string newCollection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null);


        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> DeleteDefinitionAsync( bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult DeleteDefinition(bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a new edge
        /// </summary>
        /// <param name="document">The edge document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        IDocumentIdentifierResult Insert(string from, string to, object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a new edge
        /// </summary>
        /// <param name="document">The edge document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        Task<IDocumentIdentifierResult> InsertAsync(string from, string to, object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Fetches an existing edge
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        T Get<T>(string id, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Fetches an existing edge
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        Task<T> GetAsync<T>(string id, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Partially updates the edge with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        IDocumentIdentifierResult UpdateById(string id, object document
            , bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Partially updates the edge with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<IDocumentIdentifierResult> UpdateByIdAsync(string id, object document
            , bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null);

        ///<summary>
        ///Partially updates the edge
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        IDocumentIdentifierResult Update(object document,
           bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null);

        ///<summary>
        ///Partially updates the edge
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> UpdateAsync(object document,
           bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the edge with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        IDocumentIdentifierResult ReplaceById(string id, object document,
            bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the edge with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> ReplaceByIdAsync(string id, object document,
            bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the edge
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        IDocumentIdentifierResult Replace(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the edge
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> ReplaceAsync(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the edge without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        bool RemoveById(string id, bool? waitForSync = null
            , Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the edge without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        Task<bool> RemoveByIdAsync(string id,
            bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the edge
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        bool Remove(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the edge
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        Task<bool> RemoveAsync(object document,
            bool? waitForSync = null, Action<BaseResult> baseResult = null);
    }

    public interface IArangoGraphEdge<T>
    {
        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> ExtendDefinitionsAsync(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult ExtendDefinitions(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> EditDefinitionAsync<TCollection>(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult EditDefinition<TCollection>(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult DeleteDefinition(bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> DeleteDefinitionAsync(bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a new edge
        /// </summary>
        /// <param name="document">The edge document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        IDocumentIdentifierResult Insert<TFrom, TTo>(string from, string to, object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a new edge
        /// </summary>
        /// <param name="document">The edge document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        Task<IDocumentIdentifierResult> InsertAsync<TFrom, TTo>(string from, string to, object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Fetches an existing edge
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        T Get(string id, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Fetches an existing edge
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        Task<T> GetAsync(string id, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Partially updates the edge with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        IDocumentIdentifierResult UpdateById(string id, object document
            , bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Partially updates the edge with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<IDocumentIdentifierResult> UpdateByIdAsync(string id, object document
            , bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null);

        ///<summary>
        ///Partially updates the edge
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        IDocumentIdentifierResult Update(object document,
           bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null);

        ///<summary>
        ///Partially updates the edge
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> UpdateAsync(object document,
           bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the edge with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        IDocumentIdentifierResult ReplaceById(string id, object document,
            bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the edge with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> ReplaceByIdAsync(string id, object document,
            bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the edge
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        IDocumentIdentifierResult Replace(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the edge
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> ReplaceAsync(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the edge without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        bool RemoveById(string id, bool? waitForSync = null
            , Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the edge without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        Task<bool> RemoveByIdAsync(string id,
            bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the edge
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        bool Remove(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the edge
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        Task<bool> RemoveAsync(object document,
            bool? waitForSync = null, Action<BaseResult> baseResult = null);
    }
}
