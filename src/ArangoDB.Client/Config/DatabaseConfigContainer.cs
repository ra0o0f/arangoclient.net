using ArangoDB.Client.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Config
{
    public class DatabaseConfigContainer
    {
        private ConcurrentDictionary<string, IDatabaseConfig> _cachedConfigs = new ConcurrentDictionary<string, IDatabaseConfig>();

        public void AddOrUpdate(IDatabaseConfig config)
        {
            Utils.CheckNotNull(nameof(config.ConfigIdentifier), config.ConfigIdentifier);

            _cachedConfigs[config.ConfigIdentifier] = config;
        }

        public IDatabaseConfig Get(string identifier)
        {
            _cachedConfigs.TryGetValue(identifier, out IDatabaseConfig config);
            return config;
        }
    }
}
