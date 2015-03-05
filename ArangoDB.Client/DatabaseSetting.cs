using ArangoDB.Client.Http;
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
    public class DatabaseSetting
    {
        public DatabaseSetting()
        {
            Cursor = new DatabaseCursorSetting();
            Linq = new DatabaseLinqSetting();
            Document = new DatabaseDocumentSetting();
            Collection = new DatabaseCollectionSetting(this);
            IdentifierModifier = new DocumentIdentifierModifier(this);
            CreateCollectionOnTheFly = true;
            Url = "http://localhost:8529";
            Credentials = new NetworkCredential("root", "");
            SystemDatabaseCredentials = new NetworkCredential("root", "");
        }

        private string _url;

        public string Url
        { 
            get { return _url; } 
            set 
            {
                _url = new UriBuilder(value).Uri.ToString();
            }
        }

        public string Database { get; set; }

        public ICredentials Credentials { get; set; }

        public string SettingIdentifier { get; internal set; }

        public bool WaitForSync { get; set; }

        private bool _createCollectionOnTheFly;
        public bool CreateCollectionOnTheFly
        { 
            get
            {
                return _createCollectionOnTheFly && !ClusterMode;
            }
            set { _createCollectionOnTheFly = value; }
        }

        public ICredentials SystemDatabaseCredentials { get; set; }

        public bool ClusterMode { get; set; }

        public bool DisableChangeTracking { get; set; }

        public DatabaseCursorSetting Cursor;

        public DatabaseLinqSetting Linq;

        public DatabaseDocumentSetting Document;

        public DatabaseCollectionSetting Collection;

        internal DocumentIdentifierModifier IdentifierModifier;
    }

    public class DatabaseLinqSetting
    {
        public Func<string,string> TranslateGroupByIntoName { get; set; }

        public DatabaseLinqSetting()
        {
        }
    }

    public class DatabaseDocumentSetting
    {
        public ReplacePolicy? ReplacePolicy { get; set; }

        public bool MergeObjectsOnUpdate { get; set; }

        public bool KeepNullAttributesOnUpdate { get; set; }

        public DatabaseDocumentSetting()
        {
            MergeObjectsOnUpdate = true;
            KeepNullAttributesOnUpdate = true;
        }
    }

    public class DatabaseCursorSetting
    {
        public int? BatchSize { get; set; }

        public bool? Count { get; set; }

        public TimeSpan? Ttl { get; set; }

        public int? MaxPlans { get; set; }

        public readonly List<string> Rules;

        public DatabaseCursorSetting()
        {
            Rules = new List<string>();
        }
    }
}
