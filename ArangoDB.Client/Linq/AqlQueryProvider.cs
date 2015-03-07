// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using ArangoDB.Client.Common.Remotion.Linq;
using ArangoDB.Client.Common.Remotion.Linq.Clauses.StreamedData;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.Structure;
using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Linq
{
    public sealed class AqlQueryProvider : QueryProviderBase
    {
        IArangoDatabase db;

        private readonly Type _queryableType;

        static T CheckNotNull<T>(string argumentName,T actualValue)
        {
            if (actualValue == null)
                throw new ArgumentNullException(argumentName);

            return actualValue;
        }

        static Type CheckTypeIsAssignableFrom(string argumentName,Type actualType,Type expectedType)
        {
            CheckNotNull("expectedType", expectedType);
            if (actualType != null)
            {
                if (!expectedType.GetTypeInfo().IsAssignableFrom(actualType.GetTypeInfo()))
                {
                    string message = string.Format(
                        "Parameter '{0}' is a '{2}', which cannot be assigned to type '{1}'.",
                        argumentName,
                        expectedType,
                        actualType);
                    throw new ArgumentException(message, argumentName);
                }
            }

            return actualType;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultQueryProvider"/> using a custom <see cref="IQueryParser"/>.
        /// </summary>
        /// <param name="queryableType">
        ///   A type implementing <see cref="IQueryable{T}"/>. This type is used to construct the chain of query operators. Must be a generic type
        ///   definition.
        /// </param>
        /// <param name="queryParser">The <see cref="IQueryParser"/> used to parse queries. Specify an instance of 
        ///   <see cref="Parsing.Structure.QueryParser"/> for default behavior. See also <see cref="QueryParser.CreateDefault"/>.</param>
        /// <param name="executor">The <see cref="IQueryExecutor"/> used to execute queries against a specific query backend.</param>
        public AqlQueryProvider(Type queryableType, IQueryParser queryParser, IQueryExecutor executor,IArangoDatabase db)
            : base(CheckNotNull("queryParser", queryParser), CheckNotNull("executor", executor))
        {
            CheckNotNull("queryableType", queryableType);
            CheckQueryableType(queryableType);

            this.db = db;

            _queryableType = queryableType;
        }

        private void CheckQueryableType(Type queryableType)
        {
            CheckTypeIsAssignableFrom("queryableType", queryableType, typeof(IQueryable));

            TypeInfo queryableTypeInfo = queryableType.GetTypeInfo();
            if (!queryableTypeInfo.IsGenericTypeDefinition)
            {
                var message = string.Format(
                    "Expected the generic type definition of an implementation of IQueryable<T>, but was '{0}'.",
                    queryableType);
                throw new ArgumentException(message, "queryableType");
            }
            var genericArgumentCount = queryableTypeInfo.GenericTypeParameters.Length;
            if (genericArgumentCount != 1)
            {
                var message = string.Format(
                    "Expected the generic type definition of an implementation of IQueryable<T> with exactly one type argument, but found {0} arguments on '{1}.",
                    genericArgumentCount,
                    queryableType);
                throw new ArgumentException(message, "queryableType");
            }
        }

        /// <summary>
        /// Gets the type of queryable created by this provider. This is the generic type definition of an implementation of <see cref="IQueryable{T}"/>
        /// (usually a subclass of <see cref="QueryableBase{T}"/>) with exactly one type argument.
        /// </summary>
        public Type QueryableType
        {
            get { return _queryableType; }
        }

        /// <summary>
        /// Creates a new <see cref="IQueryable"/> (of type <see cref="QueryableType"/> with <typeparamref name="T"/> as its generic argument) that
        /// represents the query defined by <paramref name="expression"/> and is able to enumerate its results.
        /// </summary>
        /// <typeparam name="T">The type of the data items returned by the query.</typeparam>
        /// <param name="expression">An expression representing the query for which a <see cref="IQueryable{T}"/> should be created.</param>
        /// <returns>An <see cref="IQueryable{T}"/> that represents the query defined by <paramref name="expression"/>.</returns>
        public override IQueryable<T> CreateQuery<T>(Expression expression)
        {
            return (IQueryable<T>)Activator.CreateInstance(QueryableType.MakeGenericType(typeof(T)), this, expression,db);
        }

        public QueryData GetQueryData(Expression expression)
        {
            CheckNotNull("expression", expression);

            var queryModel = GenerateQueryModel(expression);

            var visitor = new AqlModelVisitor(db);
            visitor.VisitQueryModel(queryModel);

            visitor.QueryData.Query = visitor.QueryText.ToString();

            return visitor.QueryData;
        }
    }
}
