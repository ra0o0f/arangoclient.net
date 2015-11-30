using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client.Utility;
using ArangoDB.Client.Http;
using System.Net.Http;
using ArangoDB.Client.ChangeTracking;
using ArangoDB.Client.Common.Newtonsoft.Json.Linq;
using ArangoDB.Client.Serialization;

namespace ArangoDB.Client.Graph
{
    public class ArangoGraphVertex : IArangoGraphVertex
    {
        IArangoDatabase db;
        string graphName;
        string collection;

        public ArangoGraphVertex(IArangoDatabase db, string graphName,string collection)
        {
            this.db = db;
            this.graphName = graphName;
            this.collection = collection;
        }

        /// <summary>
        /// Add an additional vertex collection to the graph
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult AddCollection(Action<BaseResult> baseResult = null)
        {
            return AddCollectionAsync(baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Add an additional vertex collection to the graph
        /// </summary>
        /// <param name="collection">The name of the collection</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> AddCollectionAsync(Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Post,
                Command = $"{graphName}/vertex"
            };

            var data = new AddVertexCollectionData
            {
                Collection = collection
            };

            var result = await command.RequestMergedResult<GraphResult>(data).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Graph;
        }

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="collection">The name of the collection</param>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult RemoveCollection(bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            return RemoveCollectionAsync(dropCollection, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="collection">The name of the collection</param>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> RemoveCollectionAsync(bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Delete,
                Command = $"{graphName}/vertex/{collection}"
            };

            var data = new DropGraphCollectionData
            {
                DropCollection = dropCollection
            };

            var result = await command.RequestMergedResult<GraphResult>(data).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Graph;
        }


        /// <summary>
        /// Creates a new vertex
        /// </summary>
        /// <param name="document">The vertex document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        public IDocumentIdentifierResult Insert(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return InsertAsync(document, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Creates a new vertex
        /// </summary>
        /// <param name="document">The vertex document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        public async Task<IDocumentIdentifierResult> InsertAsync(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            waitForSync = waitForSync ?? db.Setting.WaitForSync;

            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Post,
                Command = $"{graphName}/vertex/{collection}",
                Query = new Dictionary<string, string>()
            };

            command.Query.Add("waitForSync", waitForSync.ToString());

            var result = await command.RequestMergedResult<InsertVertexResult>(document).ConfigureAwait(false);

            if (!result.BaseResult.Error)
            {
                if (db.Setting.DisableChangeTracking == false)
                    db.ChangeTracker.TrackChanges(document, result.Result.Vertex);

                db.SharedSetting.IdentifierModifier.Modify(document, result.Result.Vertex);
            }

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Vertex;
        }

        /// <summary>
        /// Fetches an existing vertex
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        public T Get<T>(string id, Action<BaseResult> baseResult = null)
        {
            return GetAsync<T>(id, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Fetches an existing vertex
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        public async Task<T> GetAsync<T>(string id, Action<BaseResult> baseResult = null)
        {
            var documentHandle = id.IndexOf("/") == -1 ? $"{collection}/{id}" : id;

            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Get,
                Command = $"{graphName}/vertex/{documentHandle}",
                EnableChangeTracking = !db.Setting.DisableChangeTracking
            };

            var defaultThrowForServerErrors = db.Setting.ThrowForServerErrors;
            db.Setting.ThrowForServerErrors = false;

            var result = await command.RequestGenericSingleResult<T, VertexInheritedCommandResult<T>>().ConfigureAwait(false);

            db.Setting.ThrowForServerErrors = defaultThrowForServerErrors;

            if (db.Setting.Document.ThrowIfDocumentDoesNotExists ||
                (result.BaseResult.Error && result.BaseResult.ErrorNum != 1202))
                new BaseResultAnalyzer(db).Throw(result.BaseResult);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result;
        }

        /// <summary>
        /// Partially updates the vertex with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public IDocumentIdentifierResult UpdateById(string id, object document
            , bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null)
        {
            return UpdateByIdAsync(id, document, waitForSync, keepNull, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Partially updates the vertex with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<IDocumentIdentifierResult> UpdateByIdAsync(string id, object document
            , bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null)
        {
            var documentHandle = id.IndexOf("/") == -1 ? $"{collection}/{id}" : id;

            keepNull = keepNull ?? db.Setting.Document.KeepNullAttributesOnUpdate;
            waitForSync = waitForSync ?? db.Setting.WaitForSync;

            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = new HttpMethod("PATCH"),
                Query = new Dictionary<string, string>(),
                Command = $"{graphName}/vertex/{documentHandle}"
            };

            command.Query.Add("keepNull", keepNull.ToString());
            command.Query.Add("waitForSync", waitForSync.ToString());

            var result = await command.RequestMergedResult<CrudVertexResult>(document).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Vertex;
        }

        ///<summary>
        ///Partially updates the vertex
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        public IDocumentIdentifierResult Update(object document,
           bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null)
        {
            return UpdateAsync(document, waitForSync, keepNull, baseResult).ResultSynchronizer();
        }

        ///<summary>
        ///Partially updates the vertex
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        public async Task<IDocumentIdentifierResult> UpdateAsync(object document,
           bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null)
        {
            if (db.Setting.DisableChangeTracking == true)
                throw new InvalidOperationException("Change tracking is disabled, use UpdateById() instead");

            DocumentContainer container = null;
            JObject jObject = null;
            var changed = db.ChangeTracker.GetChanges(document, out container, out jObject);

            if (changed.Count != 0)
            {
                BaseResult bResult = null;

                var result = await UpdateByIdAsync(container.Id, changed, waitForSync, keepNull, (b) => bResult = b).ConfigureAwait(false);

                if (!bResult.Error)
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

        /// <summary>
        /// Completely updates the vertex with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public IDocumentIdentifierResult ReplaceById(string id, object document,
            bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return ReplaceByIdAsync(id, document, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Completely updates the vertex with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<IDocumentIdentifierResult> ReplaceByIdAsync(string id, object document,
            bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            var documentHandle = id.IndexOf("/") == -1 ? $"{collection}/{id}" : id;

            waitForSync = waitForSync ?? db.Setting.WaitForSync;

            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Put,
                Query = new Dictionary<string, string>(),
                Command = $"{graphName}/vertex/{documentHandle}"
            };

            command.Query.Add("waitForSync", waitForSync.ToString());

            var result = await command.RequestMergedResult<CrudVertexResult>(document).ConfigureAwait(false);

            if (!result.BaseResult.Error)
                db.SharedSetting.IdentifierModifier.Modify(document, result.Result.Vertex);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Vertex;
        }

        /// <summary>
        /// Completely updates the vertex
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public IDocumentIdentifierResult Replace(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return ReplaceAsync(document, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Completely updates the vertex
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<IDocumentIdentifierResult> ReplaceAsync(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            if (db.Setting.DisableChangeTracking == true)
                throw new InvalidOperationException("Change tracking is disabled, use ReplaceById() instead");

            var container = db.ChangeTracker.FindDocumentInfo(document);

            BaseResult bResult = null;

            var result = await ReplaceByIdAsync(container.Id, document, waitForSync, (b) => bResult = b).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(bResult);

            if (!bResult.Error)
            {
                container.Rev = result.Rev;
                container.Document = JObject.FromObject(document, new DocumentSerializer(db).CreateJsonSerializer());
                db.SharedSetting.IdentifierModifier.FindIdentifierMethodFor(document.GetType()).SetRevision(document, result.Rev);
            }

            return result;
        }

        /// <summary>
        /// Deletes the vertex without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public bool RemoveById(string id, bool? waitForSync = null
            , Action<BaseResult> baseResult = null)
        {
            return RemoveByIdAsync(id, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Deletes the vertex without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task<bool> RemoveByIdAsync(string id,
            bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            var documentHandle = id.IndexOf("/") == -1 ? $"{collection}/{id}" : id;

            waitForSync = waitForSync ?? db.Setting.WaitForSync;

            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.Document,
                Method = HttpMethod.Delete,
                Query = new Dictionary<string, string>(),
                Command = $"{graphName}/vertex/{documentHandle}"
            };

            command.Query.Add("waitForSync", waitForSync.ToString());

            var result = await command.RequestMergedResult<DropGraphResult>().ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Removed;
        }

        /// <summary>
        /// Deletes the vertex
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public bool Remove(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return RemoveAsync(document, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Deletes the vertex
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync(object document,
            bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            if (db.Setting.DisableChangeTracking == true)
                throw new InvalidOperationException("Change tracking is disabled, use RemoveById() instead");

            var container = db.ChangeTracker.FindDocumentInfo(document);

            BaseResult bResult = null;

            var result = await RemoveByIdAsync(container.Id, waitForSync, (b) => bResult = b).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(bResult);

            if (!bResult.Error)
                db.ChangeTracker.StopTrackChanges(document);

            return result;
        }
    }

    public class ArangoGraphVertex<T> : IArangoGraphVertex<T>
    {
        IArangoDatabase db;
        string graphName;
        IArangoGraphVertex collectionMethods;


        public ArangoGraphVertex(IArangoDatabase db, string graphName)
        {
            this.db = db;
            this.graphName = graphName;
            collectionMethods = new ArangoGraphVertex(db, graphName, db.SharedSetting.Collection.ResolveCollectionName<T>());
        }

        /// <summary>
        /// Add an additional vertex collection to the graph
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult AddCollection(Action<BaseResult> baseResult = null)
        {
            return AddCollectionAsync(baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Add an additional vertex collection to the graph
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> AddCollectionAsync(Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.AddCollectionAsync(baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult RemoveCollection(bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            return RemoveCollectionAsync(dropCollection, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> RemoveCollectionAsync(bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.RemoveCollectionAsync(dropCollection, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new vertex
        /// </summary>
        /// <param name="document">The vertex document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        public IDocumentIdentifierResult Insert(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return InsertAsync(document, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Creates a new vertex
        /// </summary>
        /// <param name="document">The vertex document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="baseResult"></param>
        /// <returns>DocumentIdentifierResult</returns>
        public async Task<IDocumentIdentifierResult> InsertAsync(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.InsertAsync(document, waitForSync, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Fetches an existing vertex
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        public T Get(string id, Action<BaseResult> baseResult = null)
        {
            return GetAsync(id, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Fetches an existing vertex
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="baseResult"></param>
        /// <returns>T</returns>
        public async Task<T> GetAsync(string id, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.GetAsync<T>(id, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Partially updates the vertex with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public IDocumentIdentifierResult UpdateById(string id, object document
            , bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null)
        {
            return UpdateByIdAsync(id, document, waitForSync, keepNull, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Partially updates the vertex with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the patch document</param>
        /// <param name="waitForSync">Define if the request should wait until synced to disk</param>
        /// <param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<IDocumentIdentifierResult> UpdateByIdAsync(string id, object document
            , bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.UpdateByIdAsync(id, document, waitForSync, keepNull, baseResult).ConfigureAwait(false);
        }

        ///<summary>
        ///Partially updates the vertex
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        public IDocumentIdentifierResult Update(object document,
           bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null)
        {
            return UpdateAsync(document, waitForSync, keepNull, baseResult).ResultSynchronizer();
        }

        ///<summary>
        ///Partially updates the vertex
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        public async Task<IDocumentIdentifierResult> UpdateAsync(object document,
           bool? waitForSync = null, bool? keepNull = null, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.UpdateAsync(document, waitForSync, keepNull, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Completely updates the vertex with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public IDocumentIdentifierResult ReplaceById(string id, object document,
            bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return ReplaceByIdAsync(id, document, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Completely updates the vertex with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<IDocumentIdentifierResult> ReplaceByIdAsync(string id, object document,
            bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.ReplaceByIdAsync(id, document, waitForSync, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Completely updates the vertex
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public IDocumentIdentifierResult Replace(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return ReplaceAsync(document, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Completely updates the vertex
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        public async Task<IDocumentIdentifierResult> ReplaceAsync(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.ReplaceAsync(document, waitForSync, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the vertex without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public bool RemoveById(string id, bool? waitForSync = null
            , Action<BaseResult> baseResult = null)
        {
            return RemoveByIdAsync(id, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Deletes the vertex without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task<bool> RemoveByIdAsync(string id,
            bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.RemoveByIdAsync(id, waitForSync, baseResult).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Deletes the vertex
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public bool Remove(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return RemoveAsync(document, waitForSync, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Deletes the vertex
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync(object document,
            bool? waitForSync = null, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.RemoveAsync(document, waitForSync, baseResult).ConfigureAwait(false);
        }
    }
}
