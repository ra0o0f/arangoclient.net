using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Config
{
    public class DatabaseCursorConfigBuilder
    {
        DatabaseConfigBuilder configBuilder;
        IDatabaseConfig config;

        public DatabaseCursorConfigBuilder(DatabaseConfigBuilder configBuilder, IDatabaseConfig config)
        {
            this.configBuilder = configBuilder;
            this.config = config;
        }

        public DatabaseConfigBuilder BatchSize(int batchSize)
        {
            config.Cursor.BatchSize = batchSize;
            return configBuilder;
        }

        public DatabaseConfigBuilder Count(bool count = true)
        {
            config.Cursor.Count = count;
            return configBuilder;
        }

        public DatabaseConfigBuilder Ttl(TimeSpan ttl)
        {
            config.Cursor.Ttl = ttl;
            return configBuilder;
        }

        public DatabaseConfigBuilder MaxPlans(int? maxPlans)
        {
            config.Cursor.MaxPlans = maxPlans;
            return configBuilder;
        }

        public DatabaseConfigBuilder AddRule(string rule)
        {
            config.Cursor.Rules.Add(rule);
            return configBuilder;
        }
    }
}
