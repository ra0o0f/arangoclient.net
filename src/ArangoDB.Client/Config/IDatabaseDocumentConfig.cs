using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Config
{
    public interface IDatabaseDocumentConfig
    {
        ReplacePolicy? ReplacePolicy { get; set; }

        bool MergeObjectsOnUpdate { get; set; }

        bool KeepNullAttributesOnUpdate { get; set; }

        bool ThrowIfDocumentDoesNotExists { get; set; }
    }
}
