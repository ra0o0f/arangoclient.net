using ArangoDB.Client.Common.Newtonsoft.Json;
using ArangoDB.Client.Serialization.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class QueryData
    {
        public string Query { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Count { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? BatchSize { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? Ttl { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(QueryParameterConverter))]
        public IList<QueryParameter> BindVars { get; set; }

        public QueryOption Options { get; set; }

        public string QueryReplacedWithVariables(IArangoDatabase db)
        {
            string query = Query;

            string[] breakWords = new string[] { "for","filter","and","sort","limit","return","collect","let"
            ,"remove","update","replace","insert","upsert"};

            foreach (var b in breakWords)
                query = query.Replace(b, Environment.NewLine + b);

            if(BindVars!=null)
                for (int i = 0; i < BindVars.Count; i++)
                    query = query.Replace($"@{BindVars[i].Name}",
                        new Serialization.DocumentSerializer(db).SerializeWithoutReader(BindVars[i].Value));
            
            return query;
        }

        public QueryData()
        {
            this.BindVars = new List<QueryParameter>();
            this.Options = new QueryOption();
        }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class QueryOption
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? FullCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxPlans { get; set; }

        public QueryOptimizerOption Optimizer { get; set; }

        public QueryOption()
        {
            Optimizer = new QueryOptimizerOption();
        }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class QueryOptimizerOption
    {
        public IList<string> Rules { get; set; }

        public QueryOptimizerOption()
        {
            Rules = new List<string>();
        }
    }

    public class QueryParameter
    {
        public string Name { get; set; }

        public object Value { get; set; }
    }
}
