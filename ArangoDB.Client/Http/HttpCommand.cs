using ArangoDB.Client.Common.Newtonsoft.Json.Linq;
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
        Graph=7
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
                {CommandApi.Graph,"gharial"}
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
        public async Task<ICommandResult<TResult>> RequestGenericSingleResult<TResult, TDeserialize>(object data = null) where TDeserialize : new()
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

            new BaseResultAnalyzer(db).ThrowIfNeeded(result.BaseResult);

            return result;
        }

        // T can be any type that derived from BaseResult
        public async Task<ICommandResult<T>> RequestMergedResult<T>(object data=null)
        {
            DistinctCommandResult<T> result = new DistinctCommandResult<T>();
            var response = await SendCommandAsync(data).ConfigureAwait(false);

            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                var serializer = new DocumentSerializer(db);

                result.Result = serializer.Deserialize<T>(stream);
                result.BaseResult = result.Result as BaseResult;

                if (!response.IsSuccessStatusCode)
                    result.Result = default(T);
            }

            new BaseResultAnalyzer(db).ThrowIfNeeded(result.BaseResult);

            return result;
        }

        // T can be any type
        public async Task<ICommandResult<T>> RequestDistinctResult<T>(object data = null)
        {
            DistinctCommandResult<T> result = new DistinctCommandResult<T>();
            var response = await SendCommandAsync(data).ConfigureAwait(false);

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

                result.BaseResult = !response.IsSuccessStatusCode ? serializer.Deserialize<BaseResult>(stream) : new BaseResult { Code = (int)response.StatusCode };
            }

            new BaseResultAnalyzer(db).ThrowIfNeeded(result.BaseResult);

            return result;
        }

        public async Task<HttpResponseMessage> SendCommandAsync(object data = null)
        {
            NetworkCredential credential = IsSystemCommand ? db.SharedSetting.SystemDatabaseCredential : db.SharedSetting.Credential;
            return await db.Connection.SendCommandAsync(Method, BuildUrl(), data, credential).ConfigureAwait(false);
        }

        public ICursor<T> CreateCursor<T>(object data=null)
        {
            var asyncEnumerator = new CursorAsyncEnumerator<T>(db, this, data);
            return new Data.Cursor<T>(asyncEnumerator);
        }
    }
}
