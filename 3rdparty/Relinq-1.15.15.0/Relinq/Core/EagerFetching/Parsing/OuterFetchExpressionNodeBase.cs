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
using System.Linq;
using System.Linq.Expressions;
using ArangoDB.Client.Common.Remotion.Linq.Clauses;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Utilities;

namespace ArangoDB.Client.Common.Remotion.Linq.EagerFetching.Parsing
{
  /// <summary>
  /// Provides common functionality for <see cref="FetchOneExpressionNode"/> and <see cref="FetchManyExpressionNode"/>.
  /// </summary>
  public abstract class OuterFetchExpressionNodeBase : FetchExpressionNodeBase
  {
    protected OuterFetchExpressionNodeBase (MethodCallExpressionParseInfo parseInfo, LambdaExpression relatedObjectSelector)
        : base(parseInfo, relatedObjectSelector)
    {
    }

    protected abstract FetchRequestBase CreateFetchRequest ();

    protected override QueryModel ApplyNodeSpecificSemantics (QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      var existingMatchingFetchRequest =
          queryModel.ResultOperators.OfType<FetchRequestBase> ().FirstOrDefault (r => r.RelationMember.Equals (RelationMember));
      if (existingMatchingFetchRequest != null)
      {
        // Store a mapping between this node and the resultOperator so that a later ThenFetch... node may add its request to the resultOperator.
        clauseGenerationContext.AddContextInfo (this, existingMatchingFetchRequest);
        return queryModel;
      }
      else
        return base.ApplyNodeSpecificSemantics (queryModel, clauseGenerationContext);
    }

    protected override ResultOperatorBase CreateResultOperator (ClauseGenerationContext clauseGenerationContext)
    {
      var resultOperator = CreateFetchRequest();
      // Store a mapping between this node and the resultOperator so that a later ThenFetch... node may add its request to the resultOperator.
      clauseGenerationContext.AddContextInfo (this, resultOperator);
      return resultOperator;
    }
  }
}