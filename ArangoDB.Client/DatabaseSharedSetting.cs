using ArangoDB.Client.Http;
using ArangoDB.Client.Linq;
using ArangoDB.Client.Property;
using ArangoDB.Client.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public class DatabaseSharedSetting
    {
        public DatabaseSharedSetting()
        {
            AqlFunctions = new AqlFunctionCache();
            Cursor = new DatabaseCursorSharedSetting();
            Linq = new DatabaseLinqSharedSetting();
            Document = new DatabaseDocumentSharedSetting();
            Collection = new DatabaseCollectionSetting(this);
            IdentifierModifier = new DocumentIdentifierModifier(this);
            CreateCollectionOnTheFly = true;
            Url = "http://localhost:8529";
            Credential = new NetworkCredential("root", "");
            SystemDatabaseCredential = new NetworkCredential("root", "");
            ThrowForServerErrors = true;
            Logger = new DatabaseLogSharedSetting
            {
                LogOnlyLightOperations = true,
                HttpRequest = true,
                HttpResponse = true,
                Aql = true,
                HttpHeaders = false
            };
        }

        private bool _createCollectionOnTheFly;

        private string _url;

        public string Url
        {
            get { return _url; }
            set
            {
                _url = new UriBuilder(value).Uri.ToString();
                HttpConnection.ConfigureServicePoint(_url);
            }
        }

        public string Database { get; set; }

        public NetworkCredential Credential { get; set; }

        public string SettingIdentifier { get; internal set; }

        public bool WaitForSync { get; set; }

        public bool ThrowForServerErrors { get; set; }

        public bool CreateCollectionOnTheFly
        {
            get
            {
                return _createCollectionOnTheFly && !ClusterMode;
            }
            set { _createCollectionOnTheFly = value; }
        }

        public NetworkCredential SystemDatabaseCredential { get; set; }

        public bool ClusterMode { get; set; }

        public bool DisableChangeTracking { get; set; }

        public DatabaseLogSharedSetting Logger{ get; set; }

        public DatabaseCursorSharedSetting Cursor { get; set; }

        public DatabaseLinqSharedSetting Linq { get; set; }

        public DatabaseDocumentSharedSetting Document { get; set; }

        public DatabaseCollectionSetting Collection { get; set; }

        internal DocumentIdentifierModifier IdentifierModifier;

        internal AqlFunctionCache AqlFunctions { get; set; }
    }

    public class DatabaseLogSharedSetting
    {
        public Action<string> Log { get; set; }

        public bool LogOnlyLightOperations { get; set; }

        public bool Aql { get; set; }

        public bool HttpRequest { get; set; }

        public bool HttpResponse { get; set; }

        public bool HttpHeaders { get; set; }
    }

    public class DatabaseLinqSharedSetting
    {
        public Func<string,string> TranslateGroupByIntoName { get; set; }
    }

    public class DatabaseDocumentSharedSetting
    {
        public DatabaseDocumentSharedSetting()
        {
            MergeObjectsOnUpdate = true;
            KeepNullAttributesOnUpdate = true;
            ThrowIfDocumentDoesNotExists = false;
        }

        public ReplacePolicy? ReplacePolicy { get; set; }

        public bool MergeObjectsOnUpdate { get; set; }

        public bool KeepNullAttributesOnUpdate { get; set; }

        public bool ThrowIfDocumentDoesNotExists { get; set; }
    }

    public class DatabaseCursorSharedSetting
    {
        public DatabaseCursorSharedSetting()
        {
            Rules = new List<string>();
        }

        public int? BatchSize { get; set; }

        public bool? Count { get; set; }

        public TimeSpan? Ttl { get; set; }

        public int? MaxPlans { get; set; }

        public readonly List<string> Rules;
    }
}
