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
using ArangoDB.Client.Common.Remotion.Linq.Clauses;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Utilities;

namespace ArangoDB.Client.Common.Remotion.Linq.EagerFetching.Parsing
{
  /// <summary>
  /// Provides common functionality for <see cref="ThenFetchOneExpressionNode"/> and <see cref="ThenFetchManyExpressionNode"/>.
  /// </summary>
  public abstract class ThenFetchExpressionNodeBase : FetchExpressionNodeBase
  {
    protected ThenFetchExpressionNodeBase (MethodCallExpressionParseInfo parseInfo, LambdaExpression relatedObjectSelector)
        : base(parseInfo, relatedObjectSelector)
    {
    }

    protected abstract FetchRequestBase CreateFetchRequest ();

    protected override ResultOperatorBase CreateResultOperator (ClauseGenerationContext clauseGenerationContext)
    {
      throw new NotImplementedException ("Call ApplyNodeSpecificSemantics instead.");
    }

    protected override QueryModel ApplyNodeSpecificSemantics (QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      var previousFetchRequest = clauseGenerationContext.GetContextInfo (Source) as FetchRequestBase;
      if (previousFetchRequest == null)
        throw new NotSupportedException ("ThenFetchMany must directly follow another Fetch request.");

      var innerFetchRequest = CreateFetchRequest();
      innerFetchRequest = previousFetchRequest.GetOrAddInnerFetchRequest (innerFetchRequest);
      // Store a mapping between this node and the innerFetchRequest so that a later ThenFetch... node may add its request to the innerFetchRequest.
      clauseGenerationContext.AddContextInfo (this, innerFetchRequest);

      return queryModel;
    }
  }
}