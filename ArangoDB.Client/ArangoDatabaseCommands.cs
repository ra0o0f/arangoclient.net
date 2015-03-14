using ArangoDB.Client.Data;
using ArangoDB.Client.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client.Utility;

namespace ArangoDB.Client
{
    public partial class ArangoDatabase
    {
        /// <summary>
        /// Retrieves information about the current database
        /// </summary>
        /// <returns>DatabaseInformation</returns>
        public DatabaseInformation CurrentDatabaseInformation()
        {
            return CurrentDatabaseInformationAsync().ResultSynchronizer();
        }

        /// <summary>
        /// Retrieves information about the current database
        /// </summary>
        /// <returns>DatabaseInformation</returns>
        public async Task<DatabaseInformation> CurrentDatabaseInformationAsync()
        {
            var command = new HttpCommand(this)
            {
                Api = CommandApi.Database,
                Method = HttpMethod.Get,
                Command = "current"
            };

            //var result = await command.ExecuteCommandAsync<DatabaseInformation>().ConfigureAwait(false);

            var result = await command.RequestGenericSingleResult<DatabaseInformation, InheritedCommandResult<DatabaseInformation>>().ConfigureAwait(false);

            return result.Result;
        }

        /// <summary>
        /// List of accessible databases
        /// </summary>
        /// <returns>List of database names</returns>
        public List<string> ListAccessibleDatabases()
        {
            return ListAccessibleDatabasesAsync().ResultSynchronizer();
        }

        /// <summary>
        /// List of accessible databases
        /// </summary>
        /// <returns>List of database names</returns>
        public async Task<List<string>> ListAccessibleDatabasesAsync()
        {
            var command = new HttpCommand(this)
            {
                Api = CommandApi.Database,
                Method = HttpMethod.Get,
                Command = "user"
            };

            var result = await command.RequestGenericListResult<string,InheritedCommandResult<List<string>>>().ConfigureAwait(false);

            return result.Result;
        }

        /// <summary>
        /// List of databases
        /// </summary>
        /// <returns>List of database names</returns>
        public List<string> ListDatabases()
        {
            return ListDatabasesAsync().ResultSynchronizer();
        }

        /// <summary>
        /// List of databases
        /// </summary>
        /// <returns>List of database names</returns>
        public async Task<List<string>> ListDatabasesAsync()
        {
            var command = new HttpCommand(this)
            {
                Api = CommandApi.Database,
                Method = HttpMethod.Get,
                Command = "",
                IsSystemCommand = true
            };

            var result = await command.RequestGenericListResult<string, InheritedCommandResult<List<string>>>().ConfigureAwait(false);

            return result.Result;
        }

        /// <summary>
        /// Creates a database
        /// </summary>
        /// <param name="name">Name of the database</param>
        /// <param name="users">list of database user</param>
        /// <returns></returns>
        public void CreateDatabase(string name, List<DatabaseUser> users = null)
        {
            CreateDatabaseAsync(name, users).WaitSynchronizer();
        }

        /// <summary>
        /// Creates a database
        /// </summary>
        /// <param name="name">Name of the database</param>
        /// <param name="users">list of database user</param>
        /// <returns></returns>
        public async Task CreateDatabaseAsync(string name, List<DatabaseUser> users = null)
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
        public CreateCollectionResult CreateCollection(string name, bool? waitForSync = null, bool? doCompact = null, decimal? journalSize = null,
            bool? isSystem = null, bool? isVolatile = null, CollectionType? type = null, int? numberOfShards = null, string shardKeys = null)
        {
            return CreateCollectionAsync(name, waitForSync, doCompact, journalSize, isSystem, isVolatile, type, numberOfShards, shardKeys).ResultSynchronizer();
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
        public async Task<CreateCollectionResult> CreateCollectionAsync(string name, bool? waitForSync = null, bool? doCompact = null, decimal? journalSize=null,
            bool? isSystem = null, bool? isVolatile = null, CollectionType? type = null, int? numberOfShards=null, string shardKeys=null)
        {
            var command = new HttpCommand(this)
            {
                Api = CommandApi.Collection,
                Method = HttpMethod.Post
            };

            var data = new CreateCollectionData
            {
                DoCompact=doCompact,
                IsSystem=isSystem,
                IsVolatile=isVolatile,
                Name=name,
                NumberOfShards=numberOfShards,
                ShardKeys=shardKeys,
                WaitForSync=waitForSync
            };

            if (type.HasValue)
                data.Type = (int)type.Value;

            var result = await command.RequestMergedResult<CreateCollectionResult>(data).ConfigureAwait(false);

            return result.Result;
        }
    }
}
