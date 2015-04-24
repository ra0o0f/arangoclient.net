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
    public class DatabaseSharedSetting
    {
        public DatabaseSharedSetting()
        {
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
        }

        private bool _createCollectionOnTheFly;

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

        public DatabaseCursorSharedSetting Cursor;

        public DatabaseLinqSharedSetting Linq;

        public DatabaseDocumentSharedSetting Document;

        public DatabaseCollectionSetting Collection;

        internal DocumentIdentifierModifier IdentifierModifier;
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
        }

        public ReplacePolicy? ReplacePolicy { get; set; }

        public bool MergeObjectsOnUpdate { get; set; }

        public bool KeepNullAttributesOnUpdate { get; set; }
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
