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
using System.Collections.Concurrent;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace Remotion.Utilities
{
  /// <summary>
  /// Utility class for finding custom attributes via their type or an interface implemented by the type.
  /// </summary>
  static partial class AttributeUtility
  {
    private static readonly ConcurrentDictionary<Type, Lazy<AttributeUsageAttribute>> s_attributeUsageCache =
        new ConcurrentDictionary<Type, Lazy<AttributeUsageAttribute>>();

    public static bool IsAttributeInherited (Type attributeType)
    {
      AttributeUsageAttribute usage = GetAttributeUsage (attributeType);
      return usage.Inherited;
    }

    public static bool IsAttributeAllowMultiple (Type attributeType)
    {
      AttributeUsageAttribute usage = GetAttributeUsage (attributeType);
      return usage.AllowMultiple;
    }

    public static AttributeUsageAttribute GetAttributeUsage (Type attributeType)
    {
      if (attributeType == null)
        throw new ArgumentNullException ("attributeType");

      var cachedInstance = s_attributeUsageCache.GetOrAdd (attributeType, GetLazyAttributeUsage).Value;

      var newInstance = new AttributeUsageAttribute (cachedInstance.ValidOn);
      newInstance.AllowMultiple = cachedInstance.AllowMultiple;
      newInstance.Inherited = cachedInstance.Inherited;
      return newInstance;
    }

    private static Lazy<AttributeUsageAttribute> GetLazyAttributeUsage (Type attributeType)
    {
      return new Lazy<AttributeUsageAttribute> (
          () =>
          {
            var usage = (AttributeUsageAttribute[]) attributeType.GetCustomAttributes (typeof (AttributeUsageAttribute), true);
            if (usage.Length == 0)
              return new AttributeUsageAttribute (AttributeTargets.All);

            if (usage.Length > 1)
              throw new InvalidOperationException ("AttributeUsageAttribute can only be applied once.");

            return usage[0];
          },
          LazyThreadSafetyMode.ExecutionAndPublication);
    }
  }
}