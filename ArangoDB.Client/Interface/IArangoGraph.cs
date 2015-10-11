using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public interface IArangoGraph
    {
        /// <summary>
        /// Creates a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="edgeDefinitions">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="orphanCollection">Whether or not the collection will be compacted</param>
        /// <returns>CreateGraphResult</returns>
        GraphResult Create(string name, IList<EdgeDefinitionData> edgeDefinitions, IList<string> orphanCollections = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="edgeDefinitions">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="orphanCollection">Whether or not the collection will be compacted</param>
        /// <returns>CreateGraphResult</returns>
        Task<GraphResult> CreateAsync(string name, IList<EdgeDefinitionData> edgeDefinitions, IList<string> orphanCollections = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="edgeDefinitions">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="orphanCollection">Whether or not the collection will be compacted</param>
        /// <returns>CreateGraphResult</returns>
        GraphResult Create(string name, IList<EdgeDefinitionTypedData> edgeDefinitions, IList<Type> orphanCollections = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Creates a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="edgeDefinitions">If true then the data is synchronised to disk before returning from a document create, update, replace or removal operation</param>
        /// <param name="orphanCollection">Whether or not the collection will be compacted</param>
        /// <returns>CreateGraphResult</returns>
        Task<GraphResult> CreateAsync(string name, IList<EdgeDefinitionTypedData> edgeDefinitions, IList<Type> orphanCollections = null, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="dropCollections">Drop collections of this graph as well. Collections will only be dropped if they are not used in other graphs.</param>
        /// <returns></returns>
        DropGraphResult Drop(string name, bool dropCollections = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <param name="dropCollections">Drop collections of this graph as well. Collections will only be dropped if they are not used in other graphs.</param>
        /// <returns>Task</returns>
        Task<DropGraphResult> DropAsync(string name, bool dropCollections = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Get a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <returns>GraphIdentifierResult</returns>
        GraphIdentifierResult Info(string name, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Deletes a graph
        /// </summary>
        /// <param name="name">Name of the graph</param>
        /// <returns>GraphIdentifierResult</returns>
        Task<GraphIdentifierResult> InfoAsync(string name, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Lists all graphs
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns>List<GraphIdentifierResult></returns>
        Task<List<GraphIdentifierResult>> ListAsync(Action<BaseResult> baseResult = null);

        /// <summary>
        /// Lists all graphs
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns>List<GraphIdentifierResult></returns>
        List<GraphIdentifierResult> List(Action<BaseResult> baseResult = null);

        /// <summary>
        /// Lists all vertex collections used in graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<List<string>> ListVertexCollectionsAsync(string graphName, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Lists all vertex collections used in graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        List<string> ListVertexCollections(string graphName, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Add an additional vertex collection to the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="collection">The name of the collection</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> AddVertexCollectionAsync(string graphName, string collection, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Add an additional vertex collection to the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="collection">The name of the collection</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult AddVertexCollection(string graphName, string collection, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Add an additional vertex collection to the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> AddVertexCollectionAsync<T>(string graphName, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Add an additional vertex collection to the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult AddVertexCollection<T>(string graphName, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="collection">The name of the collection</param>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult RemoveVertexCollection(string graphName, string collection, bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="collection">The name of the collection</param>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> RemoveVertexCollectionAsync(string graphName, string collection, bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> RemoveVertexCollectionAsync<T>(string graphName, bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove a vertex collection form the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="dropCollection">Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult RemoveVertexCollection<T>(string graphName, bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Lists all edge definitions
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<List<string>> ListEdgeDefinitionsAsync(string graphName, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Lists all edge definitions
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        List<string> ListEdgeDefinitions(string graphName, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="collection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> ExtendEdgeDefinitionsAsync(string graphName, string collection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="collection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult ExtendEdgeDefinitions(string graphName, string collection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="collection">The types of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> ExtendEdgeDefinitionsAsync(string graphName, Type collection, IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="collection">The types of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult ExtendEdgeDefinitions(string graphName, Type collection, IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="collection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult EditEdgeDefinition(string graphName, string definitionName, string collection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null);


        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="collection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> EditEdgeDefinitionAsync(string graphName, string definitionName, string collection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="collection">The types of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> EditEdgeDefinitionAsync(string graphName, Type definitionName, Type collection, IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="collection">The types of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult EditEdgeDefinition(string graphName, Type definitionName, Type collection, IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> DeleteEdgeDefinitionAsync(string graphName, string definitionName, bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="definitionName">The name of the edge collection used in the definition</param>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult DeleteEdgeDefinition(string graphName, string definitionName, bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult DeleteEdgeDefinition<T>(string graphName, bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="graphName">The name of the graph</param>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> DeleteEdgeDefinitionAsync<T>(string graphName, bool dropCollection = false, Action<BaseResult> baseResult = null);

    }
}
