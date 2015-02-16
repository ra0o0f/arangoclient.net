using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Utility
{
    // from http://stackoverflow.com/a/8860265/1271333
    public static class TaskUtils
    {
        public static void WaitSynchronizer(this Task task)
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.Flatten().InnerExceptions.First()).Throw();
            }
        }

        public static T ResultSynchronizer<T>(this Task<T> task)
        {
            try
            {
                return task.Result;
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.Flatten().InnerExceptions.First()).Throw();
            }

            return default(T);
        }
    }
}
