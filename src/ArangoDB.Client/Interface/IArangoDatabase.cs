using ArangoDB.Client.Advanced;
using ArangoDB.Client.ChangeTracking;
using ArangoDB.Client.Data;
using ArangoDB.Client.Http;
using ArangoDB.Client.Query;
using ArangoDB.Client.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public interface IArangoDatabase : IDisposable
    {
        IAdvancedOperation Advanced { get; set; }

        IHttpConnection Connection { get; set; }

        DocumentTracker ChangeTracker { get; set; }

        DatabaseSharedSetting SharedSetting { get; set; }

        DatabaseSetting Setting { get; set; }

        IDocumentCollection<T> Collection<T>();

        IEdgeCollection<T> EdgeCollection<T>();

        IDocumentCollection Collection(string collection);

        IEdgeCollection EdgeCollection(string collection);

        ICursor<T> CreateStatement<T>(string query, IList<QueryParameter> bindVars = null, bool? count = null,
            int? batchSize = null, TimeSpan? ttl = null, QueryOption options = null);

        ArangoQueryable<T> Query<T>();

        ArangoQueryable<AQL> Query();

        string NameOf<T>();

        string NameOf(Type type);

        string NameOf<T>(Expression<Func<T, object>> member);

        void Log(string message);

        bool LoggerAvailable { get; }
        
        /// <summary>
        /// Get Document JsonObject and Identifiers
        /// </summary>
        /// <param name="id">id of document</param>
        /// <returns>A DocumentContainer</returns>
        DocumentContainer FindDocumentInfo(string id);

        /// <summary>
        /// Get Document JsonObject and Identifiers
        /// </summary>
        /// <param name="id">document object</param>
        /// <returns>A DocumentContainer</returns>
        DocumentContainer FindDocumentInfo(object document);

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="documents">Representation of the documents</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Documents identifiers</returns>
        Task<List<IDocumentIdentifierResult>> InsertMultipleAsync<T>(IList documents, bool? waitForSync = null, Action<List<BaseResult>> baseResults = null);

        /// <summary>
        /// Creates a new document in the collection
        /// </summary>
        /// <param name="documents">Representation of the documents</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Documents identifiers</returns>
        List<IDocumentIdentifierResult> InsertMultiple<T>(IList documents, bool? waitForSync = null, Action<List<BaseResult>> baseResults = null);

        /// <summary>
        /// Creates a new document in the collection for specific type
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> InsertAsync<T>(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a new document in the collection for specific type
        /// </summary>
        /// <param name="document">Representation of the document</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        IDocumentIdentifierResult Insert<T>(object document, bool? waitForSync = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the document
        /// </summary>
        /// <param name="document">Representation of the new document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        IDocumentIdentifierResult Replace<T>(object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> ReplaceAsync<T>(object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Completely updates the document with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        IDocumentIdentifierResult ReplaceById<T>(string id, object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);
        
        /// <summary>
        /// Completely updates the document with no change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="document">Representation of the new document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> ReplaceByIdAsync<T>(string id, object document, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);
        
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
        IDocumentIdentifierResult UpdateById<T>(string id, object document, bool? keepNull = null, bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);
        
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
        Task<IDocumentIdentifierResult> UpdateByIdAsync<T>(string id, object document, bool? keepNull = null, bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        ///<summary>
        ///Partially updates the document
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        ///<param name="policy">To control the update behavior in case there is a revision mismatch</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        IDocumentIdentifierResult Update<T>(object document, bool? keepNull = null, bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        ///<summary>
        ///Partially updates the document
        ///</summary>
        ///<param name="document">Representation of the patch document</param>
        ///<param name="keepNull">For remove any attributes from the existing document that are contained in the patch document with an attribute value of null</param>
        ///<param name="mergeObjects">Controls whether objects (not arrays) will be merged if present in both the existing and the patch document</param>
        ///<param name="policy">To control the update behavior in case there is a revision mismatch</param>
        ///<param name="waitForSync">Wait until document has been synced to disk</param>
        ///<returns>Document identifiers</returns>
        Task<IDocumentIdentifierResult> UpdateAsync<T>(object document, bool? keepNull = null, bool? mergeObjects = null, bool? waitForSync = null, bool? ignoreRevs = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the document without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        IDocumentIdentifierResult RemoveById<T>(string id, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the document without change tracking
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="rev">Conditionally replace a document based on revision id</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        Task<IDocumentIdentifierResult> RemoveByIdAsync<T>(string id, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the document
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        IDocumentIdentifierResult Remove<T>(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes the document
        /// </summary>
        /// <param name="document">document reference</param>
        /// <param name="policy">To control the update behavior in case there is a revision mismatch</param>
        /// <param name="waitForSync">Wait until document has been synced to disk</param>
        /// <returns></returns>
        Task<IDocumentIdentifierResult> RemoveAsync<T>(object document, bool? waitForSync = null, string ifMatchRev = null, Action<BaseResult> baseResult = null);
        
        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        T Document<T>(string id, string ifMatchRev = null, string ifNoneMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Reads a single document
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <returns>A Document</returns>
        Task<T> DocumentAsync<T>(string id, string ifMatchRev = null, string ifNoneMatchRev = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Check if document exists
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="onDocumentLoad">Runs when document loaded</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>A Document</returns>
        Task<bool> ExistsAsync<T>(string id, Action<T> onDocumentLoad = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Check if document exists
        /// </summary>
        /// <param name="id">The document handle or key of document</param>
        /// <param name="onDocumentLoad">Runs when document loaded</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>A Document</returns>
        bool Exists<T>(string id, Action<T> onDocumentLoad = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Read in or outbound edges
        /// </summary>
        /// <param name="vertexId">The document handle of the start vertex</param>
        /// <param name="direction">Selects in or out direction for edges. If not set, any edges are returned</param>
        /// <returns>Returns a list of edges starting or ending in the vertex identified by vertex document handle</returns>
        List<T> Edges<T>(string vertexId, EdgeDirection? direction = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Read in or outbound edges
        /// </summary>
        /// <param name="vertexId">The document handle of the start vertex</param>
        /// <param name="direction">Selects in or out direction for edges. If not set, any edges are returned</param>
        /// <returns>Returns a list of edges starting or ending in the vertex identified by vertex document handle</returns>
        Task<List<T>> EdgesAsync<T>(string vertexId, EdgeDirection? direction = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Returns all documents of a collections
        /// </summary>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        ICursor<T> All<T>(int? skip = null, int? limit = null, int? batchSize = null);

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
        ICursor<T> Range<T>(Expression<Func<T, object>> attribute, object left, object right, bool? closed = false,
            int? skip = null, int? limit = null, int? batchSize = null);

        /// <summary>
        /// Finds all documents matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <param name="skip">The number of documents to skip in the query</param>
        /// <param name="limit">The maximal amount of documents to return. The skip is applied before the limit restriction</param>
        /// <param name="batchSize">Limits the number of results to be transferred in one batch</param>
        /// <returns>Returns a cursor</returns>
        ICursor<T> ByExample<T>(object example, int? skip = null, int? limit = null, int? batchSize = null);

        /// <summary>
        /// Returns the first document matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <returns>A Document</returns>
        T FirstExample<T>(object example, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Returns the first document matching a given example
        /// </summary>
        /// <param name="example">The example document</param>
        /// <returns>A Document</returns>
        Task<T> FirstExampleAsync<T>(object example, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Returns a random document
        /// </summary>
        /// <returns>A Document</returns>
        T Any<T>(Action<BaseResult> baseResult = null);

        /// <summary>
        /// Returns a random document
        /// </summary>
        /// <returns>A Document</returns>
        Task<T> AnyAsync<T>(Action<BaseResult> baseResult = null);

        /// <summary>
        /// Retrieves information about the current database
        /// </summary>
        /// <returns>DatabaseInformation</returns>
        DatabaseInformation CurrentDatabaseInformation(Action<BaseResult> baseResult = null);

        /// <summary>
        /// Retrieves information about the current database
        /// </summary>
        /// <returns>DatabaseInformation</returns>
        Task<DatabaseInformation> CurrentDatabaseInformationAsync(Action<BaseResult> baseResult = null);

        /// <summary>
        /// List of accessible databases
        /// </summary>
        /// <returns>List of database names</returns>
        List<string> ListAccessibleDatabases(Action<BaseResult> baseResult = null);

        /// <summary>
        /// List of accessible databases
        /// </summary>
        /// <returns>List of database names</returns>
        Task<List<string>> ListAccessibleDatabasesAsync(Action<BaseResult> baseResult = null);

        /// <summary>
        /// List of databases
        /// </summary>
        /// <returns>List of database names</returns>
        List<string> ListDatabases(Action<BaseResult> baseResult = null);

        /// <summary>
        /// List of databases
        /// </summary>
        /// <returns>List of database names</returns>
        Task<List<string>> ListDatabasesAsync(Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a database
        /// </summary>
        /// <param name="name">Name of the database</param>
        /// <param name="users">list of database user</param>
        /// <returns></returns>
        void CreateDatabase(string name, List<DatabaseUser> users = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a database
        /// </summary>
        /// <param name="name">Name of the database</param>
        /// <param name="users">list of database user</param>
        /// <returns></returns>
        Task CreateDatabaseAsync(string name, List<DatabaseUser> users = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Drops the collection identified by collection-name
        /// </summary>
        /// <param name="name">Name of the collection</param>
        /// <param name="baseResult"></param>
        /// <returns>DropCollectionResult</returns>
        DropCollectionResult DropCollection(string name, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Drops the collection identified by collection-name
        /// </summary>
        /// <param name="name">Name of the collection</param>
        /// <param name="baseResult"></param>
        /// <returns>DropCollectionResult</returns>
        Task<DropCollectionResult> DropCollectionAsync(string name, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a collection
        /// </summary>
        /// <param name="name">Name of the collection</param>
        /// <param name="waitForSync">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="doCompact">Whether or not the collection will be compacted</param>
        /// <param name="journalSize"> The maximal size of a journal or datafile in bytes. The value must be at least 1048576 (1 MB).</param>
        /// <param name="isSystem"> If true, create a system collection. In this case collection-name should start with an underscore</param>
        /// <param name="isVolatile">If true then the collection data is kept in-memory only and not made persistent</param>
        /// <param name="type"> The type of the collection to create</param>
        /// <param name="numberOfShards">In a cluster, this value determines the number of shards to create for the collection</param>
        /// <param name="shardKeys">In a cluster, this attribute determines which document attributes are used to determine the target shard for documents</param>
        /// <returns>CreateCollectionResult</returns>
        CreateCollectionResult CreateCollection(string name, bool? waitForSync = null, bool? doCompact = null, double? journalSize = null,
            bool? isSystem = null, bool? isVolatile = null, CollectionType? type = null, int? numberOfShards = null, string shardKeys = null, CreateCollectionKeyOption keyOptions = null, int? IndexBuckets = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// List of collections
        /// </summary>
        /// <param name="excludeSystem">Exclude system collections</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>List of collection properties</returns>
        List<CreateCollectionResult> ListCollections(bool excludeSystem = true, Action<BaseResult> baseResult = null);

        /// <summary>
        /// List of collections
        /// </summary>
        /// <param name="excludeSystem">Exclude system collections</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>List of collection properties</returns>
        Task<List<CreateCollectionResult>> ListCollectionsAsync(bool excludeSystem = true, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a collection
        /// </summary>
        /// <param name="name">Name of the collection</param>
        /// <param name="waitForSync">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="doCompact">Whether or not the collection will be compacted</param>
        /// <param name="journalSize"> The maximal size of a journal or datafile in bytes. The value must be at least 1048576 (1 MB).</param>
        /// <param name="isSystem"> If true, create a system collection. In this case collection-name should start with an underscore</param>
        /// <param name="isVolatile">If true then the collection data is kept in-memory only and not made persistent</param>
        /// <param name="type"> The type of the collection to create</param>
        /// <param name="numberOfShards">In a cluster, this value determines the number of shards to create for the collection</param>
        /// <param name="shardKeys">In a cluster, this attribute determines which document attributes are used to determine the target shard for documents</param>
        /// <returns>CreateCollectionResult</returns>
        Task<CreateCollectionResult> CreateCollectionAsync(string name, bool? waitForSync = null, bool? doCompact = null, double? journalSize = null,
            bool? isSystem = null, bool? isVolatile = null, CollectionType? type = null, int? numberOfShards = null, string shardKeys = null, CreateCollectionKeyOption keyOptions = null, int? IndexBuckets = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Graph methods container
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <returns></returns>
        IArangoGraph Graph(string graphName);

        /// <summary>
        /// Lists all graphs
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns>List<GraphIdentifierResult></returns>
        Task<List<GraphIdentifierResult>> ListGraphsAsync(Action<BaseResult> baseResult = null);

        /// <summary>
        /// Lists all graphs
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns>List<GraphIdentifierResult></returns>
        List<GraphIdentifierResult> ListGraphs(Action<BaseResult> baseResult = null);

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
        ICursor<T> Near<T>(double latitude, double longitude, Expression<Func<T, object>> distance = null, string geo = null
            , int? skip = null, int? limit = null, int? batchSize = null);

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
        ICursor<T> Within<T>(double latitude, double longitude, double radius, Expression<Func<T, object>> distance = null, string geo = null
            , int? skip = null, int? limit = null, int? batchSize = null);
        
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
        ICursor<T> Fulltext<T>(Expression<Func<T, object>> attribute, string query, string index = null
            , int? skip = null, int? limit = null, int? batchSize = null);

        /// <summary>
        /// Deletes a database
        /// </summary>
        /// <param name="name">Name of the database</param>
        /// <returns></returns>
        void DropDatabase(string name, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes a database
        /// </summary>
        /// <param name="name">Name of the database</param>
        /// <returns></returns>
        Task DropDatabaseAsync(string name, Action<BaseResult> baseResult = null);
        
        Task<TResult> ExecuteTransactionAsync<TResult>(TransactionData data, Action<BaseResult> baseResult = null);
        Task ExecuteTransactionAsync(TransactionData data, Action<BaseResult> baseResult = null);

        TResult ExecuteTransaction<TResult>(TransactionData data, Action<BaseResult> baseResult = null);
        void ExecuteTransaction(TransactionData data, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Executes a server-side traversal
        /// </summary>
        /// <typeparam name="TVertex">Type of vertex</typeparam>
        /// <typeparam name="TEdge">Type of edge</typeparam>
        /// <param name="config">Configuration for the traversal</param>
        /// <param name="startVertex">Id of the startVertex</param>
        /// <param name="baseResult"></param>
        /// <returns>TraversalResult<TVertex, TEdge></returns>
        TraversalResult<TVertex, TEdge> Traverse<TVertex, TEdge>(TraversalConfig config, string startVertex = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Executes a server-side traversal
        /// </summary>
        /// <typeparam name="TVertex">Type of vertex</typeparam>
        /// <typeparam name="TEdge">Type of edge</typeparam>
        /// <param name="config">Configuration for the traversal</param>
        /// <param name="startVertex">Id of the startVertex</param>
        /// <param name="baseResult"></param>
        /// <returns>TraversalResult<TVertex, TEdge></returns>
        Task<TraversalResult<TVertex, TEdge>> TraverseAsync<TVertex, TEdge>(TraversalConfig config, string startVertex = null, Action<BaseResult> baseResult = null);

    }
}
