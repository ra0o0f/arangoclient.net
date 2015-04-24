using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public class DatabaseSetting
    {
        private bool? _createCollectionOnTheFly;

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
        }

        public bool CreateCollectionOnTheFly
        {
            get
            {
                if (sharedSetting.ClusterMode == true)
                    return false;

                if (_createCollectionOnTheFly.HasValue)
                    return _createCollectionOnTheFly.Value;

                return sharedSetting.CreateCollectionOnTheFly;
            }
            set { _createCollectionOnTheFly = value; }
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
    }
}
