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

namespace ArangoDB.Client.Common.Remotion.Linq.EagerFetching
{
  /// <summary>
  /// Provides a fluent interface to recursively fetch related objects of objects which themselves are eager-fetched. All query methods
  /// are implemented as extension methods.
  /// </summary>
  /// <typeparam name="TQueried">The type of the objects returned by the query.</typeparam>
  /// <typeparam name="TFetch">The type of object from which the recursive fetch operation should be made.</typeparam>
// ReSharper disable UnusedTypeParameter
  public class FluentFetchRequest<TQueried, TFetch> : QueryableBase<TQueried>
  {
    public FluentFetchRequest (IQueryProvider provider, Expression expression)
        : base (provider, expression)
    {
    }
  }
  // ReSharper restore UnusedTypeParameter
}
