using ArangoDB.Client.ChangeTracking;
using ArangoDB.Client.Cursor;
using ArangoDB.Client.Data;
using ArangoDB.Client.Http;
using ArangoDB.Client.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{

    public enum CollectionType
    {
        Document = 2,
        Edge = 3
    }

    public enum ReplacePolicy
    {
        Last = 0,
        Error = 1
    }

    public enum EdgeDirection
    {
        In = 0,
        Out = 1
    }

    public class ArangoCollection<T> : IDocumentCollection<T>, IEdgeCollection<T>
    {
        string collectionName;

        ArangoDatabase db;

        CollectionType collectionType { get; set; }

        /// <summary>
        /// Gets the document collection for a specific type
        /// </summary>
        /// <returns></returns>
        public ArangoCollection(ArangoDatabase db)
            : this(db, CollectionType.Document)
        {

        }

        /// <summary>
        /// Gets the collection by its type
        /// </summary>
        /// <returns></returns>
        public ArangoCollection(ArangoDatabase db, CollectionType type)
        {
            this.db = db;
            collectionName = db.Settings.Collection.ResolveCollectionName<T>();
            //property = GetCollectionProperty(typeof(T));
        }

        //public ArangoCollection(ArangoDatabase db, string name)
        //{
        //    this.db = db;
        //    property = new CollectionProperty { Name = name, Type = typeof(object) };
        //}

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public DocumentIdentifierResult Save(object document, bool? createCollection = null, bool? waitForSync = null)
        {
            return SaveAsync(document, createCollection, waitForSync).ResultSynchronizer();
        }

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<DocumentIdentifierResult> SaveAsync(object document, bool? createCollection = null, bool? waitForSync = null)
        {
            createCollection = Utils.ChangeIfNotSpecified<bool>(createCollection, db.Settings.CreateCollectionOnTheFly);
            waitForSync = Utils.ChangeIfNotSpecified<bool>(waitForSync, db.Settings.WaitForSync);

            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.Document,
                Method = HttpMethod.Post,
                Query = new Dictionary<string, string>()
            };

            command.Query.Add("collection", collectionName);
            command.Query.Add("createCollection", createCollection.ToString());
            command.Query.Add("waitForSync", waitForSync.ToString());

            var result = await command.RequestMergedResult<DocumentIdentifierResult>(document).ConfigureAwait(false);

            return result.Result;
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
        public DocumentIdentifierResult SaveEdge(string from, string to, object edgeDocument, bool? createCollection = null, bool? waitForSync = null)
        {
            return SaveEdgeAsync(from, to, edgeDocument, createCollection, waitForSync).ResultSynchronizer();
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
        public async Task<DocumentIdentifierResult> SaveEdgeAsync(string from, string to, object edgeDocument, bool? createCollection = null, bool? waitForSync = null)
        {
            createCollection = Utils.ChangeIfNotSpecified<bool>(createCollection, db.Settings.CreateCollectionOnTheFly);
            waitForSync = Utils.ChangeIfNotSpecified<bool>(waitForSync, db.Settings.WaitForSync);

            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.Edge,
                Method = HttpMethod.Post,
                Query = new Dictionary<string, string>()
            };

            command.Query.Add("collection", collectionName);
            command.Query.Add("createCollection", createCollection.ToString());
            command.Query.Add("waitForSync", waitForSync.ToString());
            command.Query.Add("from", from);
            command.Query.Add("to", to);

            var result = await command.RequestMergedResult<DocumentIdentifierResult>(edgeDocument).ConfigureAwait(false);

            return result.Result;
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
        public DocumentIdentifierResult Replace(string id, object document, string rev = null, ReplacePolicy? policy = null, bool? waitForSync = null)
        {
            return ReplaceAsync(id, document, rev, policy, waitForSync).ResultSynchronizer();
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
        public async Task<DocumentIdentifierResult> ReplaceAsync(string id, object document, string rev = null, ReplacePolicy? policy = null, bool? waitForSync = null)
        {
            string apiCommand = id.IndexOf("/") == -1 ? string.Format("{0}/{1}", collectionName, id) : id;

            policy = Utils.ChangeIfNotSpecified<ReplacePolicy>(policy, db.Settings.Document.ReplacePolicy);
            waitForSync = Utils.ChangeIfNotSpecified<bool>(waitForSync, db.Settings.WaitForSync);

            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.Document,
                Method = HttpMethod.Put,
                Query = new Dictionary<string, string>(),
                Command = apiCommand
            };

            command.Query.Add("waitForSync", waitForSync.ToString());
            if (rev != null)
                command.Query.Add("rev", rev);
            if (policy.HasValue)
                command.Query.Add("policy", policy.Value == ReplacePolicy.Last ? "last" : "error");

            var result = await command.RequestMergedResult<DocumentIdentifierResult>(document).ConfigureAwait(false);

            return result.Result;
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
        public DocumentIdentifierResult Update(string id, object document, bool? keepNull = null, bool? mergeObjects = null, string rev = null, ReplacePolicy? policy = null, bool? waitForSync = null)
        {
            return UpdateAsync(id, document, keepNull, mergeObjects, rev, policy, waitForSync).ResultSynchronizer();
        }

        /// <summary>
        /// Partially updates the document 
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<DocumentIdentifierResult> UpdateAsync(string id, object document, bool? keepNull = null, bool? mergeObjects = null, string rev = null, ReplacePolicy? policy = null, bool? waitForSync = null)
        {
            string apiCommand = id.IndexOf("/") == -1 ? string.Format("{0}/{1}", collectionName, id) : id;

            keepNull = Utils.ChangeIfNotSpecified<bool>(keepNull, db.Settings.Document.KeepNullAttributesOnUpdate);
            mergeObjects = Utils.ChangeIfNotSpecified<bool>(mergeObjects, db.Settings.Document.MergeObjectsOnUpdate);
            policy = Utils.ChangeIfNotSpecified<ReplacePolicy>(policy, db.Settings.Document.ReplacePolicy);
            waitForSync = Utils.ChangeIfNotSpecified<bool>(waitForSync, db.Settings.WaitForSync);

            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.Document,
                Method = new HttpMethod("PATCH"),
                Query = new Dictionary<string, string>(),
                Command = apiCommand
            };

            command.Query.Add("keepNull", keepNull.ToString());
            command.Query.Add("mergeObjects", mergeObjects.ToString());
            command.Query.Add("waitForSync", waitForSync.ToString());
            if (rev != null)
                command.Query.Add("rev", rev);
            if (policy.HasValue)
                command.Query.Add("policy", policy.Value == ReplacePolicy.Last ? "last" : "error");

            var result = await command.RequestMergedResult<DocumentIdentifierResult>(document).ConfigureAwait(false);

            return result.Result;
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
        public DocumentIdentifierResult Update(object document, bool? keepNull = null, bool? mergeObjects = null, ReplacePolicy? policy = null, bool? waitForSync = null)
        {
            return UpdateAsync(document, keepNull, mergeObjects, policy, waitForSync).ResultSynchronizer();
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
        public async Task<DocumentIdentifierResult> UpdateAsync(object document, bool? keepNull = null, bool? mergeObjects = null, ReplacePolicy? policy = null, bool? waitForSync = null)
        {
            DocumentContainer container = null;
            var changed = db.ChangeTracker.GetChanges(document,out container);

            policy = Utils.ChangeIfNotSpecified<ReplacePolicy>(policy, db.Settings.Document.ReplacePolicy);
            string rev = policy.HasValue && policy.Value == ReplacePolicy.Error ? container.Rev : null;

            if (changed.Count != 0)
            {
                var result = await UpdateAsync(container.Id, changed, keepNull, mergeObjects, rev, policy, waitForSync).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(result.Rev))
                    container.Rev = result.Rev;

                return result;
            }
            else
                return new DocumentIdentifierResult() { Id = container.Id, Key = container.Key, Rev = container.Rev };
            
        }

        private async Task<DocumentIdentifierResult> DocumentHeaderAsync(string id, string rev = null)
        {
            string apiCommand = id.IndexOf("/") == -1 ? string.Format("{0}/{1}", collectionName, id) : id;

            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.Document,
                Method = HttpMethod.Head,
                Query = new Dictionary<string, string>(),
                Command = apiCommand
            };

            if (rev != null)
                command.Query.Add("rev", rev);

            var result = await command.RequestMergedResult<DocumentIdentifierResult>().ConfigureAwait(false);

            return result.Result;
        }

        /// <summary>
        /// Deletes the document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public void Remove(string id, string rev = null, ReplacePolicy? policy = null, bool? waitForSync = null)
        {
            RemoveAsync(id, rev, policy, waitForSync).WaitSynchronizer();
        }

        /// <summary>
        /// Deletes the document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task RemoveAsync(string id, string rev = null, ReplacePolicy? policy = null, bool? waitForSync = null)
        {
            string apiCommand = id.IndexOf("/") == -1 ? string.Format("{0}/{1}", collectionName, id) : id;

            policy = Utils.ChangeIfNotSpecified<ReplacePolicy>(policy, db.Settings.Document.ReplacePolicy);
            waitForSync = Utils.ChangeIfNotSpecified<bool>(waitForSync, db.Settings.WaitForSync);

            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.Document,
                Method = HttpMethod.Delete,
                Query = new Dictionary<string, string>(),
                Command = apiCommand
            };

            command.Query.Add("waitForSync", waitForSync.ToString());
            if (rev != null)
                command.Query.Add("rev", rev);
            if (policy.HasValue)
                command.Query.Add("policy", policy.Value == ReplacePolicy.Last ? "last" : "error");

            var result = await command.RequestMergedResult<DocumentIdentifierResult>().ConfigureAwait(false);
        }

        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        public T Document(string id)
        {
            return DocumentAsync(id).ResultSynchronizer();
        }

        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        public async Task<T> DocumentAsync(string id)
        {
            string apiCommand = id.IndexOf("/") == -1 ? string.Format("{0}/{1}", collectionName, id) : id;

            var command = new HttpCommand(this.db)
            {
                Api = collectionType == CollectionType.Document ? CommandApi.Document : CommandApi.Edge,
                Method = HttpMethod.Get,
                Command = apiCommand,
                EnableChangeTracking = !db.Settings.DisableChangeTracking
            };

            var result = await command.RequestDistinctResult<T>().ConfigureAwait(false);

            return result.Result;
        }

        /// <summary>
        /// Read in or outbound edges
        /// </summary>
        /// <param name="vertexId">The document handle of the start vertex</param>
        /// <param name="direction">Selects in or out direction for edges. If not set, any edges are returned</param>
        /// <returns>Returns a list of edges starting or ending in the vertex identified by vertex document handle</returns>
        public List<T> Edges(string vertexId, EdgeDirection? direction = null)
        {
            return EdgesAsync(vertexId, direction).ResultSynchronizer();
        }

        /// <summary>
        /// Read in or outbound edges
        /// </summary>
        /// <param name="vertexId">The document handle of the start vertex</param>
        /// <param name="direction">Selects in or out direction for edges. If not set, any edges are returned</param>
        /// <returns>Returns a list of edges starting or ending in the vertex identified by vertex document handle</returns>
        public async Task<List<T>> EdgesAsync(string vertexId, EdgeDirection? direction = null)
        {
            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.AllEdges,
                Method = HttpMethod.Get,
                Query = new Dictionary<string, string>(),
                Command = collectionName
            };

            command.Query.Add("vertex", vertexId);

            if (direction.HasValue)
                command.Query.Add("direction", direction.Value == EdgeDirection.In ? "in" : "out");

            var result = await command.RequestGenericResult<List<T>, EdgesInheritedCommandResult<List<T>>>().ConfigureAwait(false);

            return result.Result;
        }

        /// <summary>
        /// Returns all documents of a collections
        /// </summary>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        public ICursor<T> All(int? skip = null, int? limit = null, int? batchSize = null)
        {
            batchSize = batchSize.HasValue ? batchSize.Value : db.Settings.Cursor.BatchSize;

            SimpleData data = new SimpleData
            {
                BatchSize = batchSize,
                Collection = collectionName,
                Limit = limit,
                Skip = skip
            };

            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.Simple,
                Method = HttpMethod.Put,
                Command = "all"
            };

            return command.CreateCursor<T>(data);
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
        public ICursor<T> Range(Expression<Func<T, object>> attribute, object left, object right, bool? closed = false,
            int? skip = null, int? limit = null, int? batchSize = null)
        {
            batchSize = batchSize.HasValue ? batchSize.Value : db.Settings.Cursor.BatchSize;

            SimpleData data = new SimpleData
            {
                BatchSize = batchSize,
                Collection = collectionName,
                Attribute = db.Settings.Collection.ResolvePropertyName(attribute),
                Left = left,
                Right = right,
                Closed = closed,
                Limit = limit,
                Skip = skip
            };

            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.Simple,
                Method = HttpMethod.Put,
                Command = "range"
            };

            return command.CreateCursor<T>(data);
        }

        /// <summary>
        /// Finds all documents matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        public ICursor<T> ByExample(object example, int? skip = null, int? limit = null, int? batchSize = null)
        {
            SimpleData data = new SimpleData
            {
                Example = example,
                BatchSize = batchSize,
                Collection = collectionName,
                Limit = limit,
                Skip = skip
            };

            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.Simple,
                Method = HttpMethod.Put,
                Command = "by-example"
            };

            return command.CreateCursor<T>(data);
        }

        /// <summary>
        /// Returns the first document matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <returns>A Document</returns>
        public T FirstExample(object example)
        {
            return FirstExampleAsync(example).ResultSynchronizer();
        }

        /// <summary>
        /// Returns the first document matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <returns>A Document</returns>
        public async Task<T> FirstExampleAsync(object example)
        {
            SimpleData data = new SimpleData
            {
                Example = example,
                Collection = collectionName
            };

            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.Simple,
                Command = "first-example",
                Method = HttpMethod.Put,
            };

            var result = await command.RequestGenericResult<T, DocumentInheritedCommandResult<T>>(data).ConfigureAwait(false);

            return result.Result;
        }

        /// <summary>
        /// Returns a random document
        /// </summary>
        /// <returns>A Document</returns>
        public T Any()
        {
            return AnyAsync().ResultSynchronizer();
        }

        /// <summary>
        /// Returns a random document
        /// </summary>
        /// <returns>A Document</returns>
        public async Task<T> AnyAsync()
        {
            SimpleData data = new SimpleData
            {
                Collection = collectionName
            };

            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.Simple,
                Command = "any",
                Method = HttpMethod.Put,
            };

            var result = await command.RequestGenericResult<T, DocumentInheritedCommandResult<T>>(data).ConfigureAwait(false);

            return result.Result;
        }
    }

    //public class CollectionProperty
    //{
    //    public Type Type { get; set; }
    //    public string Name { get; set; }
    //}
}
