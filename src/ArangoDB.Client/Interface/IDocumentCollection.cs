using ArangoDB.Client.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public interface IDocumentCollection : IArangoCollection
    {
        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="documents">Representation of the documents</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Documents identifiers</returns>
        Task<List<IDocumentIdentifierResult>> InsertMultipleAsync(IList documents, bool? waitForSync = null, Action<List<BaseResult>> baseResults = null);

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="documents">Representation of the documents</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Documents identifiers</returns>
        List<IDocumentIdentifierResult> InsertMultiple(IList documents, bool? waitForSync = null, Action<List<BaseResult>> baseResults = null);

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> InsertAsync(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        IDocumentIdentifierResult Insert(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);
    }

    public interface IDocumentCollection<T> : IArangoCollection<T>
    {
        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="documents">Representation of the documents</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Documents identifiers</returns>
        Task<List<IDocumentIdentifierResult>> InsertMultipleAsync(IList documents, bool? waitForSync = null, Action<List<BaseResult>> baseResults = null);

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="documents">Representation of the documents</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Documents identifiers</returns>
        List<IDocumentIdentifierResult> InsertMultiple(IList documents, bool? waitForSync = null, Action<List<BaseResult>> baseResults = null);

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> InsertAsync(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="createCollection">If true, then the collection is created if it does not yet exist</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        IDocumentIdentifierResult Insert(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);
    }
}
