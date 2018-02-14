using ArangoDB.Client.Config;
using ArangoDB.Client.ServiceProvider;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ArangoDB.Client
{
    public interface IDatabaseConfig : IScopeItem
    {
        string ConfigIdentifier { get; set; }

        bool WaitForSync { get; set; }

        bool ThrowForServerErrors { get; set; }

        bool DisableChangeTracking { get; set; }

        string Url { get; set; }

        string Database { get; set; }

        NetworkCredential Credential { get; set; }

        NetworkCredential SystemDatabaseCredential { get; set; }

        IDatabaseCursorConfig Cursor { get; set; }

        IDatabaseLinqConfig Linq { get; set; }

        IDatabaseDocumentConfig Document { get; set; }

        IDatabaseSerializationConfig Serialization { get; set; }
    }
}
