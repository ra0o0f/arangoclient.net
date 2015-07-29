using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public interface IDocumentCollection<T> : IArangoCollection<T>
    {
        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        Task<DocumentIdentifierResult> InsertAsync(object document, bool? createCollection = null, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        DocumentIdentifierResult Insert(object document, bool? createCollection = null, bool? waitForSync = null, Action<BaseResult> baseResult = null);

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
        BulkImportResult Import(IEnumerable<object> documents, bool? createCollection = null, bool? waitForSync = null, bool? complete = false, bool? details = false, BulkImportMethod? importMethod = null, Action<BaseResult> baseResult = null);

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
        Task<BulkImportResult> ImportAsync(IEnumerable<object> documents, bool? createCollection = null, bool? waitForSync = null, bool? complete = false, bool? details = false, BulkImportMethod? importMethod = null, Action<BaseResult> baseResult = null);
    }
}
