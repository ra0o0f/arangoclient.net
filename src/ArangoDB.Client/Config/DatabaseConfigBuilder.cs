using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ArangoDB.Client.Config
{
    public class DatabaseConfigBuilder
    {
        IDatabaseConfig config;

        public DatabaseCursorConfigBuilder Cursor { get; set; }
        public DatabaseLinqConfigBuilder Linq { get; set; }


        public DatabaseConfigBuilder(IDatabaseConfig config)
        {
            this.config = config;
            Cursor = new DatabaseCursorConfigBuilder(this, config);
            Linq = new DatabaseLinqConfigBuilder(this, config);
        }

        public DatabaseConfigBuilder WaitForSync(bool value = true)
        {
            config.WaitForSync = value;
            return this;
        }

        public DatabaseConfigBuilder ThrowForServerErrors(bool value = true)
        {
            config.ThrowForServerErrors = true;
            return this;
        }

        public DatabaseConfigBuilder DisableChangeTracking(bool value = true)
        {
            config.DisableChangeTracking = true;
            return this;
        }

        public DatabaseConfigBuilder Url(string url)
        {
            config.Url = url;
            return this;
        }

        public DatabaseConfigBuilder Database(string database)
        {
            config.Database = database;
            return this;
        }

        public DatabaseConfigBuilder Credential(string user, string password)
        {
            config.Credential = new NetworkCredential(user, password);
            return this;
        }

        public DatabaseConfigBuilder SystemCredential(string user, string password)
        {
            config.SystemDatabaseCredential = new NetworkCredential(user, password);
            return this;
        }
    }
}
