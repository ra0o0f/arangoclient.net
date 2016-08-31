using ArangoDB.Client.Collection;
using ArangoDB.Client.Data;
using System;
using System.Collections;
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
            return new ArangoCollection<T>(this, CollectionType.Document);
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
        /// Gets the collection for a specific type
        /// </summary>
        /// <returns></returns>
        public IDocumentCollection Collection(string collection)
        {
            return new ArangoCollection(this, CollectionType.Document, collection);
        }

        /// <summary>
        /// Gets the edge collection for a specific type
        /// </summary>
        /// <returns></returns>
        public IEdgeCollection EdgeCollection(string collection)
        {
            return new ArangoCollection(this, CollectionType.Edge, collection);
        }

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="documents">Representation of the documents</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Documents identifiers</returns>
        public async Task<List<IDocumentIdentifierResult>> InsertMultipleAsync<T>(IList documents, bool? waitForSync = null, Action<List<BaseResult>> baseResults = null)
        {
            return await Collection<T>().InsertMultipleAsync(documents, waitForSync, baseResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="documents">Representation of the documents</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Documents identifiers</returns>
        public List<IDocumentIdentifierResult> InsertMultiple<T>(IList documents, bool? waitForSync = null, Action<List<BaseResult>> baseResults = null)
        {
            return Collection<T>().InsertMultiple(documents, waitForSync, baseResults);
        }

        /// <summary>
        /// Creates a new document in the collection for specific type
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<IDocumentIdentifierResult> InsertAsync<T>(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().InsertAsync(document, waitForSync, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new document in the collection for specific type
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public IDocumentIdentifierResult Insert<T>(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().Insert(document, waitForSync, baseResult);
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
        public IDocumentIdentifierResult Replace<T>(object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().Replace(document, waitForSync, ignoreRevs, ifMatchRev, baseResult);
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
        public async Task<IDocumentIdentifierResult> ReplaceAsync<T>(object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().ReplaceAsync(document, waitForSync, ignoreRevs, ifMatchRev, baseResult).ConfigureAwait(false);
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
        public IDocumentIdentifierResult ReplaceById<T>(string id, object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().ReplaceById(id, document, waitForSync, ignoreRevs, ifMatchRev, baseResult);
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
        public async Task<IDocumentIdentifierResult> ReplaceByIdAsync<T>(string id, object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().ReplaceByIdAsync(id, document, waitForSync, ignoreRevs, ifMatchRev, baseResult).ConfigureAwait(false);
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
        public IDocumentIdentifierResult UpdateById<T>(string id, object document, bool? keepNull = null, bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().UpdateById(id, document, keepNull, mergeObjects, waitForSync, ignoreRevs, ifMatchRev, baseResult);
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
        public async Task<IDocumentIdentifierResult> UpdateByIdAsync<T>(string id, object document, bool? keepNull = null, bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().UpdateByIdAsync(id, document, keepNull, mergeObjects, waitForSync, ignoreRevs, ifMatchRev, baseResult).ConfigureAwait(false);
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
        public IDocumentIdentifierResult Update<T>(object document, bool? keepNull = null, bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().Update(document, keepNull, mergeObjects, waitForSync, ignoreRevs, ifMatchRev, baseResult);
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
        public async Task<IDocumentIdentifierResult> UpdateAsync<T>(object document, bool? keepNull = null, bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().UpdateAsync(document, keepNull, mergeObjects, waitForSync, ignoreRevs, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the document
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public IDocumentIdentifierResult Remove<T>(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().Remove(document, waitForSync, ifMatchRev, baseResult);
        }

        /// <summary>
        /// Deletes the document
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task<IDocumentIdentifierResult> RemoveAsync<T>(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().RemoveAsync(document, waitForSync, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the document without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public IDocumentIdentifierResult RemoveById<T>(string id, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().RemoveById(id, waitForSync, ifMatchRev, baseResult);
        }

        /// <summary>
        /// Deletes the document without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task<IDocumentIdentifierResult> RemoveByIdAsync<T>(string id, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().RemoveByIdAsync(id, waitForSync, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        public T Document<T>(string id, string ifMatchRev = null, string ifNoneMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().Document(id, ifMatchRev, ifNoneMatchRev, baseResult);
        }

        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        public async Task<T> DocumentAsync<T>(string id, string ifMatchRev = null, string ifNoneMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().DocumentAsync(id, ifMatchRev, ifNoneMatchRev, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Check if document exists
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="onDocumentLoad">Runs when document loaded</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>A Document</returns>
        public bool Exists<T>(string id, Action<T> onDocumentLoad = null, Action<BaseResult> baseResult = null)
        {
            return Collection<T>().Exists(id, onDocumentLoad, baseResult);
        }

        /// <summary>
        /// Check if document exists
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="onDocumentLoad">Runs when document loaded</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>A Document</returns>
        public async Task<bool> ExistsAsync<T>(string id, Action<T> onDocumentLoad = null, Action<BaseResult> baseResult = null)
        {
            return await Collection<T>().ExistsAsync(id, onDocumentLoad, baseResult).ConfigureAwait(false);
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
        public ICursor<T> Fulltext<T>(Expression<Func<T, object>> attribute, string query, string index = null
            , int? skip = null, int? limit = null, int? batchSize = null)
        {
            return Collection<T>().Fulltext(attribute, query, index, skip, limit, batchSize);
        }
    }
}
