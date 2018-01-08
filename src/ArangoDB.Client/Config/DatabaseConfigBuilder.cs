using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ArangoDB.Client.Config
{
    public class DatabaseConfigBuilder
    {
        IDatabaseConfig _config;

        public DatabaseConfigBuilder(IDatabaseConfig config)
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

        public DatabaseConfigBuilder Url(string url)
        {
            _config.Url = url;
            return this;
        }

        public DatabaseConfigBuilder Database(string database)
        {
            _config.Database = database;
            return this;
        }

        public DatabaseConfigBuilder Credential(string user, string password)
        {
            _config.Credential = new NetworkCredential(user, password);
            return this;
        }

        public DatabaseConfigBuilder SystemCredential(string user, string password)
        {
            _config.SystemDatabaseCredential = new NetworkCredential(user, password);
            return this;
        }
    }
}
