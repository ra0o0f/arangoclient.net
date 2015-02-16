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
using Remotion.Utilities;

namespace ArangoDB.Client.Common.Remotion.Linq.EagerFetching
{
  /// <summary>
  /// Represents a property holding one object that should be eager-fetched when a query is executed.
  /// </summary>
  public class FetchOneRequest : FetchRequestBase
  {
    public FetchOneRequest (MemberInfo relationMember)
        : base (ArgumentUtility.CheckNotNull ("relationMember", relationMember))
    {
    }

    /// <summary>
    /// Modifies the given query model for fetching, changing the <see cref="SelectClause.Selector"/> to the fetch source expression.
    /// For example, a fetch request such as <c>FetchOne (x => x.Customer)</c> will be transformed into a <see cref="SelectClause"/> selecting
    /// <c>y.Customer</c> (where <c>y</c> is what the query model originally selected).
    /// This method is called by <see cref="FetchRequestBase.CreateFetchQueryModel"/> in the process of creating the new fetch query model.
    /// </summary>
    protected override void ModifyFetchQueryModel (QueryModel fetchQueryModel)
    {
      ArgumentUtility.CheckNotNull ("fetchQueryModel", fetchQueryModel);
      fetchQueryModel.SelectClause.Selector = GetFetchedMemberExpression (fetchQueryModel.SelectClause.Selector);
    }

    /// <inheritdoc />
    public override ResultOperatorBase Clone (CloneContext cloneContext)
    {
      ArgumentUtility.CheckNotNull ("cloneContext", cloneContext);
      
      var clone = new FetchOneRequest (RelationMember);
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
