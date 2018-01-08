using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Config
{
    public class DatabaseGlobalConfigBuilder
    {
        public DatabaseGlobalConfigBuilder ConfigureLogging(Action<ILoggerFactory> configureLoggerFactory)
        {
            DatabaseConfig.ConfigureLoggerFactory = configureLoggerFactory; 
            return this;
        }
    }
}
