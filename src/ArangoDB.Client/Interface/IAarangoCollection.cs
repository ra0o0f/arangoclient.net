using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public interface IArangoCollection
    {
        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        T Document<T>(string id, string ifMatchRev = null, string ifNoneMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        Task<T> DocumentAsync<T>(string id, string ifMatchRev = null, string ifNoneMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Check if document exists
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>A Document</returns>
        Task<bool> ExistsAsync(string id, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Check if document exists
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="onDocumentLoad">Runs when document loaded</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>A Document</returns>
        Task<bool> ExistsAsync<T>(string id, Action<T> onDocumentLoad, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Check if document exists
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>A Document</returns>
        bool Exists(string id, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Check if document exists
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="onDocumentLoad">Runs when document loaded</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>A Document</returns>
        bool Exists<T>(string id, Action<T> onDocumentLoad, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Returns all documents of a collections
        /// </summary>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        ICursor<T> All<T>(int? skip = null, int? limit = null, int? batchSize = null);

        /// <summary>
        /// Completely updates the document
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        IDocumentIdentifierResult Replace(object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> ReplaceAsync(object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the document with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        IDocumentIdentifierResult ReplaceById(string id, object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the document with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> ReplaceByIdAsync(string id, object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        ///<summary>
        ///Partially updates the document without change tracking
        ///</summary>
        ///<param name="id">The document handle or key of document</param>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        ///<param name="rev">Conditionally replace a document based on revision id</param>
        ///<param name="policy">To control the update behavior in case there is a revision mismatch</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        IDocumentIdentifierResult UpdateById(string id, object document, bool? keepNull = null, bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        ///<summary>
        ///Partially updates the document without change tracking
        ///</summary>
        ///<param name="id">The document handle or key of document</param>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        ///<param name="rev">Conditionally replace a document based on revision id</param>
        ///<param name="policy">To control the update behavior in case there is a revision mismatch</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> UpdateByIdAsync(string id, object document, bool? keepNull = null, bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        ///<summary>
        ///Partially updates the document
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        ///<param name="policy">To control the update behavior in case there is a revision mismatch</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        IDocumentIdentifierResult Update(object document, bool? keepNull = null, bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);
        
        ///<summary>
        ///Partially updates the document
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> UpdateAsync(object document, bool? keepNull = null, bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the document without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        IDocumentIdentifierResult RemoveById(string id, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the document without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        Task<IDocumentIdentifierResult> RemoveByIdAsync(string id, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the document without change tracking
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        IDocumentIdentifierResult Remove(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the document
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        Task<IDocumentIdentifierResult> RemoveAsync(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Finds all documents matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        ICursor<T> ByExample<T>(object example, int? skip = null, int? limit = null, int? batchSize = null);

        /// <summary>
        /// Returns the first document matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <returns>A Document</returns>
        T FirstExample<T>(object example, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Returns the first document matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <returns>A Document</returns>
        Task<T> FirstExampleAsync<T>(object example, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Returns a random document
        /// </summary>
        /// <returns>A Document</returns>
        Task<T> AnyAsync<T>(Action<BaseResult> baseResult = null);

        /// <summary>
        /// Returns a random document
        /// </summary>
        /// <returns>A Document</returns>
        T Any<T>(Action<BaseResult> baseResult = null);

        /// <summary>
        /// Finds all documents within a given range
        /// </summary>
        /// <param name="attribute">The attribute to check</param>
        /// <param name="left">The lower bound</param>
        /// <param name="right">The upper bound</param>
        /// <param name="closed">If true, use interval including left and right, otherwise exclude right, but include left</param>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        ICursor<T> Range<T>(string attribute, object left, object right, bool? closed = false,
            int? skip = null, int? limit = null, int? batchSize = null);

        /// <summary>
        /// Finds documents near the given coordinate
        /// </summary>
        /// <param name="latitude">The latitude of the coordinate</param>
        /// <param name="longitude">The longitude of the coordinate</param>
        /// <param name="distance">If True, distances are returned in meters</param>
        /// <param name="distance">If True, distances are returned in meters</param>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        ICursor<T> Near<T>(double latitude, double longitude, string distance = null, string geo = null
            , int? skip = null, int? limit = null, int? batchSize = null);

        /// <summary>
        /// Finds documents within a given radius around the coordinate
        /// </summary>
        /// <param name="latitude">The latitude of the coordinate</param>
        /// <param name="longitude">The longitude of the coordinate</param>
        /// <param name="radius">The maximal radius</param>
        /// <param name="distance">If True, distances are returned in meters</param>
        /// <param name="geo">The identifier of the geo-index to use</param>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        ICursor<T> Within<T>(double latitude, double longitude, double radius, string distance = null, string geo = null
            , int? skip = null, int? limit = null, int? batchSize = null);

        /// <summary>
        /// Finds all documents from the collection that match the fulltext query
        /// </summary>
        /// <param name="attribute">The attribute that contains the texts</param>
        /// <param name="query">The fulltext query</param>
        /// <param name="index">The identifier of the fulltext-index to use</param>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        ICursor<T> Fulltext<T>(string attribute, string query, string index = null
            , int? skip = null, int? limit = null, int? batchSize = null);
    }

    public interface IArangoCollection<T>
    {
        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        T Document(string id, string ifMatchRev = null, string ifNoneMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        Task<T> DocumentAsync(string id, string ifMatchRev = null, string ifNoneMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Check if document exists
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="onDocumentLoad">Runs when document loaded</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>A Document</returns>
        Task<bool> ExistsAsync(string id, Action<T> onDocumentLoad = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Check if document exists
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="onDocumentLoad">Runs when document loaded</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>A Document</returns>
        bool Exists(string id, Action<T> onDocumentLoad = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Returns all documents of a collections
        /// </summary>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        ICursor<T> All(int? skip = null, int? limit = null, int? batchSize = null);

        /// <summary>
        /// Completely updates the document
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        IDocumentIdentifierResult Replace(object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> ReplaceAsync(object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the document with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        IDocumentIdentifierResult ReplaceById(string id, object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the document with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> ReplaceByIdAsync(string id, object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        ///<summary>
        ///Partially updates the document without change tracking
        ///</summary>
        ///<param name="id">The document handle or key of document</param>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        ///<param name="rev">Conditionally replace a document based on revision id</param>
        ///<param name="policy">To control the update behavior in case there is a revision mismatch</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        IDocumentIdentifierResult UpdateById(string id, object document, bool? keepNull = null, bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        ///<summary>
        ///Partially updates the document without change tracking
        ///</summary>
        ///<param name="id">The document handle or key of document</param>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        ///<param name="rev">Conditionally replace a document based on revision id</param>
        ///<param name="policy">To control the update behavior in case there is a revision mismatch</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> UpdateByIdAsync(string id, object document, bool? keepNull = null, bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        ///<summary>
        ///Partially updates the document
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        ///<param name="policy">To control the update behavior in case there is a revision mismatch</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        IDocumentIdentifierResult Update(object document, bool? keepNull = null, bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);


        ///<summary>
        ///Partially updates the document
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        ///<param name="policy">To control the update behavior in case there is a revision mismatch</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> UpdateAsync(object document, bool? keepNull = null, bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the document without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        IDocumentIdentifierResult RemoveById(string id, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the document without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        Task<IDocumentIdentifierResult> RemoveByIdAsync(string id, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the document without change tracking
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        IDocumentIdentifierResult Remove(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the document
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        Task<IDocumentIdentifierResult> RemoveAsync(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Finds all documents matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        ICursor<T> ByExample(object example, int? skip = null, int? limit = null, int? batchSize = null);

        /// <summary>
        /// Returns the first document matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <returns>A Document</returns>
        T FirstExample(object example, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Returns the first document matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <returns>A Document</returns>
        Task<T> FirstExampleAsync(object example, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Returns a random document
        /// </summary>
        /// <returns>A Document</returns>
        Task<T> AnyAsync(Action<BaseResult> baseResult = null);

        /// <summary>
        /// Returns a random document
        /// </summary>
        /// <returns>A Document</returns>
        T Any(Action<BaseResult> baseResult = null);

        /// <summary>
        /// Finds all documents within a given range
        /// </summary>
        /// <param name="attribute">The attribute to check</param>
        /// <param name="left">The lower bound</param>
        /// <param name="right">The upper bound</param>
        /// <param name="closed">If true, use interval including left and right, otherwise exclude right, but include left</param>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        ICursor<T> Range(Expression<Func<T, object>> attribute, object left, object right, bool? closed = false,
            int? skip = null, int? limit = null, int? batchSize = null);

        /// <summary>
        /// Finds documents near the given coordinate
        /// </summary>
        /// <param name="latitude">The latitude of the coordinate</param>
        /// <param name="longitude">The longitude of the coordinate</param>
        /// <param name="distance">If True, distances are returned in meters</param>
        /// <param name="distance">If True, distances are returned in meters</param>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        ICursor<T> Near(double latitude, double longitude, Expression<Func<T, object>> distance = null, string geo = null
            , int? skip = null, int? limit = null, int? batchSize = null);

        /// <summary>
        /// Finds documents within a given radius around the coordinate
        /// </summary>
        /// <param name="latitude">The latitude of the coordinate</param>
        /// <param name="longitude">The longitude of the coordinate</param>
        /// <param name="radius">The maximal radius</param>
        /// <param name="distance">If True, distances are returned in meters</param>
        /// <param name="geo">The identifier of the geo-index to use</param>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        ICursor<T> Within(double latitude, double longitude, double radius, Expression<Func<T, object>> distance = null, string geo = null
            , int? skip = null, int? limit = null, int? batchSize = null);

        /// <summary>
        /// Finds all documents from the collection that match the fulltext query
        /// </summary>
        /// <param name="attribute">The attribute that contains the texts</param>
        /// <param name="query">The fulltext query</param>
        /// <param name="index">The identifier of the fulltext-index to use</param>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        ICursor<T> Fulltext(Expression<Func<T, object>> attribute, string query, string index=null
            , int? skip = null, int? limit = null, int? batchSize = null);
    }
}
