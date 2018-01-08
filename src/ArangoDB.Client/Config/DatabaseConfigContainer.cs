using ArangoDB.Client.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Config
{
    public class DatabaseConfigContainer
    {
        private ConcurrentDictionary<string, DatabaseConfig> _cachedConfigs = new ConcurrentDictionary<string, DatabaseConfig>();

        public void AddOrUpdate(DatabaseConfig config)
        {
            Utils.CheckNotNull(nameof(config.ConfigIdentifier), config.ConfigIdentifier);

            _cachedConfigs[config.ConfigIdentifier] = config;
        }

        public DatabaseConfig Get(string identifier)
        {
            _cachedConfigs.TryGetValue(identifier, out DatabaseConfig config);
            return config;
        }
    }
}
