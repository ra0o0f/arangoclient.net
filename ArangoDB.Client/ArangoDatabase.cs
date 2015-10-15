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
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public partial class ArangoDatabase : IArangoDatabase
    {
        private static ConcurrentDictionary<string, DatabaseSharedSetting> cachedSettings = new ConcurrentDictionary<string, DatabaseSharedSetting>();

        public DocumentTracker ChangeTracker { get; set; }

        public IHttpConnection Connection { get; set; }

        public DatabaseSharedSetting SharedSetting { get; set; }

        public DatabaseSetting Setting { get; set; }

        public static ClientSetting ClientSetting { get; private set; }

        internal bool HttpInitialized { get; set; }

        static ArangoDatabase()
        {
            ClientSetting = new ClientSetting();
        }

        public ArangoDatabase()
        {
            SharedSetting = new DatabaseSharedSetting();
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

        public ArangoDatabase(DatabaseSharedSetting sharedSetting)
            : this()
        {
            SharedSetting = sharedSetting;
            Setting = new DatabaseSetting(SharedSetting);
        }

        /// <summary>
        /// Change setting for a specific identifier
        /// </summary>
        /// <param name="identifier">name of setting</param>
        public static void ChangeSetting(string identifier, Action<DatabaseSharedSetting> action)
        {
            action(FindSetting(identifier));
        }
        
        /// <summary>
        /// Change Default Setting
        /// </summary>
        public static void ChangeSetting(Action<DatabaseSharedSetting> action)
        {
            action(FindSetting("default"));
        }

        static DatabaseSharedSetting FindSetting(string identifier, bool? throwIfNotFound = false)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentNullException("Setting identifier");

            DatabaseSharedSetting setting = null;
            if (!cachedSettings.TryGetValue(identifier, out setting))
            {
                if (throwIfNotFound == true)
                    throw new InvalidOperationException(string.Format("Can not find database setting identifier '{0}'", identifier));

                setting = new DatabaseSharedSetting();
                setting.SettingIdentifier = identifier;
                cachedSettings.TryAdd(identifier, setting);
            }
            return setting;
        }

        public static IArangoDatabase CreateWithSetting()
        {
            return CreateWithSetting("default");
        }

        public static IArangoDatabase CreateWithSetting(string identifier)
        {
            return new ArangoDatabase(FindSetting(identifier, true));
        }

        public void Log(string message)
        {
            Setting.Logger.Log?.Invoke(message);
        }

        public bool LoggerAvailable
        {
            get { return Setting.Logger.Log != null; }
        }

        /// <summary>
        /// Get Loaded Document JsonObject and Identifiers
        /// </summary>
        /// <param name="id">id of document</param>
        /// <returns>A DocumentContainer</returns>
        public DocumentContainer FindDocumentInfo(string id)
        {
            return ChangeTracker.FindDocumentInfo(id);
        }

        /// <summary>
        /// Get Loaded Document JsonObject and Identifiers
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

        public IQueryable<AQL> Query()
        {
            return Query<AQL>();
        }

        public ICursor<T> CreateStatement<T>(string query, IList<QueryParameter> bindVars = null,
           bool ? count = null, int? batchSize = null, TimeSpan? ttl = null, QueryOption options = null)
        {
            QueryData data = new QueryData();

            data.Query = query;

            data.BatchSize = batchSize ?? Setting.Cursor.BatchSize;
            data.Count = count ?? Setting.Cursor.Count;

            if (ttl.HasValue)
                data.Ttl = ttl.Value.TotalSeconds;
            else if (Setting.Cursor.Ttl.HasValue)
                data.Ttl = Setting.Cursor.Ttl.Value.TotalSeconds;

            if (bindVars != null && bindVars.Count != 0)
                data.BindVars = bindVars;
            else
                data.BindVars = null;

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

            if (LoggerAvailable)
            {
                Log("==============================");
                Log(DateTime.Now.ToString());
                Log($"creating an AQL query:");
                Log($"query: {data.Query}");
                if (Setting.Logger.LogOnlyLightOperations == false && data.BindVars != null)
                {
                    Log($"bindVars:");
                    foreach (var b in data.BindVars)
                        Log($"name: {b.Name} value: {new DocumentSerializer(this).SerializeWithoutReader(b.Value)}");
                    Log("");
                    Log("parsed query with variables replaced:");
                    Log(data.QueryReplacedWithVariables(this));
                    Log("");
                }   
            }

            return command.CreateCursor<T>(data);
        }

        /// <summary>
        /// Executes a server-side traversal
        /// </summary>
        /// <typeparam name="TVertex">Type of vertex</typeparam>
        /// <typeparam name="TEdge">Type of edge</typeparam>
        /// <param name="config">Configuration for the traversal</param>
        /// <param name="startVertex">Id of the startVertex</param>
        /// <param name="baseResult"></param>
        /// <returns>TraversalResult<TVertex, TEdge></returns>
        public TraversalResult<TVertex, TEdge> Traverse<TVertex, TEdge>(TraversalConfig config, string startVertex = null, Action<BaseResult> baseResult = null)
        {
            return TraverseAsync<TVertex, TEdge>(config, startVertex, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Executes a server-side traversal
        /// </summary>
        /// <typeparam name="TVertex">Type of vertex</typeparam>
        /// <typeparam name="TEdge">Type of edge</typeparam>
        /// <param name="config">Configuration for the traversal</param>
        /// <param name="startVertex">Id of the startVertex</param>
        /// <param name="baseResult"></param>
        /// <returns>TraversalResult<TVertex, TEdge></returns>
        public async Task<TraversalResult<TVertex, TEdge>> TraverseAsync<TVertex, TEdge>(TraversalConfig config, string startVertex = null, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(this)
            {
                Api = CommandApi.Traversal,
                Method = HttpMethod.Post
            };

            config.StartVertex = startVertex ?? config.StartVertex;

            var result = await command.RequestMergedResult<TraversalContainerResult<TVertex, TEdge>>(config).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Result;
        }
    }
}
