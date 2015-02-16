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
using ArangoDB.Client.Common.Remotion.Linq.Clauses;
using ArangoDB.Client.Common.Remotion.Linq.Clauses.Expressions;
using ArangoDB.Client.Common.Remotion.Linq.Utilities;
using Remotion.Utilities;

namespace ArangoDB.Client.Common.Remotion.Linq.EagerFetching
{
  /// <summary>
  /// Represents a relation collection property that should be eager-fetched by means of a lambda expression.
  /// </summary>
  public class FetchManyRequest : FetchRequestBase
  {
    private readonly Type _relatedObjectType;

    public FetchManyRequest (MemberInfo relationMember)
        : base (ArgumentUtility.CheckNotNull ("relationMember", relationMember))
    {
      var memberType = ReflectionUtility.GetMemberReturnType (relationMember);
      _relatedObjectType = ReflectionUtility.GetItemTypeOfClosedGenericIEnumerable (memberType, "relationMember");
    }

    /// <summary>
    /// Modifies the given query model for fetching, adding an <see cref="AdditionalFromClause"/> and changing the <see cref="SelectClause.Selector"/> to 
    /// retrieve the result of the <see cref="AdditionalFromClause"/>.
    /// For example, a fetch request such as <c>FetchMany (x => x.Orders)</c> will be transformed into a <see cref="AdditionalFromClause"/> selecting
    /// <c>y.Orders</c> (where <c>y</c> is what the query model originally selected) and a <see cref="SelectClause"/> selecting the result of the
    /// <see cref="AdditionalFromClause"/>.
    /// This method is called by <see cref="FetchRequestBase.CreateFetchQueryModel"/> in the process of creating the new fetch query model.
    /// </summary>
    protected override void ModifyFetchQueryModel (QueryModel fetchQueryModel)
    {
      ArgumentUtility.CheckNotNull ("fetchQueryModel", fetchQueryModel);

      var fromExpression = GetFetchedMemberExpression (new QuerySourceReferenceExpression (fetchQueryModel.MainFromClause));
      var memberFromClause = new AdditionalFromClause (fetchQueryModel.GetNewName ("#fetch"), _relatedObjectType, fromExpression);
      fetchQueryModel.BodyClauses.Add (memberFromClause);

      var newSelector = new QuerySourceReferenceExpression (memberFromClause);
      var newSelectClause = new SelectClause (newSelector);
      fetchQueryModel.SelectClause = newSelectClause;
    }

    /// <inheritdoc />
    public override ResultOperatorBase Clone (CloneContext cloneContext)
    {
      ArgumentUtility.CheckNotNull ("cloneContext", cloneContext);

      var clone = new FetchManyRequest (RelationMember);
      foreach (var innerFetchRequest in InnerFetchRequests)
        clone.GetOrAddInnerFetchRequest ((FetchRequestBase) innerFetchRequest.Clone (cloneContext));

      return clone;
    }

    /// <inheritdoc />
    public override void TransformExpressions (Func<Expression, Expression> transformation)
    {
      //nothing to do here
    }
  }
}
