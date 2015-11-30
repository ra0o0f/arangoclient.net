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
using System;
using System.Linq.Expressions;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Utilities;

namespace ArangoDB.Client.Common.Remotion.Linq.EagerFetching.Parsing
{
  /// <summary>
  /// Parses query operators that instruct the LINQ provider to fetch a collection-valued relationship starting from the values selected by the query. 
  /// The node creates <see cref="FetchManyRequest"/> instances and adds them to the <see cref="QueryModel"/> as 
  /// <see cref="QueryModel.ResultOperators"/> (unless the <see cref="QueryModel"/> already has an equivalent fetch request).
  /// </summary>
  /// <remarks>
  /// This class is not automatically configured for any query operator methods. LINQ provider implementations must explicitly provide and register 
  /// these  methods in order for <see cref="FetchManyExpressionNode"/> to be used. See also <see cref="FluentFetchRequest{TQueried,TFetch}"/>.
  /// </remarks>
  public class FetchManyExpressionNode : OuterFetchExpressionNodeBase
  {
    public FetchManyExpressionNode (MethodCallExpressionParseInfo parseInfo, LambdaExpression relatedObjectSelector)
        : base (parseInfo, ArgumentUtility.CheckNotNull ("relatedObjectSelector", relatedObjectSelector))
    {
    }

    protected override FetchRequestBase CreateFetchRequest ()
    {
      return new FetchManyRequest (RelationMember);
    }
  }
}
