using System.IO;
using Newtonsoft.Json;

namespace ArangoDB.Client.Serialization
{
    public class ArangoJsonTextReader : JsonTextReader
    {
        public ArangoJsonTextReader(TextReader reader) : base(reader)
        {
            this.DateParseHandling = DateParseHandling.None;
        }
    }
}
