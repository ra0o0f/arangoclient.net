using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Config
{
    public class DatabaseLinqConfig : IDatabaseLinqConfig
    {
        public Func<string, string> TranslateGroupByIntoName { get; set; }
    }
}
