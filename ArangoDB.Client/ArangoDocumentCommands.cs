using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public partial class ArangoDatabase
    {
        /// <summary>
        /// Gets the collection for a specific type
        /// </summary>
        /// <returns></returns>
        public IDocumentCollection<T> Collection<T>()
        {
            return new ArangoCollection<T>(this);
        }

        /// <summary>
        /// Gets the named collection for a specific type
        /// </summary>
        /// <returns></returns>
        public IDocumentCollection<T> Collection<T>(string collectionName)
        {
            return new ArangoCollection<T>(this, collectionName);
        }

        /// <summary>
        /// Gets the edge collection for a specific type
        /// </summary>
        /// <returns></returns>
        public IEdgeCollection<T> EdgeCollection<T>()
        {
            return new ArangoCollection<T>(this, CollectionType.Edge);
        }

        /// <summary>
        /// Gets the named edge collection for a specific type
        /// </summary>
        /// <returns></returns>
        public IEdgeCollection<T> EdgeCollection<T>(string collectionName)
        {
            return new ArangoCollection<T>(this, CollectionType.Edge, collectionName);
        }

        /// <summary>
        /// Creates a new document in the collection for specific type
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<DocumentIdentifierResult> InsertAsync<T>(object document, bool? createCollection = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().InsertAsync(document, createCollection, waitForSync, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new document in the collection for specific type
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public DocumentIdentifierResult Insert<T>(object document, bool? createCollection = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().Insert(document, createCollection, waitForSync, baseResult);
        }

        /// <summary>
        /// Creates a new edge document in the collection
        /// </summary>
        /// <param name="from">The document handle or key of the start point</param>
        /// <param name="to"> The document handle or key of the end point</param>
        /// <param name="edgeDocument">Representation of the edge document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public DocumentIdentifierResult InsertEdge<T>(string from, string to, object edgeDocument, bool? createCollection = null,
            bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return EdgeCollection<T>().InsertEdge(from, to, edgeDocument, createCollection, waitForSync, baseResult);
        }

        /// <summary>
        /// Creates a new edge document in the collection
        /// </summary>
        /// <param name="from">The document handle or key of the start point</param>
        /// <param name="to"> The document handle or key of the end point</param>
        /// <param name="edgeDocument">Representation of the edge document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<DocumentIdentifierResult> InsertEdgeAsync<T>(string from, string to, object edgeDocument,
            bool? createCollection = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return await EdgeCollection<T>().InsertEdgeAsync(from, to, edgeDocument, createCollection, waitForSync, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Read in or outbound edges
        /// </summary>
        /// <param name="vertexId">The document handle of the start vertex</param>
        /// <param name="direction">Selects in or out direction for edges. If not set, any edges are returned</param>
        /// <returns>Returns a list of edges starting or ending in the vertex identified by vertex document handle</returns>
        public List<T> Edges<T>(string vertexId, EdgeDirection? direction = null, Action<BaseResult> baseResult = null)
        {
            return EdgeCollection<T>().Edges(vertexId, direction, baseResult);
        }

        /// <summary>
        /// Read in or outbound edges
        /// </summary>
        /// <param name="vertexId">The document handle of the start vertex</param>
        /// <param name="direction">Selects in or out direction for edges. If not set, any edges are returned</param>
        /// <returns>Returns a list of edges starting or ending in the vertex identified by vertex document handle</returns>
        public async Task<List<T>> EdgesAsync<T>(string vertexId, EdgeDirection? direction = null, Action<BaseResult> baseResult = null)
        {
            return await EdgeCollection<T>().EdgesAsync(vertexId, direction, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Completely updates the document
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public DocumentIdentifierResult Replace<T>(object document, ReplacePolicy? policy = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().Replace(document, policy, waitForSync, baseResult);
        }

        /// <summary>
        /// Completely updates the document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<DocumentIdentifierResult> ReplaceAsync<T>(object document, ReplacePolicy? policy = null,
            bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().ReplaceAsync(document, policy, waitForSync, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Completely updates the document with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public DocumentIdentifierResult ReplaceById<T>(string id, object document, string rev = null,
            ReplacePolicy? policy = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().ReplaceById(id, document, rev, policy, waitForSync, baseResult);
        }

        /// <summary>
        /// Completely updates the document with no change tacking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<DocumentIdentifierResult> ReplaceByIdAsync<T>(string id, object document, string rev = null,
            ReplacePolicy? policy = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().ReplaceByIdAsync(id, document, rev, policy, waitForSync, baseResult).ConfigureAwait(false);
        }

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
        public DocumentIdentifierResult UpdateById<T>(string id, object document, bool? keepNull = null, bool? mergeObjects = null,
            string rev = null, ReplacePolicy? policy = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().UpdateById(id, document, keepNull, mergeObjects, rev, policy, waitForSync, baseResult);
        }

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
        public async Task<DocumentIdentifierResult> UpdateByIdAsync<T>(string id, object document, bool? keepNull = null, bool? mergeObjects = null,
            string rev = null, ReplacePolicy? policy = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().UpdateByIdAsync(id, document,keepNull,mergeObjects, rev, policy, waitForSync, baseResult).ConfigureAwait(false);
        }

        ///<summary>
        ///Partially updates the document 
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        ///<param name="policy">To control the update behavior in case there is a revision mismatch</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        public DocumentIdentifierResult Update<T>(object document, bool? keepNull = null, bool? mergeObjects = null,
            ReplacePolicy? policy = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().Update(document, keepNull, mergeObjects, policy, waitForSync,baseResult);
        }

        ///<summary>
        ///Partially updates the document 
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        ///<param name="policy">To control the update behavior in case there is a revision mismatch</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        public async Task<DocumentIdentifierResult> UpdateAsync<T>(object document, bool? keepNull = null, bool? mergeObjects = null,
            ReplacePolicy? policy = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().UpdateAsync(document, keepNull, mergeObjects, policy, waitForSync, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the document
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public DocumentIdentifierResult Remove<T>(object document, ReplacePolicy? policy = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().Remove(document, policy, waitForSync, baseResult);
        }

        /// <summary>
        /// Deletes the document
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task<DocumentIdentifierResult> RemoveAsync<T>(object document, ReplacePolicy? policy = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().RemoveAsync(document, policy, waitForSync, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the document without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public DocumentIdentifierResult RemoveById<T>(string id, string rev = null, ReplacePolicy? policy = null,
            bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().RemoveById(id, rev, policy, waitForSync, baseResult);
        }

        /// <summary>
        /// Deletes the document without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task<DocumentIdentifierResult> RemoveByIdAsync<T>(string id, string rev = null, ReplacePolicy? policy = null,
            bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().RemoveByIdAsync(id, rev, policy, waitForSync, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        public T Document<T>(string id, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().Document(id, baseResult);
        }

        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        public async Task<T> DocumentAsync<T>(string id, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().DocumentAsync(id, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns all documents of a collections
        /// </summary>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        public ICursor<T> All<T>(int? skip = null, int? limit = null, int? batchSize = null)
        {
            return Collection<T>().All(skip, limit, batchSize);
        }

        /// <summary>
        /// Finds all documents matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        public ICursor<T> ByExample<T>(object example, int? skip = null, int? limit = null, int? batchSize = null)
        {
            return Collection<T>().ByExample(example, skip, limit, batchSize);
        }

        /// <summary>
        /// Returns the first document matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <returns>A Document</returns>
        public T FirstExample<T>(object example, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().FirstExample(example, baseResult);
        }

        /// <summary>
        /// Returns the first document matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <returns>A Document</returns>
        public async Task<T> FirstExampleAsync<T>(object example, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().FirstExampleAsync(example, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a random document
        /// </summary>
        /// <returns>A Document</returns>
        public T Any<T>(Action<BaseResult> baseResult = null)
        {
            return Collection<T>().Any(baseResult);
        }

        /// <summary>
        /// Returns a random document
        /// </summary>
        /// <returns>A Document</returns>
        public async Task<T> AnyAsync<T>(Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().AnyAsync(baseResult).ConfigureAwait(false);
        }

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
        public ICursor<T> Range<T>(Expression<Func<T, object>> attribute, object left, object right, bool? closed = false,
            int? skip = null, int? limit = null, int? batchSize = null)
        {
            return Collection<T>().Range(attribute, left, right, closed, skip, limit, batchSize);
        }

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
        public ICursor<T> Near<T>(double latitude, double longitude, Expression<Func<T, object>> distance = null, string geo = null
            , int? skip = null, int? limit = null, int? batchSize = null)
        {
            return Collection<T>().Near(latitude, longitude, distance, geo, skip, limit, batchSize);
        }

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
        public ICursor<T> Within<T>(double latitude, double longitude, double radius, Expression<Func<T, object>> distance = null, string geo = null
            , int? skip = null, int? limit = null, int? batchSize = null)
        {
            return Collection<T>().Within(latitude, longitude, radius, distance, geo, skip, limit, batchSize);
        }

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
        public ICursor<T> Fulltext<T>(Expression<Func<T, object>> attribute, string query, string index=null
            , int? skip = null, int? limit = null, int? batchSize = null)
        {
            return Collection<T>().Fulltext(attribute, query, index, skip, limit, batchSize);
        }

        /// <summary>
        /// Bulk imports documents into the collection. Note that change tracking is disabled for bulk imports.
        /// </summary>
        /// <param name="documents">Representation of the set of documents</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <param name="complete">Make the entire import fail if any of the uploaded documents is invalid and cannot be imported</param>
        /// <param name="details">Make the import API return details about documents that could not be imported</param>
        /// <param name="importMethod">Method that will be used to send the documents to the server</param>
        /// <returns>Summary of import results and details of failed documents if requested</returns>
        public BulkImportResult Import<T>(IEnumerable<object> documents, bool? createCollection = null, bool? waitForSync = null, bool? complete = false, bool? details = false, BulkImportMethod? importMethod = null, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().Import(documents, createCollection, waitForSync, complete, details, importMethod, baseResult);
        }

        /// <summary>
        /// Bulk imports documents into the collection. Note that change tracking is disabled for bulk imports.
        /// </summary>
        /// <param name="documents">Representation of the set of documents</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <param name="complete">Make the entire import fail if any of the uploaded documents is invalid and cannot be imported</param>
        /// <param name="details">Make the import API return details about documents that could not be imported</param>
        /// <param name="importMethod">Method that will be used to send the documents to the server</param>
        /// <returns>Summary of import results and details of failed documents if requested</returns>
        public async Task<BulkImportResult> ImportAsync<T>(IEnumerable<object> documents, bool? createCollection = null, bool? waitForSync = null, bool? complete = false, bool? details = false, BulkImportMethod? importMethod = null, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().ImportAsync(documents, createCollection, waitForSync, complete, details, importMethod, baseResult);
        }

        /// <summary>
        /// Removes all documents from the collection, but leaves the indexes intact
        /// </summary>
        /// <returns>Collection information</returns>
        public CollectionInformationResult Truncate<T>()
        {
            return Collection<T>().Truncate();
        }

        /// <summary>
        /// Removes all documents from the collection, but leaves the indexes intact
        /// </summary>
        /// <returns>Collection information</returns>
        public async Task<CollectionInformationResult> TruncateAsync<T>()
        {
            return await Collection<T>().TruncateAsync();
        }

        /// <summary>
        /// Returns number of documents in a collection
        /// </summary>
        /// <returns>Number of documents</returns>
        public int DocumentCount<T>()
        {
            return Collection<T>().DocumentCount();
        }

        /// <summary>
        /// Returns number of documents in a collection
        /// </summary>
        /// <returns>Number of documents</returns>
        public async Task<int> DocumentCountAsync<T>()
        {
            return await Collection<T>().DocumentCountAsync();
        }
    }
}
