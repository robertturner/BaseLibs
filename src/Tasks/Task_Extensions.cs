using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaseLibs.Tasks
{
    public static class Task_Extensions
    {
        public static TaskGeneric TryGetAsGenericTask(this Task task)
        {
            return new TaskGeneric(task);
        }

        public static Task CastResultAs(this Task task, Type resultTypeToCastTo)
        {
            var genTask = task.TryGetAsGenericTask();
            var tcs = new TaskCompletionSourceGeneric(resultTypeToCastTo);
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.SetException(t.Exception);
                else if (t.IsCanceled)
                    tcs.SetCanceled();
                else
                {
                    try
                    {
                        tcs.SetResult(genTask.Result);
                    }
                    catch (InvalidCastException ex)
                    {
                        tcs.SetException(ex);
                    }
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }

        public static Task CastResultAs<T>(this Task<T> task, Type resultTypeToCastTo)
        {
            var tcs = new TaskCompletionSourceGeneric(resultTypeToCastTo);
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.SetException(t.Exception);
                else if (t.IsCanceled)
                    tcs.SetCanceled();
                else
                {
                    try
                    {
                        tcs.SetResult(t.Result);
                    }
                    catch (InvalidCastException ex)
                    {
                        tcs.SetException(ex);
                    }
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }

        public static Task<T> CastResultAs<T>(this Task task)
        {
            var tcs = new TaskCompletionSource<T>();
            var genTask = task.TryGetAsGenericTask();
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.SetException(t.Exception);
                else if (t.IsCanceled)
                    tcs.SetCanceled();
                else
                {
                    try
                    {
                        tcs.SetResult((T)genTask.Result);
                    }
                    catch (InvalidCastException ex)
                    {
                        tcs.SetException(ex);
                    }
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }

        public static Task<TRes> Transform<TArg, TRes>(this Task<TArg> task, Func<TArg, TRes> transformer)
        {
            if (transformer == null)
                ExThrowers.ThrowArgNull(nameof(transformer));
            var tcs = new TaskCompletionSource<TRes>();
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.SetException(t.Exception);
                else if (t.IsCanceled)
                    tcs.SetCanceled();
                else
                {
                    try
                    {
                        tcs.SetResult(transformer(t.Result));
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }

        public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> source) => Task.WhenAll(source);

        public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout)
        {
            if (timeout == TimeSpan.MaxValue)
                return await task;
            if (timeout == TimeSpan.Zero)
            {
                ExThrowers.ThrowTimeoutEx();
                return await task; // will never get here
            }
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                if (task == await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token)))
                    timeoutCancellationTokenSource.Cancel();
                else
                    ExThrowers.ThrowTimeoutEx();
                return await task;
            }
        }
        
    }
}
