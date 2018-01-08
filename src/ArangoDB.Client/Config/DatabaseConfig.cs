using ArangoDB.Client.ServiceProvider;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ArangoDB.Client.Config
{
    public class DatabaseConfig : IDatabaseConfig
    {
        public static Action<ILoggerFactory> ConfigureLoggerFactory { get; set; }

        public DatabaseConfig()
        {
            Cursor = new DatabaseCursorConfig();
        }

        public string ConfigIdentifier { get; set; }

        public bool WaitForSync { get; set; }

        public bool ThrowForServerErrors { get; set; }

        public bool DisableChangeTracking { get; set; }

        public string Url { get; set; }

        public string Database { get; set; }

        public NetworkCredential Credential { get; set; }

        public NetworkCredential SystemDatabaseCredential { get; set; }

        public IDatabaseCursorConfig Cursor { get; set; }
    }
}
