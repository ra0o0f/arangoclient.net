using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Config
{
    public class DatabaseDocumentConfig : IDatabaseDocumentConfig
    {
        public DatabaseDocumentConfig()
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
}
