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
using System.Reflection;
using ArangoDB.Client.Common.Remotion.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Utilities;

namespace ArangoDB.Client.Common.Remotion.Linq.EagerFetching.Parsing
{
  /// <summary>
  /// Provides common functionality used by all expression nodes representing fetch operations.
  /// </summary>
  public abstract class FetchExpressionNodeBase : ResultOperatorExpressionNodeBase
  {
    protected FetchExpressionNodeBase (MethodCallExpressionParseInfo parseInfo, LambdaExpression relatedObjectSelector)
        : base (parseInfo, null, null)
    {
      ArgumentUtility.CheckNotNull ("relatedObjectSelector", relatedObjectSelector);

      var memberExpression = relatedObjectSelector.Body as MemberExpression;
      if (memberExpression == null)
      {
        var message = string.Format (
            "A fetch request must be a simple member access expression; '{0}' is a {1} instead.",
            relatedObjectSelector.Body,
            relatedObjectSelector.Body.GetType ().Name);
        throw new ArgumentException (message, "relatedObjectSelector");
      }

      var owner = StripConverts (memberExpression.Expression);
      if (owner.NodeType != ExpressionType.Parameter)
      {
        var message = string.Format (
            "A fetch request must be a simple member access expression of the kind o => o.Related; '{0}' is too complex.",
            relatedObjectSelector.Body);
        throw new ArgumentException (message, "relatedObjectSelector");
      }

      RelationMember = memberExpression.Member;
    }

    private Expression StripConverts (Expression expression)
    {
      while (expression.NodeType == ExpressionType.Convert)
        expression = ((UnaryExpression) expression).Operand;
      return expression;
    }

    public MemberInfo RelationMember { get; private set; }

    public override Expression Resolve (ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
    {
      ArgumentUtility.CheckNotNull ("inputParameter", inputParameter);
      ArgumentUtility.CheckNotNull ("expressionToBeResolved", expressionToBeResolved);

      return Source.Resolve (inputParameter, expressionToBeResolved, clauseGenerationContext);
    }
  }
}
