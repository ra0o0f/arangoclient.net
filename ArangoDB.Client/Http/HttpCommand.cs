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
        Collection=6
    }

    public enum CommandResultPartition
    {
        InheritedResult = 0,
        DistinctResult = 1,
        MergedResult = 2
    }

    public class HttpCommand
    {
        ArangoDatabase db;

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
                {CommandApi.Collection,"collection"}
            };
        }

        public HttpCommand(ArangoDatabase db)
        {
            this.db = db;
        }

        public CommandResultPartition ResultPartition { get; set; }

        public HttpMethod Method { get; set; }

        public bool IsSystemCommand { get; set; }

        public string Command { get; set; }

        public CommandApi Api { get; set; }

        public Dictionary<string,string> Query { get; set; }

        Uri BuildUrl()
        {
            string databaseName = IsSystemCommand ? "_system" : db.Settings.Database;

            UriBuilder builder = new UriBuilder(db.Settings.Url);
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

        ICommandResult<TResult> CreateGenericResult<TResult, TDeserialize>(Stream stream)
        {
            var serializer = new DocumentSerializer(db);

            var result = serializer.Deserialize<TDeserialize>(stream);

            return result as ICommandResult<TResult>;
        }

        ICommandResult<T> CreateInheritedResult<T>(Stream stream)
        {
            var serializer = new DocumentSerializer(db);

            InheritedCommandResult<T> result = new InheritedCommandResult<T>();

            result = serializer.Deserialize<InheritedCommandResult<T>>(stream);

            return result;
        }

        ICommandResult<T> CreateMergedResult<T>(Stream stream)
        {
            var serializer = new DocumentSerializer(db);

            DistinctCommandResult<T> result = new DistinctCommandResult<T>();

            result.Result = serializer.Deserialize<T>(stream);
            result.BaseResult = result.Result as BaseResult;

            return result;
        }

        ICommandResult<T> CreateDistinctResult<T>(HttpResponseMessage response, Stream stream)
        {
            var serializer = new DocumentSerializer(db);

            DistinctCommandResult<T> result = new DistinctCommandResult<T>();

            result.Result = response.IsSuccessStatusCode ? serializer.Deserialize<T>(stream) : default(T);
            result.BaseResult = !response.IsSuccessStatusCode ? serializer.Deserialize<BaseResult>(stream) : new BaseResult();

            return result;
        }

        public async Task<HttpResponseMessage> SendCommandAsync(object data = null)
        {
            return await db.Connection.SendCommandAsync(Method, BuildUrl(), data).ConfigureAwait(false);
        }

        public async Task<ICommandResult<T>> ExecuteCommandAsync<T>(object data = null)
        {
            var response = await SendCommandAsync(data);

            ICommandResult<T> result = null;

            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                if (ResultPartition == CommandResultPartition.InheritedResult)
                    result = CreateInheritedResult<T>(stream);

                if (ResultPartition == CommandResultPartition.DistinctResult)
                    result = CreateDistinctResult<T>(response, stream);

                if (ResultPartition == CommandResultPartition.MergedResult)
                    result = CreateMergedResult<T>(stream);
            }

            new BaseResultAnalyzer(db).ThrowIfNeeded(result.BaseResult);

            return result;
        }

        public async Task<ICommandResult<TResult>> ExecuteCommandAsync<TResult, TDeserialize>(object data = null)
        {
            var response = await SendCommandAsync(data);

            ICommandResult<TResult> result = null;

            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                result = CreateGenericResult<TResult, TDeserialize>(stream);
            }

            new BaseResultAnalyzer(db).ThrowIfNeeded(result.BaseResult);

            return result;
        }

        public ICursor<T> CreateCursor<T>(object data=null)
        {
            var asyncEnumerator = new CursorAsyncEnumerator<T>(db, this, data);
            return new Data.Cursor<T>(asyncEnumerator);
        }
    }
}
