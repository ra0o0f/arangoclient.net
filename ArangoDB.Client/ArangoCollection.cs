using ArangoDB.Client.ChangeTracking;
using ArangoDB.Client.Common.Newtonsoft.Json.Linq;
using ArangoDB.Client.Cursor;
using ArangoDB.Client.Data;
using ArangoDB.Client.Http;
using ArangoDB.Client.Serialization;
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

        IArangoDatabase db;

        CollectionType collectionType { get; set; }

        /// <summary>
        /// Gets the document collection for a specific type
        /// </summary>
        /// <returns></returns>
        public ArangoCollection(IArangoDatabase db)
            : this(db, CollectionType.Document)
        {

        }

        /// <summary>
        /// Gets the collection by its type
        /// </summary>
        /// <returns></returns>
        public ArangoCollection(IArangoDatabase db, CollectionType type)
        {
            this.db = db;
            collectionName = db.SharedSetting.Collection.ResolveCollectionName<T>();
            collectionType = type;
        }

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public DocumentIdentifierResult Insert(object document, bool? createCollection = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return InsertAsync(document, createCollection, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<DocumentIdentifierResult> InsertAsync(object document, bool? createCollection = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            createCollection = Utils.ChangeIfNotSpecified<bool>(createCollection, db.Setting.CreateCollectionOnTheFly);
            waitForSync = Utils.ChangeIfNotSpecified<bool>(waitForSync, db.Setting.WaitForSync);

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

            if (!result.BaseResult.Error)
            {
                if (db.Setting.DisableChangeTracking == false)
                    db.ChangeTracker.TrackChanges(document, result.Result);

                db.SharedSetting.IdentifierModifier.Modify(document, result.Result);
            }

            if (baseResult != null)
                baseResult(result.BaseResult);

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
        public DocumentIdentifierResult InsertEdge(string from, string to, object edgeDocument, bool? createCollection = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return InsertEdgeAsync(from, to, edgeDocument, createCollection, waitForSync, baseResult).ResultSynchronizer();
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
        public async Task<DocumentIdentifierResult> InsertEdgeAsync(string from, string to, object edgeDocument,
            bool? createCollection = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            createCollection = Utils.ChangeIfNotSpecified<bool>(createCollection, db.Setting.CreateCollectionOnTheFly);
            waitForSync = Utils.ChangeIfNotSpecified<bool>(waitForSync, db.Setting.WaitForSync);

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

            if (!result.BaseResult.Error)
            {
                if (!db.Setting.DisableChangeTracking)
                {
                    var container = db.ChangeTracker.TrackChanges(edgeDocument, result.Result);
                    if (container != null)
                    {
                        container.From = from;
                        container.To = to;
                    }
                }

                db.SharedSetting.IdentifierModifier.Modify(edgeDocument, result.Result, from, to);
            }

            if(baseResult!=null)
                baseResult(result.BaseResult);

            return result.Result;
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
        public DocumentIdentifierResult ReplaceById(string id, object document, string rev = null, ReplacePolicy? policy = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return ReplaceByIdAsync(id, document, rev, policy, waitForSync, baseResult).ResultSynchronizer();
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
        public async Task<DocumentIdentifierResult> ReplaceByIdAsync(string id, object document, string rev = null,
            ReplacePolicy? policy = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            string apiCommand = id.IndexOf("/") == -1 ? string.Format("{0}/{1}", collectionName, id) : id;

            policy = Utils.ChangeIfNotSpecified<ReplacePolicy>(policy, db.Setting.Document.ReplacePolicy);
            waitForSync = Utils.ChangeIfNotSpecified<bool>(waitForSync, db.Setting.WaitForSync);

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

            if(!result.BaseResult.Error)
                db.SharedSetting.IdentifierModifier.Modify(document, result.Result);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result;
        }

        /// <summary>
        /// Completely updates the document
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public DocumentIdentifierResult Replace(object document, ReplacePolicy? policy = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return ReplaceAsync(document, policy, waitForSync, baseResult).ResultSynchronizer();
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
        public async Task<DocumentIdentifierResult> ReplaceAsync(object document, ReplacePolicy? policy = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            if (db.Setting.DisableChangeTracking == true)
                throw new InvalidOperationException("Change tracking is disabled, use ReplaceById() instead");

            var container = db.ChangeTracker.FindDocumentInfo(document);
            policy = Utils.ChangeIfNotSpecified<ReplacePolicy>(policy, db.Setting.Document.ReplacePolicy);
            string rev = policy.HasValue && policy.Value == ReplacePolicy.Error ? container.Rev : null;

            var result = await ReplaceByIdAsync(container.Id, document, rev, policy, waitForSync, baseResult).ConfigureAwait(false);
            
            if (!result.Error)
            {
                container.Rev = result.Rev;
                container.Document = JObject.FromObject(document, new DocumentSerializer(db).CreateJsonSerializer());
                db.SharedSetting.IdentifierModifier.FindIdentifierMethodFor(document.GetType()).SetRevision(document, result.Rev);
            }

            return result;
        }

        ///<summary>
        ///Partially updates the document with no change tracking
        ///</summary>
        ///<param name="id">The document handle or key of document</param>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        ///<param name="rev">Conditionally replace a document based on revision id</param>
        ///<param name="policy">To control the update behavior in case there is a revision mismatch</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        public DocumentIdentifierResult UpdateById(string id, object document, bool? keepNull = null,
            bool? mergeObjects = null, string rev = null, ReplacePolicy? policy = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return UpdateByIdAsync(id, document, keepNull, mergeObjects, rev, policy, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Partially updates the document with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<DocumentIdentifierResult> UpdateByIdAsync(string id, object document, bool? keepNull = null,
            bool? mergeObjects = null, string rev = null, ReplacePolicy? policy = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            string apiCommand = id.IndexOf("/") == -1 ? string.Format("{0}/{1}", collectionName, id) : id;

            keepNull = Utils.ChangeIfNotSpecified<bool>(keepNull, db.Setting.Document.KeepNullAttributesOnUpdate);
            mergeObjects = Utils.ChangeIfNotSpecified<bool>(mergeObjects, db.Setting.Document.MergeObjectsOnUpdate);
            policy = Utils.ChangeIfNotSpecified<ReplacePolicy>(policy, db.Setting.Document.ReplacePolicy);
            waitForSync = Utils.ChangeIfNotSpecified<bool>(waitForSync, db.Setting.WaitForSync);

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

            if (baseResult != null)
                baseResult(result.BaseResult);

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
        public DocumentIdentifierResult Update(object document, bool? keepNull = null, bool? mergeObjects = null,
            ReplacePolicy? policy = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return UpdateAsync(document, keepNull, mergeObjects, policy, waitForSync, baseResult).ResultSynchronizer();
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
        public async Task<DocumentIdentifierResult> UpdateAsync(object document, bool? keepNull = null,
            bool? mergeObjects = null, ReplacePolicy? policy = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            if (db.Setting.DisableChangeTracking == true)
                throw new InvalidOperationException("Change tracking is disabled, use UpdateById() instead");


            DocumentContainer container = null;
            JObject jObject = null;
            var changed = db.ChangeTracker.GetChanges(document,out container, out jObject);

            policy = Utils.ChangeIfNotSpecified<ReplacePolicy>(policy, db.Setting.Document.ReplacePolicy);
            string rev = policy.HasValue && policy.Value == ReplacePolicy.Error ? container.Rev : null;

            if (changed.Count != 0)
            {
                var result = await UpdateByIdAsync(container.Id, changed, keepNull, mergeObjects, rev, policy, waitForSync, baseResult).ConfigureAwait(false);

                if (!result.Error)
                {
                    container.Rev = result.Rev;
                    container.Document = jObject;
                    db.SharedSetting.IdentifierModifier.FindIdentifierMethodFor(document.GetType()).SetRevision(document, result.Rev);
                }

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
        /// Deletes the document without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public DocumentIdentifierResult RemoveById(string id, string rev = null, ReplacePolicy? policy = null, bool? waitForSync = null
            , Action<BaseResult> baseResult = null)
        {
            return RemoveByIdAsync(id, rev, policy, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Deletes the document without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task<DocumentIdentifierResult> RemoveByIdAsync(string id, string rev = null, ReplacePolicy? policy = null,
            bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            string apiCommand = id.IndexOf("/") == -1 ? string.Format("{0}/{1}", collectionName, id) : id;

            policy = Utils.ChangeIfNotSpecified<ReplacePolicy>(policy, db.Setting.Document.ReplacePolicy);
            waitForSync = Utils.ChangeIfNotSpecified<bool>(waitForSync, db.Setting.WaitForSync);

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

            if(baseResult!=null)
                baseResult(result.BaseResult);

            return result.Result;
        }

        /// <summary>
        /// Deletes the document
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public DocumentIdentifierResult Remove(object document, ReplacePolicy? policy = null, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return RemoveAsync(document, policy, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Deletes the document
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task<DocumentIdentifierResult> RemoveAsync(object document, ReplacePolicy? policy = null,
            bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            if (db.Setting.DisableChangeTracking == true)
                throw new InvalidOperationException("Change tracking is disabled, use RemoveById() instead");

            var container = db.ChangeTracker.FindDocumentInfo(document);
            policy = Utils.ChangeIfNotSpecified<ReplacePolicy>(policy, db.Setting.Document.ReplacePolicy);
            string rev = policy.HasValue && policy.Value == ReplacePolicy.Error ? container.Rev : null;

            var result = await RemoveByIdAsync(container.Id, rev, policy, waitForSync, baseResult).ConfigureAwait(false);

            if (!result.Error)
                db.ChangeTracker.StopTrackChanges(document);

            return result;
        }

        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        public T Document(string id, Action<BaseResult> baseResult = null)
        {
            return DocumentAsync(id, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        public async Task<T> DocumentAsync(string id, Action<BaseResult> baseResult = null)
        {
            string apiCommand = id.IndexOf("/") == -1 ? string.Format("{0}/{1}", collectionName, id) : id;

            var command = new HttpCommand(this.db)
            {
                Api = collectionType == CollectionType.Document ? CommandApi.Document : CommandApi.Edge,
                Method = HttpMethod.Get,
                Command = apiCommand,
                EnableChangeTracking = db.Setting.DisableChangeTracking == false
            };

            var result = await command.RequestDistinctResult<T>().ConfigureAwait(false);

            if(baseResult != null)
                baseResult(result.BaseResult);

            return result.Result;
        }

        /// <summary>
        /// Read in or outbound edges
        /// </summary>
        /// <param name="vertexId">The document handle of the start vertex</param>
        /// <param name="direction">Selects in or out direction for edges. If not set, any edges are returned</param>
        /// <returns>Returns a list of edges starting or ending in the vertex identified by vertex document handle</returns>
        public List<T> Edges(string vertexId, EdgeDirection? direction = null, Action<BaseResult> baseResult = null)
        {
            return EdgesAsync(vertexId, direction, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Read in or outbound edges
        /// </summary>
        /// <param name="vertexId">The document handle of the start vertex</param>
        /// <param name="direction">Selects in or out direction for edges. If not set, any edges are returned</param>
        /// <returns>Returns a list of edges starting or ending in the vertex identified by vertex document handle</returns>
        public async Task<List<T>> EdgesAsync(string vertexId, EdgeDirection? direction = null, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.AllEdges,
                Method = HttpMethod.Get,
                Query = new Dictionary<string, string>(),
                Command = collectionName,
                EnableChangeTracking = db.Setting.DisableChangeTracking == false
            };

            command.Query.Add("vertex", vertexId);

            if (direction.HasValue)
                command.Query.Add("direction", direction.Value == EdgeDirection.In ? "in" : "out");

            var result = await command.RequestGenericListResult<T, EdgesInheritedCommandResult<List<T>>>().ConfigureAwait(false);

            if(baseResult != null)
                baseResult(result.BaseResult);

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
            batchSize = Utils.ChangeIfNotSpecified<int>(batchSize, db.Setting.Cursor.BatchSize);

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
            batchSize = Utils.ChangeIfNotSpecified<int>(batchSize, db.Setting.Cursor.BatchSize);

            SimpleData data = new SimpleData
            {
                BatchSize = batchSize,
                Collection = collectionName,
                Attribute = db.SharedSetting.Collection.ResolvePropertyName(attribute),
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
            batchSize = Utils.ChangeIfNotSpecified<int>(batchSize, db.Setting.Cursor.BatchSize);

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
        /// Finds documents near the given coordinate
        /// </summary>
        /// <param name="latitude">The latitude of the coordinate</param>
        /// <param name="longitude">The longitude of the coordinate</param>
        /// <param name="distance">If True, distances are returned in meters</param>
        /// <param name="geo">The identifier of the geo-index to use</param>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        public ICursor<T> Near(double latitude, double longitude, Expression<Func<T, object>> distance = null, string geo = null
            , int? skip = null, int? limit = null, int? batchSize = null)
        {
            batchSize = Utils.ChangeIfNotSpecified<int>(batchSize, db.Setting.Cursor.BatchSize);

            SimpleData data = new SimpleData
            {
                Latitude = latitude,
                Longitude = longitude,
                Distance = distance!=null ? db.SharedSetting.Collection.ResolvePropertyName(distance) : null,
                Geo = geo,
                BatchSize = batchSize,
                Collection = collectionName,
                Limit = limit,
                Skip = skip
            };

            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.Simple,
                Method = HttpMethod.Put,
                Command = "near"
            };

            return command.CreateCursor<T>(data);
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
        public ICursor<T> Within(double latitude, double longitude, double radius, Expression<Func<T, object>> distance = null, string geo = null
            , int? skip = null, int? limit = null, int? batchSize = null)
        {
            batchSize = Utils.ChangeIfNotSpecified<int>(batchSize, db.Setting.Cursor.BatchSize);

            SimpleData data = new SimpleData
            {
                Latitude = latitude,
                Longitude = longitude,
                Radius = radius,
                Distance = distance != null ? db.SharedSetting.Collection.ResolvePropertyName(distance) : null,
                Geo = geo,
                BatchSize = batchSize,
                Collection = collectionName,
                Limit = limit,
                Skip = skip
            };

            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.Simple,
                Method = HttpMethod.Put,
                Command = "within"
            };

            return command.CreateCursor<T>(data);
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
        public ICursor<T> Fulltext(Expression<Func<T, object>> attribute, string query, string index=null
            , int? skip = null, int? limit = null, int? batchSize = null)
        {
            batchSize = Utils.ChangeIfNotSpecified<int>(batchSize, db.Setting.Cursor.BatchSize);

            SimpleData data = new SimpleData
            {
                Attribute = db.SharedSetting.Collection.ResolvePropertyName(attribute),
                Query = query,
                Index = index,
                BatchSize = batchSize,
                Collection = collectionName,
                Limit = limit,
                Skip = skip
            };

            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.Simple,
                Method = HttpMethod.Put,
                Command = "fulltext"
            };

            return command.CreateCursor<T>(data);
        }

        /// <summary>
        /// Returns the first document matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <returns>A Document</returns>
        public T FirstExample(object example, Action<BaseResult> baseResult = null)
        {
            return FirstExampleAsync(example, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Returns the first document matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <returns>A Document</returns>
        public async Task<T> FirstExampleAsync(object example, Action<BaseResult> baseResult = null)
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
                EnableChangeTracking = db.Setting.DisableChangeTracking == false
            };

            var result = await command.RequestGenericSingleResult<T, DocumentInheritedCommandResult<T>>(data).ConfigureAwait(false);

            if(baseResult != null)
                baseResult(result.BaseResult);

            return result.Result;
        }

        /// <summary>
        /// Returns a random document
        /// </summary>
        /// <returns>A Document</returns>
        public T Any(Action<BaseResult> baseResult = null)
        {
            return AnyAsync(baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Returns a random document
        /// </summary>
        /// <returns>A Document</returns>
        public async Task<T> AnyAsync(Action<BaseResult> baseResult = null)
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
                EnableChangeTracking = db.Setting.DisableChangeTracking == false
            };

            var result = await command.RequestGenericSingleResult<T, DocumentInheritedCommandResult<T>>(data).ConfigureAwait(false);

            if(baseResult != null)
                baseResult(result.BaseResult);

            return result.Result;
        }
    }
}
