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
using System.IO;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace Remotion.Utilities
{
  /// <summary>
  /// Provides utility operations for File-IO.
  /// </summary>
  /// <remarks>
  /// Deleting files in Windows can be an asynchronous operation if the system is already busy. The following code is an example that can result in 
  /// an exception because the file <c>a.txt</c> has not yet been deleted when the copy-operation is attempted.
  /// <code>
  /// File.Delete (@"C:\temp\a.txt")
  /// File.Copy (@"C:\temp\b.txt", @"C:\temp\a.txt")
  /// </code>
  /// </remarks>
  static partial class FileUtility
  {
    public static void DeleteOnDemandAndWaitForCompletion (string fileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fileName", fileName);

      if (File.Exists (fileName))
        DeleteAndWaitForCompletion (fileName);
    }

    public static void DeleteAndWaitForCompletion (string fileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fileName", fileName);

      File.Delete (fileName);
      while (File.Exists (fileName))
        Thread.Sleep (10);
    }

    public static void MoveAndWaitForCompletion (string sourceFileName, string destinationFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sourceFileName", sourceFileName);
      ArgumentUtility.CheckNotNullOrEmpty ("destinationFileName", destinationFileName);

      File.Move (sourceFileName, destinationFileName);

      if (Path.GetFullPath (sourceFileName) == Path.GetFullPath (destinationFileName))
        return;

      while (File.Exists (sourceFileName) || !File.Exists (destinationFileName))
        Thread.Sleep (10);
    }
  }
}