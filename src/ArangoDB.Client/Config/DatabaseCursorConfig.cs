using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Config
{
    public class DatabaseCursorConfig : IDatabaseCursorConfig
    {
        public DatabaseCursorConfig()
        {
            Rules = new HashSet<string>();
        }
        
        public int? BatchSize { get; set; }

        public bool? Count { get; set; }

        public TimeSpan? Ttl { get; set; }

        public int? MaxPlans { get; set; }

        public HashSet<string> Rules { get; set; }
    }
}
