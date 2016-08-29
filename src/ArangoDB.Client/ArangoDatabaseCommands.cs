using ArangoDB.Client.Data;
using ArangoDB.Client.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client.Utility;
using ArangoDB.Client.Graph;

namespace ArangoDB.Client
{
    public partial class ArangoDatabase
    {
        /// <summary>
        /// Retrieves information about the current database
        /// </summary>
        /// <returns>DatabaseInformation</returns>
        public DatabaseInformation CurrentDatabaseInformation(Action<BaseResult> baseResult = null)
        {
            return CurrentDatabaseInformationAsync(baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Retrieves information about the current database
        /// </summary>
        /// <returns>DatabaseInformation</returns>
        public async Task<DatabaseInformation> CurrentDatabaseInformationAsync(Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(this)
            {
                Api = CommandApi.Database,
                Method = HttpMethod.Get,
                Command = "current"
            };

            var result = await command.RequestGenericSingleResult<DatabaseInformation, InheritedCommandResult<DatabaseInformation>>().ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result;
        }

        /// <summary>
        /// List of accessible databases
        /// </summary>
        /// <returns>List of database names</returns>
        public List<string> ListAccessibleDatabases(Action<BaseResult> baseResult = null)
        {
            return ListAccessibleDatabasesAsync(baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// List of accessible databases
        /// </summary>
        /// <returns>List of database names</returns>
        public async Task<List<string>> ListAccessibleDatabasesAsync(Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(this)
            {
                Api = CommandApi.Database,
                Method = HttpMethod.Get,
                Command = "user"
            };

            var result = await command.RequestGenericListResult<string, InheritedCommandResult<List<string>>>().ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result;
        }

        /// <summary>
        /// List of databases
        /// </summary>
        /// <returns>List of database names</returns>
        public List<string> ListDatabases(Action<BaseResult> baseResult = null)
        {
            return ListDatabasesAsync(baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// List of databases
        /// </summary>
        /// <returns>List of database names</returns>
        public async Task<List<string>> ListDatabasesAsync(Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(this)
            {
                Api = CommandApi.Database,
                Method = HttpMethod.Get,
                Command = "",
                IsSystemCommand = true
            };

            var result = await command.RequestGenericListResult<string, InheritedCommandResult<List<string>>>().ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result;
        }

        /// <summary>
        /// Creates a database
        /// </summary>
        /// <param name="name">Name of the database</param>
        /// <param name="users">list of database user</param>
        /// <returns></returns>
        public void CreateDatabase(string name, List<DatabaseUser> users = null, Action<BaseResult> baseResult = null)
        {
            CreateDatabaseAsync(name, users, baseResult).WaitSynchronizer();
        }

        /// <summary>
        /// Creates a database
        /// </summary>
        /// <param name="name">Name of the database</param>
        /// <param name="users">list of database user</param>
        /// <returns></returns>
        public async Task CreateDatabaseAsync(string name, List<DatabaseUser> users = null, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(this)
            {
                Api = CommandApi.Database,
                Method = HttpMethod.Post,
                IsSystemCommand = true
            };

            var data = new CreateDatabaseData
            {
                Name = name,
                Users = users
            };

            var result = await command.RequestGenericSingleResult<bool, InheritedCommandResult<bool>>(data).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);
        }

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
        public CreateCollectionResult CreateCollection(string name, bool? waitForSync = null, bool? doCompact = null, double? journalSize = null,
            bool? isSystem = null, bool? isVolatile = null, CollectionType? type = null, int? numberOfShards = null,
            string shardKeys = null, CreateCollectionKeyOption keyOptions = null, int? IndexBuckets = null, Action<BaseResult> baseResult = null)
        {
            return CreateCollectionAsync(name, waitForSync, doCompact, journalSize, isSystem,
                isVolatile, type, numberOfShards, shardKeys, keyOptions, IndexBuckets, baseResult).ResultSynchronizer();
        }

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
        public async Task<CreateCollectionResult> CreateCollectionAsync(string name, bool? waitForSync = null, bool? doCompact = null, double? journalSize = null,
            bool? isSystem = null, bool? isVolatile = null, CollectionType? type = null, int? numberOfShards = null
            , string shardKeys = null, CreateCollectionKeyOption keyOptions = null, int? IndexBuckets = null, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(this)
            {
                Api = CommandApi.Collection,
                Method = HttpMethod.Post
            };

            var data = new CreateCollectionData
            {
                DoCompact = doCompact,
                IsSystem = isSystem,
                IsVolatile = isVolatile,
                Name = name,
                NumberOfShards = numberOfShards,
                ShardKeys = shardKeys,
                WaitForSync = waitForSync,
                JournalSize = journalSize,
                KeyOptions = keyOptions,
                IndexBuckets = IndexBuckets
            };

            if (type.HasValue)
                data.Type = (int)type.Value;

            var result = await command.RequestMergedResult<CreateCollectionResult>(data).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result;
        }

        /// <summary>
        /// Drops the collection identified by collection-name
        /// </summary>
        /// <param name="name">Name of the collection</param>
        /// <param name="baseResult"></param>
        /// <returns>DropCollectionResult</returns>
        public DropCollectionResult DropCollection(string name, Action<BaseResult> baseResult = null)
        {
            return DropCollectionAsync(name, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Drops the collection identified by collection-name
        /// </summary>
        /// <param name="name">Name of the collection</param>
        /// <param name="baseResult"></param>
        /// <returns>DropCollectionResult</returns>
        public async Task<DropCollectionResult> DropCollectionAsync(string name, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(this)
            {
                Api = CommandApi.Collection,
                Method = HttpMethod.Delete,
                Command = name
            };

            var result = await command.RequestMergedResult<DropCollectionResult>().ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result;
        }

        /// <summary>
        /// List of collections
        /// </summary>
        /// <param name="excludeSystem">Exclude system collections</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>List of collection properties</returns>
        public List<CreateCollectionResult> ListCollections(bool excludeSystem = true, Action<BaseResult> baseResult = null)
        {
            return ListCollectionsAsync(excludeSystem, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// List of collections
        /// </summary>
        /// <param name="excludeSystem">Exclude system collections</param>
        /// <param name="baseResult">Runs when base result is ready</param>
        /// <returns>List of collection properties</returns>
        public async Task<List<CreateCollectionResult>> ListCollectionsAsync(bool excludeSystem = true, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(this)
            {
                Api = CommandApi.Collection,
                Method = HttpMethod.Get,
                Query = new Dictionary<string, string>()
            };

            command.Query.Add("excludeSystem", excludeSystem.ToString());

            var result = await command.RequestGenericListResult<CreateCollectionResult, InheritedCommandResult<List<CreateCollectionResult>>>().ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result;
        }

        /// <summary>
        /// Deletes a database
        /// </summary>
        /// <param name="name">Name of the database</param>
        /// <returns></returns>
        public void DropDatabase(string name, Action<BaseResult> baseResult = null)
        {
            DropDatabaseAsync(name, baseResult).WaitSynchronizer();
        }

        /// <summary>
        /// Deletes a database
        /// </summary>
        /// <param name="name">Name of the database</param>
        /// <returns></returns>
        public async Task DropDatabaseAsync(string name, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(this)
            {
                Api = CommandApi.Database,
                Method = HttpMethod.Delete,
                IsSystemCommand = true,
                Command = name
            };

            var result = await command.RequestGenericSingleResult<bool, InheritedCommandResult<bool>>().ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);
        }

        public void ExecuteTransaction(TransactionData data, Action<BaseResult> baseResult = null)
        {
            ExecuteTransactionAsync(data, baseResult).WaitSynchronizer();
        }

        public async Task ExecuteTransactionAsync(TransactionData data, Action<BaseResult> baseResult = null)
        {
            await ExecuteTransactionAsync<object>(data, baseResult).ConfigureAwait(false);
        }

        public TResult ExecuteTransaction<TResult>(TransactionData data, Action<BaseResult> baseResult = null)
        {
            return ExecuteTransactionAsync<TResult>(data, baseResult).ResultSynchronizer();
        }

        public async Task<TResult> ExecuteTransactionAsync<TResult>(TransactionData data, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(this)
            {
                Api = CommandApi.Transaction,
                Method = HttpMethod.Post
            };

            if (data.Action != null)
                data.Action = data.Action.Replace("\r\n", " ");

            var result = await command.RequestGenericSingleResult<TResult, InheritedCommandResult<TResult>>(data).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result;
        }

        /// <summary>
        /// Graph methods container
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <returns></returns>
        public IArangoGraph Graph(string graphName)
        {
            return new ArangoGraph(this, graphName);
        }

        /// <summary>
        /// Lists all graphs
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns>List<GraphIdentifierResult></returns>
        public List<GraphIdentifierResult> ListGraphs(Action<BaseResult> baseResult = null)
        {
            return ListGraphsAsync(baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Lists all graphs
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns>List<GraphIdentifierResult></returns>
        public async Task<List<GraphIdentifierResult>> ListGraphsAsync(Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(this)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Get
            };

            var result = await command.RequestMergedResult<GraphListResult>().ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Graphs;
        }
    }
}
