using ArangoDB.Client.ChangeTracking;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.Structure;
using ArangoDB.Client.Data;
using ArangoDB.Client.Http;
using ArangoDB.Client.Linq;
using ArangoDB.Client.Serialization;
using ArangoDB.Client.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public partial class ArangoDatabase : IArangoDatabase
    {
        private static ConcurrentDictionary<string, SharedDatabaseSetting> cachedSettings = new ConcurrentDictionary<string, SharedDatabaseSetting>();

        internal DocumentTracker ChangeTracker;

        internal HttpConnection Connection { get; set; }

        internal SharedDatabaseSetting SharedSetting { get; set; }

        public DatabaseSetting Setting { get; set; }

        public static ClientSetting ClientSetting { get; private set; }

        internal bool HttpInitialized { get; set; }

        static ArangoDatabase()
        {
            ClientSetting = new ClientSetting();
        }

        public ArangoDatabase()
        {
            SharedSetting = new SharedDatabaseSetting();
            Setting = new DatabaseSetting(SharedSetting);
            Connection = new HttpConnection(this);
            ChangeTracker = new DocumentTracker(this);
        }

        public ArangoDatabase(string url, string database)
            : this()
        {
            SharedSetting.Database = database;
            SharedSetting.Url = url;
        }

        public ArangoDatabase(SharedDatabaseSetting sharedSetting)
            : this()
        {
            SharedSetting = sharedSetting;
            Setting = new DatabaseSetting(SharedSetting);
        }

        /// <summary>
        /// Change setting for a specific identifier
        /// </summary>
        /// <param name="identifier">name of setting</param>
        public static void ChangeSetting(string identifier, Action<SharedDatabaseSetting> action)
        {
            action(FindSetting(identifier));
        }

        /// <summary>
        /// Change Default Setting
        /// </summary>
        public static void ChangeSetting(Action<SharedDatabaseSetting> action)
        {
            action(FindSetting("default"));
        }

        static SharedDatabaseSetting FindSetting(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentNullException("Setting identifier");

            SharedDatabaseSetting setting = null;
            if (cachedSettings.TryGetValue(identifier, out setting))
            {
                setting = new SharedDatabaseSetting();
                setting.SettingIdentifier = identifier;

            }
            return setting;
        }

        public static ArangoDatabase CreateWithSetting()
        {
            return CreateWithSetting("default");
        }

        public static ArangoDatabase CreateWithSetting(string identifier)
        {
            return new ArangoDatabase(FindSetting(identifier));
        }

        //public static DatabaseSetting LoadConnectionStringSetting(string connectionStringName)
        //{
        //    throw new NotImplementedException("ConnectionStringSetting");
        //}

        /// <summary>
        /// Get Document JsonObject and Identifiers
        /// </summary>
        /// <param name="id">id of document</param>
        /// <returns>A DocumentContainer</returns>
        public DocumentContainer FindDocumentInfo(string id)
        {
            return ChangeTracker.FindDocumentInfo(id);
        }

        /// <summary>
        /// Get Document JsonObject and Identifiers
        /// </summary>
        /// <param name="id">document object</param>
        /// <returns>A DocumentContainer</returns>
        public DocumentContainer FindDocumentInfo(object document)
        {
            return ChangeTracker.FindDocumentInfo(document);
        }

        public void Dispose()
        {

        }

        public AqlQueryable<T> Query<T>()
        {
            var queryParser = LinqUtility.CreateQueryParser();
            var executer = new AqlQueryExecuter(this);

            return new AqlQueryable<T>(queryParser, executer, this);
        }

        public ICursor<T> CreateStatement<T>(string query, IList<QueryParameter> bindVars = null, bool? count = null,
            int? batchSize = 0, TimeSpan? ttl = null, QueryOption options = null)
        {
            QueryData data = new QueryData();

            data.Query = query;

            data.BatchSize = Utils.ChangeIfNotSpecified<int>(batchSize, Setting.Cursor.BatchSize);
            data.Count = Utils.ChangeIfNotSpecified<bool>(count, Setting.Cursor.Count);

            if (ttl.HasValue)
                data.Ttl = ttl.Value.TotalSeconds;
            else if (Setting.Cursor.Ttl.HasValue)
                data.Ttl = Setting.Cursor.Ttl.Value.TotalSeconds;

            if (bindVars != null)
                data.BindVars = bindVars;

            if (options != null)
                data.Options = options;
            else
            {
                data.Options.MaxPlans = Setting.Cursor.MaxPlans;
                foreach (var r in Setting.Cursor.Rules)
                    options.Optimizer.Rules.Add(r);
            }

            var command = new HttpCommand(this)
            {
                Api = CommandApi.Cursor,
                Method = HttpMethod.Post,
                Command = ""
            };

            return command.CreateCursor<T>(data);
        }
    }
}
