using ArangoDB.Client.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public interface IAdvancedOperation
    {
        /// <summary>
        /// Creates an index
        /// </summary>
        /// <typeparam name="TCollection">Collection Type</typeparam>
        /// <param name="data">Index details</param>
        /// <param name="baseResult"></param>
        /// <returns>EnsureIndexResult</returns>
        EnsureIndexResult EnsureIndex<TCollection>(EnsureIndexData data, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates an index
        /// </summary>
        /// <typeparam name="TCollection">Collection Type</typeparam>
        /// <param name="data">Index details</param>
        /// <param name="baseResult"></param>
        /// <returns>EnsureIndexResult</returns>
        Task<EnsureIndexResult> EnsureIndexAsync<TCollection>(EnsureIndexData data, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates an index
        /// </summary>
        /// <param name="collection">The collection name</param>
        /// <param name="data">Index details</param>
        /// <param name="baseResult"></param>
        /// <returns>EnsureIndexResult</returns>
        EnsureIndexResult EnsureIndex(string collection, EnsureIndexData data, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates an index
        /// </summary>
        /// <param name="collection">The collection name</param>
        /// <param name="data">Index details</param>
        /// <param name="baseResult"></param>
        /// <returns>EnsureIndexResult</returns>
        Task<EnsureIndexResult> EnsureIndexAsync(string collection, EnsureIndexData data, Action<BaseResult> baseResult = null);
        
        /// <summary>
        /// Imports documents
        /// </summary>
        /// <typeparam name="TCollection">Collection Type</typeparam>
        /// <param name="documents">Documents to import</param>
        /// <param name="overwrite">If true, then all data in the collection will be removed prior to the import</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <param name="onDuplicate">Controls what action is carried out in case of a unique key constraint violation</param>
        /// <param name="complete">If true, then it will make the whole import fail if any error occurs</param>
        /// <param name="details">If true, then the result will include details about documents that could not be imported</param>
        /// <param name="baseResult"></param>
        /// <returns>BulkImportResult</returns>
        BulkImportResult BulkImport<TCollection>(IEnumerable documents, bool? overwrite = null
            , bool? waitForSync = null, ImportDuplicatePolicy? onDuplicate = null, bool? complete = null, bool? details = null
            , Action<BaseResult> baseResult = null);

        /// <summary>
        /// Imports documents
        /// </summary>
        /// <typeparam name="TCollection">Collection Type</typeparam>
        /// <param name="documents">Documents to import</param>
        /// <param name="overwrite">If true, then all data in the collection will be removed prior to the import</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <param name="onDuplicate">Controls what action is carried out in case of a unique key constraint violation</param>
        /// <param name="complete">If true, then it will make the whole import fail if any error occurs</param>
        /// <param name="details">If true, then the result will include details about documents that could not be imported</param>
        /// <param name="baseResult"></param>
        /// <returns>BulkImportResult</returns>
        Task<BulkImportResult> BulkImportAsync<TCollection>(IEnumerable documents, bool? overwrite = null
            , bool? waitForSync = null, ImportDuplicatePolicy? onDuplicate = null, bool? complete = null, bool? details = null
            , Action<BaseResult> baseResult = null);

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
        BulkImportResult BulkImport(string collection, IEnumerable documents, bool? overwrite = null
            , bool? waitForSync = null, ImportDuplicatePolicy? onDuplicate = null, bool? complete = null, bool? details = null
            , Action<BaseResult> baseResult = null);

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
        Task<BulkImportResult> BulkImportAsync(string collection, IEnumerable documents, bool? overwrite = null
            , bool? waitForSync = null, ImportDuplicatePolicy? onDuplicate = null, bool? complete = null, bool? details = null
            , Action<BaseResult> baseResult = null);
    }
}
