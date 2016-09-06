using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public class DatabaseSetting
    {
        private bool? _waitForSync;

        private bool? _throwForServerErrors;

        private bool? _disableChangeTracking;

        DatabaseSharedSetting sharedSetting;

        public DatabaseSetting(DatabaseSharedSetting sharedSetting)
        {
            this.sharedSetting = sharedSetting;
            this.Cursor = new DatabaseCursorSetting(sharedSetting);
            this.Document = new DatabaseDocumentSetting(sharedSetting);
            this.Linq = new DatabaseLinqSetting(sharedSetting);
            this.Logger = new DatabaseLogSetting(sharedSetting);
            this.Serialization = new DatabaseSerializationSetting(sharedSetting);
        }

        public bool WaitForSync
        {
            get
            {
                if (_waitForSync.HasValue)
                    return _waitForSync.Value;

                return sharedSetting.WaitForSync;
            }
            set { _waitForSync = value; }
        }

        public bool ThrowForServerErrors
        {
            get
            {
                if (_throwForServerErrors.HasValue)
                    return _throwForServerErrors.Value;

                return sharedSetting.ThrowForServerErrors;
            }
            set { _throwForServerErrors = value; }
        }

        public bool DisableChangeTracking
        {
            get
            {
                if (_disableChangeTracking.HasValue)
                    return _disableChangeTracking.Value;

                return sharedSetting.DisableChangeTracking;
            }
            set { _disableChangeTracking = value; }
        }

        public DatabaseCursorSetting Cursor;

        public DatabaseDocumentSetting Document;

        public DatabaseLinqSetting Linq;

        public DatabaseLogSetting Logger;

        public DatabaseSerializationSetting Serialization;
    }

    public class DatabaseLogSetting
    {
        private Action<string> _log;
        private bool? _logOnlyLightOperations;
        private bool? _aql;
        private bool? _httpRequest;
        private bool? _httpResponse;
        private bool? _httpHeaders;

        DatabaseSharedSetting sharedSetting;

        public DatabaseLogSetting(DatabaseSharedSetting sharedSetting)
        {
            this.sharedSetting = sharedSetting;
        }

        public Action<string> Log
        {
            get
            {
                if (_log != null)
                    return _log;

                return sharedSetting.Logger.Log;
            }
            set { _log = value; }
        }



        public bool LogOnlyLightOperations
        {
            get
            {
                if (_logOnlyLightOperations.HasValue)
                    return _logOnlyLightOperations.Value;

                return sharedSetting.Logger.LogOnlyLightOperations;
            }
            set { _logOnlyLightOperations = value; }
        }

        public bool Aql
        {
            get
            {
                if (_aql.HasValue)
                    return _aql.Value;

                return sharedSetting.Logger.Aql;
            }
            set { _aql = value; }
        }

        public bool HttpHeaders
        {
            get
            {
                if (_httpHeaders.HasValue)
                    return _httpHeaders.Value;

                return sharedSetting.Logger.HttpHeaders;
            }
            set { _httpHeaders = value; }
        }

        public bool HttpRequest
        {
            get
            {
                if (_httpRequest.HasValue)
                    return _httpRequest.Value;

                return sharedSetting.Logger.HttpRequest;
            }
            set { _httpRequest = value; }
        }

        public bool HttpResponse
        {
            get
            {
                if (_httpResponse.HasValue)
                    return _httpResponse.Value;

                return sharedSetting.Logger.HttpResponse;
            }
            set { _httpResponse = value; }
        }
    }

    public class DatabaseSerializationSetting
    {
        DatabaseSharedSetting sharedSetting;

        private IList<JsonConverter> _converters;

        private bool? _serializeEnumAsInteger;

        private MetadataPropertyHandling? _metadataPropertyHandling;

        public DatabaseSerializationSetting(DatabaseSharedSetting sharedSetting)
        {
            this.sharedSetting = sharedSetting;
        }

        public MetadataPropertyHandling MetadataPropertyHandling
        {
            get
            {
                if (_metadataPropertyHandling.HasValue)
                    return _metadataPropertyHandling.Value;

                return sharedSetting.Serialization.MetadataPropertyHandling;
            }
            set
            { _metadataPropertyHandling = value; }
        }

        public IList<JsonConverter> Converters
        {
            get
            {
                if (_converters != null)
                    return _converters;

                return sharedSetting.Serialization.Converters;
            }
            set
            { _converters = value; }
        }

        public bool SerializeEnumAsInteger
        {
            get
            {
                if (_serializeEnumAsInteger.HasValue)
                    return _serializeEnumAsInteger.Value;

                return sharedSetting.Serialization.SerializeEnumAsInteger;
            }
            set { _serializeEnumAsInteger = value; }
        }
    }

    public class DatabaseLinqSetting
    {
        DatabaseSharedSetting sharedSetting;

        private Func<string, string> _translateGroupByIntoName;

        public DatabaseLinqSetting(DatabaseSharedSetting sharedSetting)
        {
            this.sharedSetting = sharedSetting;
        }

        public Func<string, string> TranslateGroupByIntoName
        {
            get
            {
                if (_translateGroupByIntoName != null)
                    return _translateGroupByIntoName;

                return sharedSetting.Linq.TranslateGroupByIntoName;
            }
            set { _translateGroupByIntoName = value; }
        }
    }

    public class DatabaseCursorSetting
    {
        DatabaseSharedSetting sharedSetting;

        private int? _batchSize;

        private bool? _count;

        private TimeSpan? _ttl;

        private int? _maxPlans;

        private List<string> _rules;

        public DatabaseCursorSetting(DatabaseSharedSetting sharedSetting)
        {
            this.sharedSetting = sharedSetting;
            this.Rules = new List<string>();
        }

        public int? BatchSize
        {
            get
            {
                if (_batchSize.HasValue)
                    return _batchSize.Value;

                return sharedSetting.Cursor.BatchSize;
            }
            set { _batchSize = value; }
        }

        public bool? Count
        {
            get
            {
                if (_count.HasValue)
                    return _count.Value;

                return sharedSetting.Cursor.Count;
            }
            set { _count = value; }
        }

        public TimeSpan? Ttl
        {
            get
            {
                if (_ttl.HasValue)
                    return _ttl.Value;

                return sharedSetting.Cursor.Ttl;
            }
            set { _ttl = value; }
        }

        public int? MaxPlans
        {
            get
            {
                if (_maxPlans.HasValue)
                    return _maxPlans.Value;

                return sharedSetting.Cursor.MaxPlans;
            }
            set { _maxPlans = value; }
        }

        public List<string> Rules
        {
            get
            {
                if (_rules != null && _rules.Count != 0)
                    return _rules;

                return sharedSetting.Cursor.Rules;
            }
            set { _rules = value; }
        }
    }

    public class DatabaseDocumentSetting
    {
        DatabaseSharedSetting sharedSetting;

        private ReplacePolicy? _replacePolicy;

        private bool? _mergeObjectsOnUpdate;

        private bool? _keepNullAttributesOnUpdate;

        private bool? _throwIfDocumentDoesNotExists;

        public DatabaseDocumentSetting(DatabaseSharedSetting sharedSetting)
        {
            this.sharedSetting = sharedSetting;
        }

        public ReplacePolicy? ReplacePolicy
        {
            get
            {
                if (_replacePolicy.HasValue)
                    return _replacePolicy.Value;

                return sharedSetting.Document.ReplacePolicy;
            }
            set { _replacePolicy = value; }
        }

        public bool MergeObjectsOnUpdate
        {
            get
            {
                if (_mergeObjectsOnUpdate.HasValue)
                    return _mergeObjectsOnUpdate.Value;

                return sharedSetting.Document.MergeObjectsOnUpdate;
            }
            set { _mergeObjectsOnUpdate = value; }
        }

        public bool KeepNullAttributesOnUpdate
        {
            get
            {
                if (_keepNullAttributesOnUpdate.HasValue)
                    return _keepNullAttributesOnUpdate.Value;

                return sharedSetting.Document.KeepNullAttributesOnUpdate;
            }
            set { _keepNullAttributesOnUpdate = value; }
        }

        public bool ThrowIfDocumentDoesNotExists
        {
            get
            {
                if (_throwIfDocumentDoesNotExists.HasValue)
                    return _throwIfDocumentDoesNotExists.Value;

                return sharedSetting.Document.ThrowIfDocumentDoesNotExists;
            }
            set { _throwIfDocumentDoesNotExists = value; }
        }
    }
}
