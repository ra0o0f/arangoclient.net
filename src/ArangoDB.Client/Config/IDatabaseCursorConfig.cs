using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Config
{
    public interface IDatabaseCursorConfig
    {
        int? BatchSize { get; set; }

        bool? Count { get; set; }

        TimeSpan? Ttl { get; set; }

        int? MaxPlans { get; set; }

        HashSet<string> Rules { get; set; }
    }
}
