using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Config
{
    public class DatabaseConfigBuilder
    {
        DatabaseConfig _config;

        public DatabaseConfigBuilder(DatabaseConfig config)
        {
            _config = config;
        }

        public DatabaseConfigBuilder WaitForSync(bool value = true)
        {
            _config.WaitForSync = value;
            return this;
        }

        public DatabaseConfigBuilder ThrowForServerErrors(bool value = true)
        {
            _config.ThrowForServerErrors = true;
            return this;
        }

        public DatabaseConfigBuilder DisableChangeTracking(bool value = true)
        {
            _config.DisableChangeTracking = true;
            return this;
        }
    }
}
