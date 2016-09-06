using Newtonsoft.Json.Linq;
using ArangoDB.Client.Cursor;
using ArangoDB.Client.Data;
using ArangoDB.Client.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Http
{
    public enum CommandApi
    {
        Database = 0,
        Document = 1,
        Edge = 2,
        Cursor = 3,
        Simple = 4,
        AllEdges=5,
        Collection=6,
        Graph=7,
        Transaction = 8,
        Traversal = 9,
        Import=10,
        Index=11
    }

    public class HttpCommand
    {
        IArangoDatabase db;

        static Dictionary<CommandApi, string> ApiValues;

        static HttpCommand()
        {
            ApiValues = new Dictionary<CommandApi, string>()
            {
                {CommandApi.Database,"database"},
                {CommandApi.Document,"document"},
                {CommandApi.Edge,"edge"},
                {CommandApi.Cursor,"cursor"},
                {CommandApi.Simple,"simple"},
                {CommandApi.AllEdges,"edges"},
                {CommandApi.Collection,"collection"},
                {CommandApi.Graph,"gharial"},
                {CommandApi.Transaction,"transaction"},
                {CommandApi.Traversal,"traversal"},
                {CommandApi.Import,"import" },
                {CommandApi.Index,"index" }
            };
        }

        public HttpCommand(IArangoDatabase db)
        {
            this.db = db;
        }

        public HttpMethod Method { get; set; }

        public bool IsSystemCommand { get; set; }

        public string Command { get; set; }

        public CommandApi Api { get; set; }

        public Dictionary<string,string> Query { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public bool EnableChangeTracking { get; set; }

        Uri BuildUrl()
        {
            string databaseName = IsSystemCommand ? "_system" : db.SharedSetting.Database;

            UriBuilder builder = new UriBuilder(db.SharedSetting.Url);
            builder.Path = string.Format("/_db/{0}/_api/{1}", databaseName, ApiValues[Api]);

            if (!string.IsNullOrEmpty(Command))
                builder.Path += string.Format("/{0}", Command);

            // from https://gist.github.com/madd0/1366487
            if (Query != null)
                builder.Query = string.Join("&",
                    Query.Keys.Where(key => !string.IsNullOrWhiteSpace(Query[key]))
                    .Select(key => string.Format("{0}={1}", WebUtility.UrlEncode(key), WebUtility.UrlEncode(Query[key]))));

            return builder.Uri;
        }


        // TDeserialize can be : EdgesInheritedCommandResult<List<TResult>>, InheritedCommandResult<List<TResult>>
        // method should be used when TResult can be change tracked
        public async Task<ICommandResult<List<TResult>>> RequestGenericListResult<TResult, TDeserialize>(object data = null) where TDeserialize : new()
        {
            var response = await SendCommandAsync(data).ConfigureAwait(false);

            ICommandResult<List<TResult>> result = null;

            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                var serializer = new DocumentSerializer(db);

                if (EnableChangeTracking)
                {
                    result = new TDeserialize() as ICommandResult<List<TResult>>;
                    BaseResult baseResult = null;
                    result.Result = serializer.DeserializeBatchResult<TResult>(stream, out baseResult);
                    result.BaseResult = baseResult;
                }
                else
                {
                    result = serializer.Deserialize<TDeserialize>(stream) as ICommandResult<List<TResult>>;
                    if (!response.IsSuccessStatusCode)
                        result.Result = new List<TResult>();
                }
            }

            new BaseResultAnalyzer(db).ThrowIfNeeded(result.BaseResult);

            return result;
        }

        // TDeserialize can be : InheritedCommandResult<TResult>, DocumentInheritedCommandResult<TResult>
        // method should be used when TResult can be change tracked
        public async Task<ICommandResult<TResult>> RequestGenericSingleResult<TResult, TDeserialize>(object data = null, bool? throwForServerErrors = null) where TDeserialize : new()
        {
            var response = await SendCommandAsync(data).ConfigureAwait(false);

            ICommandResult<TResult> result = null;

            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                var serializer = new DocumentSerializer(db);

                if (EnableChangeTracking)
                {
                    result = new TDeserialize() as ICommandResult<TResult>;
                    BaseResult baseResult = null;
                    result.Result = serializer.DeserializeSingleResult<TResult>(stream, out baseResult);
                    result.BaseResult = baseResult;
                }
                else
                {
                    result = serializer.Deserialize<TDeserialize>(stream) as ICommandResult<TResult>;
                }
            }

            if (throwForServerErrors.HasValue == false || throwForServerErrors.Value == true)
                new BaseResultAnalyzer(db).ThrowIfNeeded(result.BaseResult);

            return result;
        }

        public async Task<List<ICommandResult<T>>> RequestMultipleMergedResult<T>(object data = null)
        {
            var results = new List<ICommandResult<T>>();
            var response = await SendCommandAsync(data).ConfigureAwait(false);

            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                var serializer = new DocumentSerializer(db);

                var _results = serializer.Deserialize<List<T>>(stream);
                foreach (var r in _results)
                {
                    var distinctResult = new DistinctCommandResult<T>
                    {
                        Result = r,
                        BaseResult = r as BaseResult
                    };

                    results.Add(distinctResult);
                }
            }
            
            return results;
        }

        // T can be any type that derived from BaseResult and results are not change tracked
        // method could be used if we dont want to change track T here
        public async Task<ICommandResult<T>> RequestMergedResult<T>(object data=null)
        {
            DistinctCommandResult<T> result = new DistinctCommandResult<T>();
            var response = await SendCommandAsync(data).ConfigureAwait(false);

            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                var serializer = new DocumentSerializer(db);
                
                result.Result = serializer.Deserialize<T>(stream);
                result.BaseResult = result.Result as BaseResult;
            }

            new BaseResultAnalyzer(db).ThrowIfNeeded(result.BaseResult);

            return result;
        }

        // T can be any type
        // method should be used when base result is provided on server errors
        public async Task<ICommandResult<T>> RequestDistinctResult<T>(object data = null, Func<StreamWriter, Task> onStreamReady = null, bool? throwForServerErrors = null)
        {
            DistinctCommandResult<T> result = new DistinctCommandResult<T>();

            HttpResponseMessage response = null;
            if (onStreamReady == null)
                response = await SendCommandAsync(data).ConfigureAwait(false);
            else
                response = await SendStreamCommandAsync(onStreamReady).ConfigureAwait(false);

            var serializer = new DocumentSerializer(db);

            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                if (EnableChangeTracking)
                {
                    JObject jObject = null;
                    result.Result = response.IsSuccessStatusCode ? serializer.DeserializeSingleResult<T>(stream, out jObject) : default(T);
                    if (response.IsSuccessStatusCode)
                        db.ChangeTracker.TrackChanges(result.Result, jObject);
                }
                else
                {
                    result.Result = response.IsSuccessStatusCode ? serializer.Deserialize<T>(stream) : default(T);
                }

                // response has not BaseResult if If-None-Match point to a match revision (status: HttpStatusCode.NotModified)
                result.BaseResult = response.IsSuccessStatusCode == false && response.StatusCode != HttpStatusCode.NotModified
                    ? serializer.Deserialize<BaseResult>(stream) 
                    : new BaseResult();
            }

            if(throwForServerErrors.HasValue == false || throwForServerErrors.Value == true)
                new BaseResultAnalyzer(db).ThrowIfNeeded(result.BaseResult);

            return result;
        }

        public async Task<HttpResponseMessage> SendCommandAsync(object data = null)
        {
            NetworkCredential credential = IsSystemCommand ? db.SharedSetting.SystemDatabaseCredential : db.SharedSetting.Credential;
            return await db.Connection.SendCommandAsync(Method, BuildUrl(), data, null, credential, Headers).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> SendStreamCommandAsync(Func<StreamWriter,Task> onStreamReady)
        {
            NetworkCredential credential = IsSystemCommand ? db.SharedSetting.SystemDatabaseCredential : db.SharedSetting.Credential;
            return await db.Connection.SendCommandAsync(Method, BuildUrl(), null, onStreamReady, credential, Headers).ConfigureAwait(false);
        }

        public ICursor<T> CreateCursor<T>(object data=null)
        {
            var asyncEnumerator = new CursorAsyncEnumerator<T>(db, this, data);
            return new Data.Cursor<T>(asyncEnumerator);
        }
    }
}
