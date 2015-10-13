using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public interface IArangoGraphEdge
    {
        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> ExtendDefinitionsAsync(IList<string> from, IList<string> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult ExtendDefinitions(IList<string> from, IList<string> to, Action<BaseResult> baseResult = null);
        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="newCollection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult EditDefinition(string newCollection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null);
        
        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="newCollection">The name of the edge collection to be used</param>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> EditDefinitionAsync(string newCollection, IList<string> from, IList<string> to, Action<BaseResult> baseResult = null);


        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> DeleteDefinitionAsync( bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult DeleteDefinition(bool dropCollection = false, Action<BaseResult> baseResult = null);

    }

    public interface IArangoGraphEdge<T>
    {
        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> ExtendDefinitionsAsync(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Add a new edge definition to the graph
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult ExtendDefinitions(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> EditDefinitionAsync<TCollection>(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Replace an existing edge definition
        /// </summary>
        /// <param name="from">One or many vertex collections that can contain source vertices</param>
        /// <param name="to">One or many edge collections that can contain target vertices</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult EditDefinition<TCollection>(IList<Type> from, IList<Type> to, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        GraphIdentifierResult DeleteDefinition(bool dropCollection = false, Action<BaseResult> baseResult = null);

        /// <summary>
        /// Remove an edge definition form the graph
        /// </summary>
        /// <param name="dropCollection"> Drop the collection as well. Collection will only be dropped if it is not used in other graphs</param>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        Task<GraphIdentifierResult> DeleteDefinitionAsync(bool dropCollection = false, Action<BaseResult> baseResult = null);

    }
}
