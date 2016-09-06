using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ArangoDB.Client.Data;
using ArangoDB.Client.Http;
using ArangoDB.Client.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client.Utility;

namespace ArangoDB.Client.Advanced
{
    public class AdvancedOperation : IAdvancedOperation
    {
        IArangoDatabase db;

        public AdvancedOperation(IArangoDatabase db)
        {
            this.db = db;
        }

        /// <summary>
        /// Creates an index
        /// </summary>
        /// <typeparam name="TCollection">Collection Type</typeparam>
        /// <param name="data">Index details</param>
        /// <param name="baseResult"></param>
        /// <returns>EnsureIndexResult</returns>
        public EnsureIndexResult EnsureIndex<TCollection>(EnsureIndexData data, Action<BaseResult> baseResult = null)
        {
            return EnsureIndexAsync<TCollection>(data, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Creates an index
        /// </summary>
        /// <typeparam name="TCollection">Collection Type</typeparam>
        /// <param name="data">Index details</param>
        /// <param name="baseResult"></param>
        /// <returns>EnsureIndexResult</returns>
        public async Task<EnsureIndexResult> EnsureIndexAsync<TCollection>(EnsureIndexData data, Action<BaseResult> baseResult = null)
        {
            string collection = db.SharedSetting.Collection.ResolveCollectionName<TCollection>();
            return await EnsureIndexAsync(collection, data, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates an index
        /// </summary>
        /// <param name="collection">The collection name</param>
        /// <param name="data">Index details</param>
        /// <param name="baseResult"></param>
        /// <returns>EnsureIndexResult</returns>
        public EnsureIndexResult EnsureIndex(string collection, EnsureIndexData data, Action<BaseResult> baseResult = null)
        {
            return EnsureIndexAsync(collection, data, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Creates an index
        /// </summary>
        /// <param name="collection">The collection name</param>
        /// <param name="data">Index details</param>
        /// <param name="baseResult"></param>
        /// <returns>EnsureIndexResult</returns>
        public async Task<EnsureIndexResult> EnsureIndexAsync(string collection, EnsureIndexData data, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(this.db)
            {
                Api = CommandApi.Index,
                Method = HttpMethod.Post,
                Query = new Dictionary<string, string>()
            };

            command.Query.Add("collection", collection);

            var result = await command.RequestMergedResult<EnsureIndexResult>(data).ConfigureAwait(false);

            baseResult?.Invoke(result.BaseResult);

            return result.Result;
        }

        /// <summary>
        /// Imports documents
        /// </summary>
        /// <typeparam name="TCollection">Collection Type</typeparam>
        /// <param name="documents">Documents to import</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="overwrite">If true, then all data in the collection will be removed prior to the import</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <param name="onDuplicate">Controls what action is carried out in case of a unique key constraint violation</param>
        /// <param name="complete">If true, then it will make the whole import fail if any error occurs</param>
        /// <param name="details">If true, then the result will include details about documents that could not be imported</param>
        /// <param name="baseResult"></param>
        /// <returns>BulkImportResult</returns>
        public BulkImportResult BulkImport<TCollection>(IEnumerable documents, bool? overwrite = null
            , bool? waitForSync = null, ImportDuplicatePolicy? onDuplicate = null, bool? complete = null, bool? details = null
            , Action<BaseResult> baseResult = null)
        {
            return BulkImportAsync<TCollection>(documents, overwrite, waitForSync, onDuplicate, complete, details, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Imports documents
        /// </summary>
        /// <typeparam name="TCollection">Collection Type</typeparam>
        /// <param name="documents">Documents to import</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="overwrite">If true, then all data in the collection will be removed prior to the import</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <param name="onDuplicate">Controls what action is carried out in case of a unique key constraint violation</param>
        /// <param name="complete">If true, then it will make the whole import fail if any error occurs</param>
        /// <param name="details">If true, then the result will include details about documents that could not be imported</param>
        /// <param name="baseResult"></param>
        /// <returns>BulkImportResult</returns>
        public async Task<BulkImportResult> BulkImportAsync<TCollection>(IEnumerable documents, bool? overwrite = null
            , bool? waitForSync = null, ImportDuplicatePolicy? onDuplicate = null, bool? complete = null, bool? details = null
            , Action<BaseResult> baseResult = null)
        {
            string collectionName = db.SharedSetting.Collection.ResolveCollectionName<TCollection>();

            return await BulkImportAsync(collectionName, documents, overwrite, waitForSync, onDuplicate, complete, details, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Imports documents
        /// </summary>
        /// <param name="collection">The collection name</param>
        /// <param name="documents">Documents to import</param>
        /// <param name="overwrite">If true, then all data in the collection will be removed prior to the import</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <param name="onDuplicate">Controls what action is carried out in case of a unique key constraint violation</param>
        /// <param name="complete">If true, then it will make the whole import fail if any error occurs</param>
        /// <param name="details">If true, then the result will include details about documents that could not be imported</param>
        /// <param name="baseResult"></param>
        /// <returns>BulkImportResult</returns>
        public BulkImportResult BulkImport(string collection, IEnumerable documents, bool? overwrite = null
            , bool? waitForSync = null, ImportDuplicatePolicy? onDuplicate = null, bool? complete = null, bool? details = null
            , Action<BaseResult> baseResult = null)
        {
            return BulkImportAsync(collection, documents, overwrite, waitForSync, onDuplicate, complete, details, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Imports documents
        /// </summary>
        /// <param name="collection">The collection name</param>
        /// <param name="documents">Documents to import</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="overwrite">If true, then all data in the collection will be removed prior to the import</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <param name="onDuplicate">Controls what action is carried out in case of a unique key constraint violation</param>
        /// <param name="complete">If true, then it will make the whole import fail if any error occurs</param>
        /// <param name="details">If true, then the result will include details about documents that could not be imported</param>
        /// <param name="baseResult"></param>
        /// <returns>BulkImportResult</returns>
        public async Task<BulkImportResult> BulkImportAsync(string collection, IEnumerable documents, bool? overwrite = null
            , bool? waitForSync = null, ImportDuplicatePolicy? onDuplicate = null, bool? complete = null, bool? details = null
            , Action<BaseResult> baseResult = null)
        {
            waitForSync = waitForSync ?? db.Setting.WaitForSync;

            var command = new HttpCommand(db)
            {
                Api = CommandApi.Import,
                Method = HttpMethod.Post,
                Query = new Dictionary<string, string>()
            };

            command.Query.Add("type", "documents");
            command.Query.Add("collection", collection);
            command.Query.Add("waitForSync", waitForSync.ToString());

            if (overwrite.HasValue)
                command.Query.Add("overwrite", overwrite.ToString());
            if (onDuplicate.HasValue)
                command.Query.Add("onDuplicate", Utility.Utils.ImportDuplicatePolicyToString(onDuplicate.Value));
            if (complete.HasValue)
                command.Query.Add("complete", complete.ToString());
            if (details.HasValue)
                command.Query.Add("details", details.ToString());

            Func<StreamWriter, Task> onStreamReady = async (streamWriter) =>
            {
                var docSerializer = new DocumentSerializer(db);
                var serializer = docSerializer.CreateJsonSerializer();
                var jsonWriter = new JsonTextWriter(streamWriter);
                foreach (var d in documents)
                {
                    string json = d as string;
                    if (json != null)
                    {
                        streamWriter.Write($"{json}{Environment.NewLine}");
                        continue;
                    }

                    var jObject = d as JObject;
                    if (jObject != null)
                    {
                        jObject.WriteTo(jsonWriter);
                        streamWriter.Write(Environment.NewLine);
                        continue;
                    }

                    serializer.Serialize(jsonWriter, d);
                    streamWriter.Write(Environment.NewLine);
                }
                await streamWriter.FlushAsync().ConfigureAwait(false);
            };

            var result = await command.RequestDistinctResult<BulkImportResult>(onStreamReady: onStreamReady).ConfigureAwait(false);

            baseResult?.Invoke(result.BaseResult);

            return result.Result;
        }
    }
}
