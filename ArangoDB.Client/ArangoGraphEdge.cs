using ArangoDB.Client.Data;
using ArangoDB.Client.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client.Utility;

namespace ArangoDB.Client
{
    public class ArangoGraphEdge : IArangoGraphEdge
    {
        IArangoDatabase db;
        string graphName;
        string collection;

        public ArangoGraphEdge(IArangoDatabase db, string graphName, string collection)
        {
            this.db = db;
            this.graphName = graphName;
            this.collection = collection;
        }

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult ExtendDefinitions(IList<string> from, IList<string> to, Action<BaseResult> baseResult = null)
        {
            return ExtendDefinitionsAsync(from, to, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> ExtendDefinitionsAsync(IList<string> from, IList<string> to, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Post,
                Command = $"{graphName}/edge"
            };

            EdgeDefinitionData data = new EdgeDefinitionData
            {
                Collection = collection,
                From = from,
                To = to
            };

            var result = await command.RequestMergedResult<GraphResult>(data).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Graph;
        }

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="newCollection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult EditDefinition(string newCollection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null)
        {
            return EditDefinitionAsync(newCollection, from, to, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="newCollection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> EditDefinitionAsync(string newCollection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Post,
                Command = $"{graphName}/edge/{collection}"
            };

            EdgeDefinitionData data = new EdgeDefinitionData
            {
                Collection = newCollection,
                From = from,
                To = to
            };

            var result = await command.RequestMergedResult<GraphResult>(data).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Graph;
        }


        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult DeleteDefinition(bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            return DeleteDefinitionAsync(dropCollection, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> DeleteDefinitionAsync(bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            var command = new HttpCommand(db)
            {
                Api = CommandApi.Graph,
                Method = HttpMethod.Delete,
                Command = $"{graphName}/edge/{collection}"
            };

            DropGraphCollectionData data = new DropGraphCollectionData
            {
                DropCollection = dropCollection
            };

            var result = await command.RequestMergedResult<GraphResult>(data).ConfigureAwait(false);

            if (baseResult != null)
                baseResult(result.BaseResult);

            return result.Result.Graph;
        }
    }

    public class ArangoGraphEdge<T> : IArangoGraphEdge<T>
    {
        IArangoDatabase db;
        string graphName;
        IArangoGraphEdge collectionMethods;


        public ArangoGraphEdge(IArangoDatabase db, string graphName)
        {
            this.db = db;
            this.graphName = graphName;
            collectionMethods = new ArangoGraphEdge(db, graphName, db.SharedSetting.Collection.ResolveCollectionName<T>());
        }

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> ExtendDefinitionsAsync(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null)
        {
            var fromNames = from.Select(f => db.SharedSetting.Collection.ResolveCollectionName(f)).ToList();
            var toNames = to.Select(t => db.SharedSetting.Collection.ResolveCollectionName(t)).ToList();

            return await collectionMethods.ExtendDefinitionsAsync(fromNames, toNames, baseResult);
        }

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult ExtendDefinitions(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null)
        {
            return ExtendDefinitionsAsync(from, to, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> EditDefinitionAsync<TCollection>(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null)
        {
            var newCollection = db.SharedSetting.Collection.ResolveCollectionName<TCollection>();
            var fromNames = from.Select(f => db.SharedSetting.Collection.ResolveCollectionName(f)).ToList();
            var toNames = to.Select(t => db.SharedSetting.Collection.ResolveCollectionName(t)).ToList();

            return await collectionMethods.EditDefinitionAsync(newCollection, fromNames, toNames, baseResult).ConfigureAwait(false);
        }

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult EditDefinition<TCollection>(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null)
        {
            return EditDefinitionAsync<TCollection>(from, to, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public GraphIdentifierResult DeleteDefinition(bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            return DeleteDefinitionAsync(dropCollection, baseResult).ResultSynchronizer();
        }

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        public async Task<GraphIdentifierResult> DeleteDefinitionAsync(bool dropCollection = false, Action<BaseResult> baseResult = null)
        {
            return await collectionMethods.DeleteDefinitionAsync(dropCollection, baseResult).ConfigureAwait(false);
        }
    }
}
