using ArangoDB.Client.ServiceProvider;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Config
{
    public class DatabaseConfig : IDatabaseConfig
    {
        public string ConfigIdentifier { get; set; }

        public bool WaitForSync { get; set; }

        public bool ThrowForServerErrors { get; set; }

        public bool DisableChangeTracking { get; set; }
    }
}
