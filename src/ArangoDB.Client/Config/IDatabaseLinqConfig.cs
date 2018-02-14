using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Config
{
    public interface IDatabaseLinqConfig
    {
        Func<string, string> TranslateGroupByIntoName { get; set; }
    }
}
