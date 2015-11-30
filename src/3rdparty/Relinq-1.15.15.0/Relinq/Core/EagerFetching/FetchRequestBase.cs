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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ArangoDB.Client.Common.Remotion.Linq.Clauses;
using ArangoDB.Client.Common.Remotion.Linq.Clauses.Expressions;
using ArangoDB.Client.Common.Remotion.Linq.Clauses.ResultOperators;
using ArangoDB.Client.Common.Remotion.Linq.Clauses.StreamedData;
using Remotion.Utilities;

namespace ArangoDB.Client.Common.Remotion.Linq.EagerFetching
{
  /// <summary>
  /// Base class for classes representing a property that should be eager-fetched when a query is executed.
  /// </summary>
  public abstract class FetchRequestBase : SequenceTypePreservingResultOperatorBase
  {
    private readonly FetchRequestCollection _innerFetchRequestCollection = new FetchRequestCollection();

    private MemberInfo _relationMember;

    protected FetchRequestBase (MemberInfo relationMember)
    {
      ArgumentUtility.CheckNotNull ("relationMember", relationMember);
      _relationMember = relationMember;
    }

    /// <summary>
    /// Gets the <see cref="MemberInfo"/> of the relation member whose contained object(s) should be fetched.
    /// </summary>
    /// <value>The relation member.</value>
    public MemberInfo RelationMember
    {
      get { return _relationMember; }
      set { _relationMember = ArgumentUtility.CheckNotNull ("value", value); }
    }

    /// <summary>
    /// Gets the inner fetch requests that were issued for this <see cref="FetchRequestBase"/>.
    /// </summary>
    /// <value>The fetch requests added via <see cref="GetOrAddInnerFetchRequest"/>.</value>
    public IEnumerable<FetchRequestBase> InnerFetchRequests
    {
      get { return _innerFetchRequestCollection.FetchRequests; }
    }

    /// <summary>
    /// Gets a the fetch query model, i.e. a new <see cref="QueryModel"/> that incorporates a given <paramref name="sourceItemQueryModel"/> as a
    /// <see cref="SubQueryExpression"/> and selects the fetched items from it.
    /// </summary>
    /// <param name="sourceItemQueryModel">A <see cref="QueryModel"/> that yields the source items for which items are to be fetched.</param>
    /// <returns>A <see cref="QueryModel"/> that selects the fetched items from <paramref name="sourceItemQueryModel"/> as a subquery.</returns>
    /// <remarks>
    /// This method does not clone the <paramref name="sourceItemQueryModel"/>, remove result operatores, etc. Use 
    /// <see cref="FetchQueryModelBuilder.GetOrCreateFetchQueryModel"/> (via <see cref="FetchFilteringQueryModelVisitor"/>) for the full algorithm.
    /// </remarks>
    public virtual QueryModel CreateFetchQueryModel (QueryModel sourceItemQueryModel)
    {
      ArgumentUtility.CheckNotNull ("sourceItemQueryModel", sourceItemQueryModel);

      var sourceItemName = sourceItemQueryModel.GetNewName ("#fetch");
      QueryModel fetchQueryModel;
      try
      {
        fetchQueryModel = sourceItemQueryModel.ConvertToSubQuery (sourceItemName);
      }
      catch (InvalidOperationException ex)
      {
        var message = string.Format (
            "The given source query model cannot be used to fetch the relation member '{0}': {1}",
            RelationMember.Name,
            ex.Message);
        throw new ArgumentException (message, "sourceItemQueryModel", ex);
      }

      ModifyFetchQueryModel (fetchQueryModel);

      return fetchQueryModel;
    }

    protected Expression GetFetchedMemberExpression (Expression source)
    {
      ArgumentUtility.CheckNotNull ("source", source);

      Assertion.DebugAssert (RelationMember.DeclaringType != null);
      if (!RelationMember.DeclaringType.GetTypeInfo().IsAssignableFrom (source.Type.GetTypeInfo()))
        source = Expression.Convert (source, RelationMember.DeclaringType);
      return Expression.MakeMemberAccess (source, RelationMember);
    }

    /// <summary>
    /// Modifies the given query model for fetching, adding new <see cref="AdditionalFromClause"/> instances and changing the 
    /// <see cref="SelectClause"/> as needed.
    /// This method is called by <see cref="CreateFetchQueryModel"/> in the process of creating the new fetch query model.
    /// </summary>
    /// <param name="fetchQueryModel">The fetch query model to modify.</param>
    protected abstract void ModifyFetchQueryModel (QueryModel fetchQueryModel);
    
    /// <summary>
    /// Gets or adds an inner eager-fetch request for this <see cref="FetchRequestBase"/>.
    /// </summary>
    /// <param name="fetchRequest">The <see cref="FetchRequestBase"/> to be added.</param>
    /// <returns>
    /// <paramref name="fetchRequest"/> or, if another <see cref="FetchRequestBase"/> for the same relation member already existed,
    /// the existing <see cref="FetchRequestBase"/>.
    /// </returns>
    public FetchRequestBase GetOrAddInnerFetchRequest (FetchRequestBase fetchRequest)
    {
      ArgumentUtility.CheckNotNull ("fetchRequest", fetchRequest);
      return _innerFetchRequestCollection.GetOrAddFetchRequest (fetchRequest);
    }

    public override StreamedSequence ExecuteInMemory<T> (StreamedSequence input)
    {
      ArgumentUtility.CheckNotNull ("input", input);
      return input;
    }

    public override string ToString ()
    {
      var result = string.Format ("Fetch ({0}.{1})", _relationMember.DeclaringType.Name, _relationMember.Name);
      var innerResults = InnerFetchRequests.Select (request => request.ToString ());
      return string.Join (".Then", new[] { result }.Concat (innerResults));
    }
  }
}
