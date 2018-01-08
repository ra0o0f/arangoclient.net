using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Config
{
    public class DatabaseDocumentConfigBuilder
    {
        DatabaseConfigBuilder configBuilder;
        IDatabaseConfig config;

        public DatabaseDocumentConfigBuilder(DatabaseConfigBuilder configBuilder, IDatabaseConfig config)
        {
            this.configBuilder = configBuilder;
            this.config = config;
        }

        public DatabaseConfigBuilder ReplacePolicy(ReplacePolicy replacePolicy)
        {
            config.Document.ReplacePolicy = replacePolicy;
            return configBuilder;
        }

        public DatabaseConfigBuilder MergeObjectsOnUpdate(bool mergeObjectsOnUpdate = true)
        {
            config.Document.MergeObjectsOnUpdate = mergeObjectsOnUpdate;
            return configBuilder;
        }

        public DatabaseConfigBuilder KeepNullAttributesOnUpdate(bool keepNullAttributesOnUpdate = true)
        {
            config.Document.KeepNullAttributesOnUpdate = keepNullAttributesOnUpdate;
            return configBuilder;
        }

        public DatabaseConfigBuilder ThrowIfDocumentDoesNotExists(bool throwIfDocumentDoesNotExists = true)
        {
            config.Document.ThrowIfDocumentDoesNotExists = throwIfDocumentDoesNotExists;
            return configBuilder;
        }
    }
}
