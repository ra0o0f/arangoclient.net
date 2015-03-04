using ArangoDB.Client.ChangeTracking;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.Structure;
using ArangoDB.Client.Data;
using ArangoDB.Client.Http;
using ArangoDB.Client.Linq;
using ArangoDB.Client.Serialization;
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
    public partial class ArangoDatabase : IArangoDatabase, IDisposable
    {
        private static ConcurrentDictionary<string, DatabaseSetting> cachedSettings = new ConcurrentDictionary<string, DatabaseSetting>();

        internal DocumentTracker ChangeTracker;

        internal HttpConnection Connection { get; set; }

        public DatabaseSetting Settings { get; set; }

        public static ClientSetting ClientSetting { get; private set; }

        internal bool HttpInitialized { get; set; }

        static ArangoDatabase()
        {
            ClientSetting = new ClientSetting();
        }

        public ArangoDatabase()
        {
            Settings = new DatabaseSetting();
            Connection = new HttpConnection(this);
            ChangeTracker = new DocumentTracker(this);
        }

        public ArangoDatabase(string url, string database)
            : this()
        {
            Settings.Database = database;
            Settings.Url = url;
        }

        public static ArangoDatabase WithSetting(string identifier)
        {
            return new ArangoDatabase { Settings = FindSetting(identifier) };
        }

        public static ArangoDatabase WithSetting()
        {
            return new ArangoDatabase { Settings = FindSetting() };
        }

        public static DatabaseSetting FindSetting(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentNullException("Setting identifier");

            DatabaseSetting setting = null;
            if (cachedSettings.TryGetValue(identifier, out setting))
            {
                setting = new DatabaseSetting();
                setting.SettingIdentifier = identifier;

            }
            return setting;
        }

        public static DatabaseSetting FindSetting()
        {
            return FindSetting("default");
        }

        public static DatabaseSetting LoadConnectionStringSetting(string connectionStringName)
        {
            throw new NotImplementedException("ConnectionStringSetting");
        }

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

            data.BatchSize = batchSize.HasValue ? batchSize.Value : Settings.Cursor.BatchSize;
            data.Count = count.HasValue ? count.Value : Settings.Cursor.Count;

            if (ttl.HasValue)
                data.Ttl = ttl.Value.TotalSeconds;
            else if (Settings.Cursor.Ttl.HasValue)
                data.Ttl = Settings.Cursor.Ttl.Value.TotalSeconds;

            if (bindVars != null)
                data.BindVars = bindVars;

            if (options != null)
                data.Options = options;
            else
            {
                data.Options.MaxPlans = Settings.Cursor.MaxPlans;
                foreach (var r in Settings.Cursor.Rules)
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
