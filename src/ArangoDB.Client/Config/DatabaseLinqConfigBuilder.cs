using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Config
{
    public class DatabaseLinqConfigBuilder
    {
        DatabaseConfigBuilder configBuilder;
        IDatabaseConfig config;

        public DatabaseLinqConfigBuilder(DatabaseConfigBuilder configBuilder, IDatabaseConfig config)
        {
            this.configBuilder = configBuilder;
            this.config = config;
        }

        public DatabaseConfigBuilder TranslateGroupByIntoName(Func<string, string> translateGroupByIntoName)
        {
            config.Linq.TranslateGroupByIntoName = translateGroupByIntoName;
            return configBuilder;
        }
    }
}
