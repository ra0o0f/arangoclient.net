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
        /// Gets the edge collection for a specific type
        /// </summary>
        /// <returns></returns>
        public IEdgeCollection<T> EdgeCollection<T>()
        {
            return new ArangoCollection<T>(this, CollectionType.Edge);
        }

        /// <summary>
        /// Creates a new document in the collection for specific type
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<DocumentIdentifierResult> SaveAsync<T>(object document, bool? createCollection = null, bool? waitForSync = null)
        {
            return await Collection<T>().SaveAsync(document, createCollection, waitForSync).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new document in the collection for specific type
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public DocumentIdentifierResult Save<T>(object document, bool? createCollection = null, bool? waitForSync = null)
        {
            return Collection<T>().Save(document, createCollection, waitForSync);
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
        public DocumentIdentifierResult SaveEdge<T>(string from, string to, object edgeDocument, bool? createCollection = null, bool? waitForSync = null)
        {
            return EdgeCollection<T>().SaveEdge(from, to, edgeDocument, createCollection, waitForSync);
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
        public async Task<DocumentIdentifierResult> SaveEdgeAsync<T>(string from, string to, object edgeDocument, bool? createCollection = null, bool? waitForSync = null)
        {
            return await EdgeCollection<T>().SaveEdgeAsync(from, to, edgeDocument, createCollection, waitForSync).ConfigureAwait(false);
        }

        /// <summary>
        /// Read in or outbound edges
        /// </summary>
        /// <param name="vertexId">The document handle of the start vertex</param>
        /// <param name="direction">Selects in or out direction for edges. If not set, any edges are returned</param>
        /// <returns>Returns a list of edges starting or ending in the vertex identified by vertex document handle</returns>
        public List<T> Edges<T>(string vertexId, EdgeDirection? direction = null)
        {
            return EdgeCollection<T>().Edges(vertexId, direction);
        }

        /// <summary>
        /// Read in or outbound edges
        /// </summary>
        /// <param name="vertexId">The document handle of the start vertex</param>
        /// <param name="direction">Selects in or out direction for edges. If not set, any edges are returned</param>
        /// <returns>Returns a list of edges starting or ending in the vertex identified by vertex document handle</returns>
        public async Task<List<T>> EdgesAsync<T>(string vertexId, EdgeDirection? direction = null)
        {
            return await EdgeCollection<T>().EdgesAsync(vertexId, direction);
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
        public DocumentIdentifierResult Replace<T>(string id, object document, string rev = null, ReplacePolicy? policy = null, bool? waitForSync = null)
        {
            return Collection<T>().Replace(id, document, rev, policy, waitForSync);
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
        public async Task<DocumentIdentifierResult> ReplaceAsync<T>(string id, object document, string rev = null, ReplacePolicy? policy = null, bool? waitForSync = null)
        {
            return await Collection<T>().ReplaceAsync(id, document, rev, policy, waitForSync).ConfigureAwait(false);
        }

        ///<summary>
        ///Partially updates the document 
        ///</summary>
        ///<param name="id">The document handle or key of document</param>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        ///<param name="rev">Conditionally replace a document based on revision id</param>
        ///<param name="policy">To control the update behavior in case there is a revision mismatch</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        public DocumentIdentifierResult Update<T>(string id, object document, bool? keepNull = null, bool? mergeObjects = null, string rev = null, ReplacePolicy? policy = null, bool? waitForSync = null)
        {
            return Collection<T>().Update(id, document, keepNull, mergeObjects, rev, policy, waitForSync);
        }

        ///<summary>
        ///Partially updates the document 
        ///</summary>
        ///<param name="id">The document handle or key of document</param>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        ///<param name="rev">Conditionally replace a document based on revision id</param>
        ///<param name="policy">To control the update behavior in case there is a revision mismatch</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        public async Task<DocumentIdentifierResult> UpdateAsync<T>(string id, object document, bool? keepNull = null, bool? mergeObjects = null, string rev = null, ReplacePolicy? policy = null, bool? waitForSync = null)
        {
            return await Collection<T>().UpdateAsync(id, document,keepNull,mergeObjects, rev, policy, waitForSync).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public void Remove<T>(string id, string rev = null, ReplacePolicy? policy = null, bool? waitForSync = null)
        {
            Collection<T>().Remove(id, rev, policy, waitForSync);
        }

        /// <summary>
        /// Deletes the document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task RemoveAsync<T>(string id, string rev = null, ReplacePolicy? policy = null, bool? waitForSync = null)
        {
            await Collection<T>().RemoveAsync(id, rev, policy, waitForSync).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        public T Document<T>(string id)
        {
            return Collection<T>().Document(id);
        }

        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        public async Task<T> DocumentAsync<T>(string id)
        {
            return await Collection<T>().DocumentAsync(id).ConfigureAwait(false);
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
        public T FirstExample<T>(object example)
        {
            return Collection<T>().FirstExample(example);
        }

        /// <summary>
        /// Returns the first document matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <returns>A Document</returns>
        public async Task<T> FirstExampleAsync<T>(object example)
        {
            return await Collection<T>().FirstExampleAsync(example).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a random document
        /// </summary>
        /// <returns>A Document</returns>
        public T Any<T>()
        {
            return Collection<T>().Any();
        }

        /// <summary>
        /// Returns a random document
        /// </summary>
        /// <returns>A Document</returns>
        public async Task<T> AnyAsync<T>()
        {
            return await Collection<T>().AnyAsync().ConfigureAwait(false);
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
    }
}
