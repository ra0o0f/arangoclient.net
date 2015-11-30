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
using System.Reflection;
using Remotion.Utilities;

namespace ArangoDB.Client.Common.Remotion.Linq.EagerFetching
{
  /// <summary>
  /// Holds a number of <see cref="FetchManyRequest"/> instances keyed by the <see cref="MemberInfo"/> instances representing the relation members
  /// to be eager-fetched.
  /// </summary>
  public sealed class FetchRequestCollection
  {
    private readonly Dictionary<MemberInfo, FetchRequestBase> _fetchRequests = new Dictionary<MemberInfo, FetchRequestBase>();

    public IEnumerable<FetchRequestBase> FetchRequests
    {
      get { return _fetchRequests.Values; }
    }

    /// <summary>
    /// Gets or adds an eager-fetch request to this <see cref="FetchRequestCollection"/>.
    /// </summary>
    /// <param name="fetchRequest">The <see cref="FetchRequestBase"/> to be added.</param>
    /// <returns>
    /// <paramref name="fetchRequest"/> or, if another <see cref="FetchRequestBase"/> for the same relation member already existed,
    /// the existing <see cref="FetchRequestBase"/>.
    /// </returns>
    public FetchRequestBase GetOrAddFetchRequest (FetchRequestBase fetchRequest)
    {
      ArgumentUtility.CheckNotNull ("fetchRequest", fetchRequest);

      FetchRequestBase existingFetchRequest;
      if (_fetchRequests.TryGetValue (fetchRequest.RelationMember, out existingFetchRequest))
        return existingFetchRequest;
      else
      {
        _fetchRequests.Add (fetchRequest.RelationMember, fetchRequest);
        return fetchRequest;
      }
    }
  }
}
