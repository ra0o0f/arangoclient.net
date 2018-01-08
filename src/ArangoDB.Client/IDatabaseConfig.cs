using ArangoDB.Client.ServiceProvider;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client
{
    public interface IDatabaseConfig : IScopeItem
    {
        string ConfigIdentifier { get; set; }

        bool WaitForSync { get; set; }

        bool ThrowForServerErrors { get; set; }

        bool DisableChangeTracking { get; set; }
    }
}
