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
using System.Collections.ObjectModel;
using System.Linq;
using ArangoDB.Client.Common.Remotion.Linq.Clauses;
using Remotion.Utilities;

namespace ArangoDB.Client.Common.Remotion.Linq.EagerFetching
{
  /// <summary>
  /// Visits a <see cref="QueryModel"/>, removing all <see cref="FetchRequestBase"/> instances from its <see cref="QueryModel.ResultOperators"/>
  /// collection and returning <see cref="FetchQueryModelBuilder"/> objects for them.
  /// </summary>
  /// <remarks>
  /// Note that this visitor does not remove fetch requests from sub-queries.
  /// </remarks>
  public class FetchFilteringQueryModelVisitor : QueryModelVisitorBase
  {
    public static FetchQueryModelBuilder[] RemoveFetchRequestsFromQueryModel (QueryModel queryModel)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      var visitor = new FetchFilteringQueryModelVisitor ();
      queryModel.Accept (visitor);
      return visitor.FetchQueryModelBuilders.ToArray();
    }

    private readonly List<FetchQueryModelBuilder> _fetchQueryModelBuilders = new List<FetchQueryModelBuilder> ();

    protected FetchFilteringQueryModelVisitor ()
    {
    }

    protected ReadOnlyCollection<FetchQueryModelBuilder> FetchQueryModelBuilders
    {
      get { return new ReadOnlyCollection<FetchQueryModelBuilder> (_fetchQueryModelBuilders); }
    }

    public override void VisitResultOperator (ResultOperatorBase resultOperator, QueryModel queryModel, int index)
    {
      ArgumentUtility.CheckNotNull ("resultOperator", resultOperator);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      var fetchRequest = resultOperator as FetchRequestBase;
      if (fetchRequest != null)
      {
        queryModel.ResultOperators.RemoveAt (index);
        _fetchQueryModelBuilders.Add (new FetchQueryModelBuilder (fetchRequest, queryModel, index));
      }
    }
  }
}
