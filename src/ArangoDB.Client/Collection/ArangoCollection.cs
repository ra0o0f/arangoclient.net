using ArangoDB.Client.ChangeTracking;
using Newtonsoft.Json.Linq;
using ArangoDB.Client.Cursor;
using ArangoDB.Client.Data;
using ArangoDB.Client.Http;
using ArangoDB.Client.Serialization;
using ArangoDB.Client.Utility;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Collection
{
    public class ArangoCollection : IDocumentCollection, IEdgeCollection
    {
        string collectionName;

        IArangoDatabase db;

        CollectionType collectionType { get; set; }

        /// <summary>
        /// Gets the collection for a specific type
        /// </summary>
        /// <returns></returns>
        public ArangoCollection(IArangoDatabase db, CollectionType type, string collectionName)
        {
            this.db = db;
            this.collectionName = collectionName;
            collectionType = type;
        }

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="documents">Representation of the documents</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Documents identifiers</returns>
        public List<IDocumentIdentifierResult> InsertMultiple(IList documents, bool? waitForSync = null, Action<List<BaseResult>> baseResults = null)
        {
            return InsertMultipleAsync(documents, waitForSync, baseResults).ResultSynchronizer();
        }

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="documents">Representation of the documents</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Documents identifiers</returns>
        public async Task<List<IDocumentIdentifierResult>> InsertMultipleAsync(IList documents, bool? waitForSync = null, Action<List<BaseResult>> baseResults = null)
        {
            waitForSync = waitForSync ?? db.Setting.WaitForSync;

            var command = new HttpCommand(db)
            {
                Api = CommandApi.Document,
                Method = HttpMethod.Post,
                Query = new Dictionary<string, string>(),
                Command = collectionName
            };

            command.Query.Add("waitForSync", waitForSync.ToString());

            var results = await command.RequestMultipleMergedResult<DocumentIdentifierBaseResult>(documents).ConfigureAwait(false);

            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].BaseResult.HasError() == false)
                {
                    if (db.Setting.DisableChangeTracking == false)
                        db.ChangeTracker.TrackChanges(documents[i], results[i].Result);

                    db.SharedSetting.IdentifierModifier.Modify(documents[i], results[i].Result);
                }
            }

            if (baseResults != null)
                baseResults(results.Select(x => x.BaseResult).ToList());

            return results.Select(x => x.Result as IDocumentIdentifierResult).ToList();
        }

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public IDocumentIdentifierResult Insert(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return InsertAsync(document, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<IDocumentIdentifierResult> InsertAsync(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            waitForSync = waitForSync ?? db.Setting.WaitForSync;

            var command = new HttpCommand(db)
            {
                Api = CommandApi.Document,
                Method = HttpMethod.Post,
                Query = new Dictionary<string, string>(),
                Command = collectionName
            };

            command.Query.Add("waitForSync", waitForSync.ToString());

            var result = await command.RequestMergedResult<DocumentIdentifierBaseResult>(document).ConfigureAwait(false);

            if (result.BaseResult.HasError() == false)
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
        /// Completely updates the document with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public IDocumentIdentifierResult ReplaceById(string id, object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return ReplaceByIdAsync(id, document, waitForSync, ignoreRevs, ifMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Completely updates the document with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<IDocumentIdentifierResult> ReplaceByIdAsync(string id, object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            string apiCommand = id.IndexOf("/") == -1 ? string.Format("{0}/{1}", collectionName, id) : id;
            
            waitForSync = waitForSync ?? db.Setting.WaitForSync;

            var command = new HttpCommand(db)
            {
                Api = CommandApi.Document,
                Method = HttpMethod.Put,
                Query = new Dictionary<string, string>(),
                Command = apiCommand,
                Headers = new Dictionary<string, string>()
            };

            if (string.IsNullOrEmpty(ifMatchRev) == false)
                command.Headers.Add("If-Match", ifMatchRev);

            command.Query.Add("waitForSync", waitForSync.ToString());
            if (ignoreRevs.HasValue)
                command.Query.Add("ignoreRevs", ignoreRevs.Value.ToString());

            var result = await command.RequestMergedResult<DocumentIdentifierBaseResult>(document).ConfigureAwait(false);

            if (result.BaseResult.HasError() == false)
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
        public IDocumentIdentifierResult Replace(object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return ReplaceAsync(document, waitForSync, ignoreRevs, ifMatchRev, baseResult).ResultSynchronizer();
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
        public async Task<IDocumentIdentifierResult> ReplaceAsync(object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            if (db.Setting.DisableChangeTracking == true)
                throw new InvalidOperationException("Change tracking is disabled, use ReplaceById() instead");

            var container = db.ChangeTracker.FindDocumentInfo(document);

            BaseResult bResult = null;

            var result = await ReplaceByIdAsync(container.Id, document, waitForSync, ignoreRevs, ifMatchRev, (b) => bResult = b).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(bResult);

            if (bResult.HasError() == false)
            {
                container.Rev = result.Rev;
                container.Document = JObject.FromObject(document, new DocumentSerializer(db).CreateJsonSerializer());
                db.SharedSetting.IdentifierModifier.FindIdentifierMethodFor(document.GetType()).SetRevision(document, result.Rev);
            }

            return result;
        }

        ///<summary>
        /// Partially updates the document with no change tracking
        ///</summary>
        ///<param name="id">The document handle or key of document</param>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        ///<param name="rev">Conditionally replace a document based on revision id</param>
        ///<param name="policy">To control the update behavior in case there is a revision mismatch</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        public IDocumentIdentifierResult UpdateById(string id, object document, bool? keepNull = null,
            bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return UpdateByIdAsync(id, document, keepNull, mergeObjects, waitForSync, ignoreRevs, ifMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Partially updates the document with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<IDocumentIdentifierResult> UpdateByIdAsync(string id, object document, bool? keepNull = null,
            bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            string apiCommand = id.IndexOf("/") == -1 ? string.Format("{0}/{1}", collectionName, id) : id;

            keepNull = keepNull ?? db.Setting.Document.KeepNullAttributesOnUpdate;
            mergeObjects = mergeObjects ?? db.Setting.Document.MergeObjectsOnUpdate;
            waitForSync = waitForSync ?? db.Setting.WaitForSync;

            var command = new HttpCommand(db)
            {
                Api = CommandApi.Document,
                Method = new HttpMethod("PATCH"),
                Query = new Dictionary<string, string>(),
                Command = apiCommand,
                Headers = new Dictionary<string, string>()
            };

            if (string.IsNullOrEmpty(ifMatchRev) == false)
                command.Headers.Add("If-Match", ifMatchRev);

            command.Query.Add("keepNull", keepNull.ToString());
            command.Query.Add("mergeObjects", mergeObjects.ToString());
            command.Query.Add("waitForSync", waitForSync.ToString());
            if (ignoreRevs.HasValue)
                command.Query.Add("ignoreRevs", ignoreRevs.Value.ToString());

            var result = await command.RequestMergedResult<DocumentIdentifierBaseResult>(document).ConfigureAwait(false);

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
        public IDocumentIdentifierResult Update(object document, bool? keepNull = null, bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return UpdateAsync(document, keepNull, mergeObjects, waitForSync, ignoreRevs, ifMatchRev, baseResult).ResultSynchronizer();
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
        public async Task<IDocumentIdentifierResult> UpdateAsync(object document, bool? keepNull = null,
            bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            if (db.Setting.DisableChangeTracking == true)
                throw new InvalidOperationException("Change tracking is disabled, use UpdateById() instead");


            DocumentContainer container = null;
            JObject jObject = null;
            var changed = db.ChangeTracker.GetChanges(document, out container, out jObject);
            
            if (changed.Count != 0)
            {
                BaseResult bResult = null;

                var result = await UpdateByIdAsync(container.Id, changed, keepNull, mergeObjects, waitForSync, ignoreRevs, ifMatchRev, (b) => bResult = b).ConfigureAwait(false);

                if (baseResult != null)
                    baseResult(bResult);

                if (bResult.HasError() == false)
                {
                    container.Rev = result.Rev;
                    container.Document = jObject;
                    db.SharedSetting.IdentifierModifier.FindIdentifierMethodFor(document.GetType()).SetRevision(document, result.Rev);
                }

                return result;
            }
            else
                return new DocumentIdentifierWithoutBaseResult() { Id = container.Id, Key = container.Key, Rev = container.Rev };

        }

        private async Task<IDocumentIdentifierResult> DocumentHeaderAsync(string id, string rev = null)
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

            var result = await command.RequestMergedResult<DocumentIdentifierBaseResult>().ConfigureAwait(false);

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
        public IDocumentIdentifierResult RemoveById(string id, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return RemoveByIdAsync(id, waitForSync, ifMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Deletes the document without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task<IDocumentIdentifierResult> RemoveByIdAsync(string id,
            bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            string apiCommand = id.IndexOf("/") == -1 ? string.Format("{0}/{1}", collectionName, id) : id;
            
            waitForSync = waitForSync ?? db.Setting.WaitForSync;

            var command = new HttpCommand(db)
            {
                Api = CommandApi.Document,
                Method = HttpMethod.Delete,
                Query = new Dictionary<string, string>(),
                Command = apiCommand,
                Headers = new Dictionary<string, string>()
            };

            if (string.IsNullOrEmpty(ifMatchRev) == false)
                command.Headers.Add("If-Match", ifMatchRev);

            command.Query.Add("waitForSync", waitForSync.ToString());

            var result = await command.RequestMergedResult<DocumentIdentifierBaseResult>().ConfigureAwait(false);

            if (baseResult != null)
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
        public IDocumentIdentifierResult Remove(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return RemoveAsync(document, waitForSync, ifMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Deletes the document
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task<IDocumentIdentifierResult> RemoveAsync(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            if (db.Setting.DisableChangeTracking == true)
                throw new InvalidOperationException("Change tracking is disabled, use RemoveById() instead");

            var container = db.ChangeTracker.FindDocumentInfo(document);

            BaseResult bResult = null;

            var result = await RemoveByIdAsync(container.Id, waitForSync, ifMatchRev, (b) => bResult = b).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(bResult);

            if (bResult.HasError() == false)
                db.ChangeTracker.StopTrackChanges(document);

            return result;
        }

        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        public T Document<T>(string id, string ifMatchRev = null, string ifNoneMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return DocumentAsync<T>(id, ifMatchRev, ifNoneMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        public async Task<T> DocumentAsync<T>(string id, string ifMatchRev = null, string ifNoneMatchRev = null, Action<BaseResult> baseResult = null)
        {
            string apiCommand = id.IndexOf("/") == -1 ? string.Format("{0}/{1}", collectionName, id) : id;

            var command = new HttpCommand(db)
            {
                Api = CommandApi.Document,
                Method = HttpMethod.Get,
                Command = apiCommand,
                EnableChangeTracking = db.Setting.DisableChangeTracking == false,
                Headers = new Dictionary<string, string>()
            };

            if (string.IsNullOrEmpty(ifMatchRev) == false)
                command.Headers.Add("If-Match", ifMatchRev);
            else if (string.IsNullOrEmpty(ifNoneMatchRev) == false)
                command.Headers.Add("If-None-Match", ifNoneMatchRev);

            var result = await command.RequestDistinctResult<T>(throwForServerErrors: false).ConfigureAwait(false);

            if (db.Setting.Document.ThrowIfDocumentDoesNotExists ||
                (result.BaseResult.HasError() && result.BaseResult.ErrorNum != 1202))
                new BaseResultAnalyzer(db).Throw(result.BaseResult);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result;
        }

        /// <summary>
        /// Check if document exists
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="onDocumentLoad">Runs when document loaded</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>A Document</returns>
        public bool Exists<T>(string id, Action<T> onDocumentLoad, Action<BaseResult> baseResult = null)
        {
            return ExistsAsync<T>(id, onDocumentLoad, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Check if document exists
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="onDocumentLoad">Runs when document loaded</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>A Document</returns>
        public async Task<bool> ExistsAsync<T>(string id, Action<T> onDocumentLoad, Action<BaseResult> baseResult = null)
        {
            var defaultThrowForServerErrors = db.Setting.ThrowForServerErrors;
            db.Setting.ThrowForServerErrors = false;

            bool exists = false;
            var document = await DocumentAsync<T>(id, baseResult: (b) =>
             {
                 if (b.HasError() && b.ErrorNum != 1202)
                     new BaseResultAnalyzer(db).Throw(b);

                 exists = b.HasError() == false;

                 if (baseResult != null)
                     baseResult(b);

             }).ConfigureAwait(false);

            if (onDocumentLoad != null)
                onDocumentLoad(document);

            db.Setting.ThrowForServerErrors = defaultThrowForServerErrors;

            return exists;
        }

        /// <summary>
        /// Check if document exists
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>A Document</returns>
        public bool Exists(string id, Action<BaseResult> baseResult = null)
        {
            return ExistsAsync(id, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Check if document exists
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>A Document</returns>
        public async Task<bool> ExistsAsync(string id, Action<BaseResult> baseResult = null)
        {
            return await ExistsAsync<dynamic>(id, null, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Read in or outbound edges
        /// </summary>
        /// <param name="vertexId">The document handle of the start vertex</param>
        /// <param name="direction">Selects in or out direction for edges. If not set, any edges are returned</param>
        /// <returns>Returns a list of edges starting or ending in the vertex identified by vertex document handle</returns>
        public List<T> Edges<T>(string vertexId, EdgeDirection? direction = null, Action<BaseResult> baseResult = null)
        {
            return EdgesAsync<T>(vertexId, direction, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Read in or outbound edges
        /// </summary>
        /// <param name="vertexId">The document handle of the start vertex</param>
        /// <param name="direction">Selects in or out direction for edges. If not set, any edges are returned</param>
        /// <returns>Returns a list of edges starting or ending in the vertex identified by vertex document handle</returns>
        public async Task<List<T>> EdgesAsync<T>(string vertexId, EdgeDirection? direction = null, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.AllEdges,
                Method = HttpMethod.Get,
                Query = new Dictionary<string, string>(),
                Command = collectionName,
                EnableChangeTracking = db.Setting.DisableChangeTracking == false
            };

            command.Query.Add("vertex", vertexId);

            if (direction.HasValue && direction.Value != EdgeDirection.Any)
                command.Query.Add("direction", direction.Value == EdgeDirection.Inbound ? "in" : "out");

            var result = await command.RequestGenericListResult<T, EdgesInheritedCommandResult<List<T>>>().ConfigureAwait(false);

            if (baseResult != null)
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
        public ICursor<T> All<T>(int? skip = null, int? limit = null, int? batchSize = null)
        {
            batchSize = batchSize ?? db.Setting.Cursor.BatchSize;

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
        public ICursor<T> Range<T>(string attribute, object left, object right, bool? closed = false,
            int? skip = null, int? limit = null, int? batchSize = null)
        {
            batchSize = batchSize ?? db.Setting.Cursor.BatchSize;

            SimpleData data = new SimpleData
            {
                BatchSize = batchSize,
                Collection = collectionName,
                Attribute = attribute,
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
        public ICursor<T> ByExample<T>(object example, int? skip = null, int? limit = null, int? batchSize = null)
        {
            batchSize = batchSize ?? db.Setting.Cursor.BatchSize;

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
        public ICursor<T> Near<T>(double latitude, double longitude, string distance = null, string geo = null
            , int? skip = null, int? limit = null, int? batchSize = null)
        {
            batchSize = batchSize ?? db.Setting.Cursor.BatchSize;

            SimpleData data = new SimpleData
            {
                Latitude = latitude,
                Longitude = longitude,
                Distance = distance,
                Geo = geo,
                BatchSize = batchSize,
                Collection = collectionName,
                Limit = limit,
                Skip = skip
            };

            var command = new HttpCommand(db)
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
        public ICursor<T> Within<T>(double latitude, double longitude, double radius, string distance = null, string geo = null
            , int? skip = null, int? limit = null, int? batchSize = null)
        {
            batchSize = batchSize ?? db.Setting.Cursor.BatchSize;

            SimpleData data = new SimpleData
            {
                Latitude = latitude,
                Longitude = longitude,
                Radius = radius,
                Distance = distance,
                Geo = geo,
                BatchSize = batchSize,
                Collection = collectionName,
                Limit = limit,
                Skip = skip
            };

            var command = new HttpCommand(db)
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
        public ICursor<T> Fulltext<T>(string attribute, string query, string index = null
            , int? skip = null, int? limit = null, int? batchSize = null)
        {
            batchSize = batchSize ?? db.Setting.Cursor.BatchSize;

            SimpleData data = new SimpleData
            {
                Attribute = attribute,
                Query = query,
                Index = index,
                BatchSize = batchSize,
                Collection = collectionName,
                Limit = limit,
                Skip = skip
            };

            var command = new HttpCommand(db)
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
        public T FirstExample<T>(object example, Action<BaseResult> baseResult = null)
        {
            return FirstExampleAsync<T>(example, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Returns the first document matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <returns>A Document</returns>
        public async Task<T> FirstExampleAsync<T>(object example, Action<BaseResult> baseResult = null)
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

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result;
        }

        /// <summary>
        /// Returns a random document
        /// </summary>
        /// <returns>A Document</returns>
        public T Any<T>(Action<BaseResult> baseResult = null)
        {
            return AnyAsync<T>(baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Returns a random document
        /// </summary>
        /// <returns>A Document</returns>
        public async Task<T> AnyAsync<T>(Action<BaseResult> baseResult = null)
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

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result;
        }
    }

    public class ArangoCollection<T> : IDocumentCollection<T>, IEdgeCollection<T>
    {
        ArangoCollection collectionMethods;

        IArangoDatabase db;

        CollectionType collectionType { get; set; }

        /// <summary>
        /// Gets the collection by its type
        /// </summary>
        /// <returns></returns>
        public ArangoCollection(IArangoDatabase db, CollectionType type)
        {
            this.db = db;
            string collectionName = db.SharedSetting.Collection.ResolveCollectionName<T>();
            collectionMethods = new ArangoCollection(db, type, collectionName);
            collectionType = type;
        }

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="documents">Representation of the documents</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Documents identifiers</returns>
        public List<IDocumentIdentifierResult> InsertMultiple(IList documents, bool? waitForSync = null, Action<List<BaseResult>> baseResults = null)
        {
            return InsertMultipleAsync(documents, waitForSync, baseResults).ResultSynchronizer();
        }

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="documents">Representation of the documents</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Documents identifiers</returns>
        public async Task<List<IDocumentIdentifierResult>> InsertMultipleAsync(IList documents, bool? waitForSync = null, Action<List<BaseResult>> baseResults = null)
        {
            return await collectionMethods.InsertMultipleAsync(documents, waitForSync, baseResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public IDocumentIdentifierResult Insert(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return InsertAsync(document, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<IDocumentIdentifierResult> InsertAsync(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.InsertAsync(document, waitForSync, baseResult).ConfigureAwait(false);
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
        public IDocumentIdentifierResult ReplaceById(string id, object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return ReplaceByIdAsync(id, document, waitForSync, ignoreRevs, ifMatchRev, baseResult).ResultSynchronizer();
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
        public async Task<IDocumentIdentifierResult> ReplaceByIdAsync(string id, object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.ReplaceByIdAsync(id, document, waitForSync, ignoreRevs, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Completely updates the document
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public IDocumentIdentifierResult Replace(object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return ReplaceAsync(document, waitForSync, ignoreRevs, ifMatchRev, baseResult).ResultSynchronizer();
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
        public async Task<IDocumentIdentifierResult> ReplaceAsync(object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.ReplaceAsync(document, waitForSync, ignoreRevs, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        ///<summary>
        /// Partially updates the document with no change tracking
        ///</summary>
        ///<param name="id">The document handle or key of document</param>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        ///<param name="rev">Conditionally replace a document based on revision id</param>
        ///<param name="policy">To control the update behavior in case there is a revision mismatch</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        public IDocumentIdentifierResult UpdateById(string id, object document, bool? keepNull = null,
            bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return UpdateByIdAsync(id, document, keepNull, mergeObjects, waitForSync, ignoreRevs, ifMatchRev, baseResult).ResultSynchronizer();
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
        public async Task<IDocumentIdentifierResult> UpdateByIdAsync(string id, object document, bool? keepNull = null,
            bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.UpdateByIdAsync(id, document, keepNull, mergeObjects, waitForSync, ignoreRevs, ifMatchRev, baseResult).ConfigureAwait(false);
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
        public IDocumentIdentifierResult Update(object document, bool? keepNull = null, bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return UpdateAsync(document, keepNull, mergeObjects, waitForSync, ignoreRevs, ifMatchRev, baseResult).ResultSynchronizer();
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
        public async Task<IDocumentIdentifierResult> UpdateAsync(object document, bool? keepNull = null,
            bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.UpdateAsync(document, keepNull, mergeObjects, waitForSync, ignoreRevs, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the document without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public IDocumentIdentifierResult RemoveById(string id, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return RemoveByIdAsync(id, waitForSync, ifMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Deletes the document without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task<IDocumentIdentifierResult> RemoveByIdAsync(string id, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.RemoveByIdAsync(id, waitForSync, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the document
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public IDocumentIdentifierResult Remove(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return RemoveAsync(document, waitForSync, ifMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Deletes the document
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task<IDocumentIdentifierResult> RemoveAsync(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.RemoveAsync(document, waitForSync, ifMatchRev, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        public T Document(string id, string ifMatchRev = null, string ifNoneMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return DocumentAsync(id, ifMatchRev, ifNoneMatchRev, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        public async Task<T> DocumentAsync(string id, string ifMatchRev = null, string ifNoneMatchRev = null, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.DocumentAsync<T>(id, ifMatchRev, ifNoneMatchRev, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Check if document exists
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="onDocumentLoad">Runs when document loaded</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>A Document</returns>
        public bool Exists(string id, Action<T> onDocumentLoad = null, Action<BaseResult> baseResult = null)
        {
            return ExistsAsync(id, onDocumentLoad, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Check if document exists
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="onDocumentLoad">Runs when document loaded</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>A Document</returns>
        public async Task<bool> ExistsAsync(string id, Action<T> onDocumentLoad = null, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.ExistsAsync<T>(id, onDocumentLoad, baseResult).ConfigureAwait(false);
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
            return await collectionMethods.EdgesAsync<T>(vertexId, direction, baseResult).ConfigureAwait(false);
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
            return collectionMethods.All<T>(skip, limit, batchSize);
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
            string attributeName = db.SharedSetting.Collection.ResolvePropertyName(attribute);

            return collectionMethods.Range<T>(attributeName, left, right, closed, skip, limit, batchSize);
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
            return collectionMethods.ByExample<T>(example, skip, limit, batchSize);
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
            string distanceName = distance != null ? db.SharedSetting.Collection.ResolvePropertyName(distance) : null;

            return collectionMethods.Near<T>(latitude, longitude, distanceName, geo, skip, limit, batchSize);
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
            string distanceName = distance != null ? db.SharedSetting.Collection.ResolvePropertyName(distance) : null;

            return collectionMethods.Within<T>(latitude, longitude, radius, distanceName, geo, skip, limit, batchSize);
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
        public ICursor<T> Fulltext(Expression<Func<T, object>> attribute, string query, string index = null
            , int? skip = null, int? limit = null, int? batchSize = null)
        {
            string attributeName = db.SharedSetting.Collection.ResolvePropertyName(attribute);

            return collectionMethods.Fulltext<T>(attributeName, query, index, skip, limit, batchSize);
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
            return await collectionMethods.FirstExampleAsync<T>(example, baseResult).ConfigureAwait(false);
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
            return await collectionMethods.AnyAsync<T>(baseResult).ConfigureAwait(false);
        }
    }
}
