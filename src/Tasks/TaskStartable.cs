using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace BaseLibs.Tasks
{
    public sealed class TaskStartable<T>
    {
        readonly Func<Task<T>> starter;
        public TaskStartable(Func<Task<T>> starter = null)
        {
            this.starter = starter;
        }

        volatile TaskCompletionSource<T> newTcs_ = new TaskCompletionSource<T>();
        TaskCompletionSource<T> tcs_;

        TaskCompletionSource<T> GetTCS()
        {
            // First crab the candidate replacement. We leave a small hole while newTcs_ is null, so need to check if it was
            var ourNewTcs = Interlocked.Exchange(ref newTcs_, null);
            if (ourNewTcs == null) // unlikely
                ourNewTcs = new TaskCompletionSource<T>();

            var tcs = Interlocked.CompareExchange(ref tcs_, ourNewTcs, null);
            if (tcs == null)
            {
                // Replace the one we took
                newTcs_ = new TaskCompletionSource<T>();

                // Call starter
                if (starter != null)
                {
                    var ret = starter();
                    if (ret != null)
                        ret.ContinueWith(r => Set(ourNewTcs, r));
                    else
                        ourNewTcs.SetResult(default(T));
                }
                return ourNewTcs;
            }
            else
            {
                // Was already started, give back newTcs
                newTcs_ = ourNewTcs;
                return tcs;
            }
        }

        public Task<T> Task => GetTCS().Task;

        public void Reset() => tcs_ = null;

        static void Set(TaskCompletionSource<T> tcs, Task<T> task)
        {
            if (task.IsFaulted)
                tcs.SetException(task.Exception);
            else if (task.IsCanceled)
                tcs.SetCanceled();
            else
                tcs.SetResult(task.Result);
        }

        public void Set(Task<T> task)
        {
            var tcs = GetTCS();
            Set(tcs, task);
        }
        public void TrySet(Task<T> task)
        {
            var tcs = GetTCS();
            if (task.IsFaulted)
                tcs.TrySetException(task.Exception);
            else if (task.IsCanceled)
                tcs.TrySetCanceled();
            else
                tcs.TrySetResult(task.Result);
        }

    }
}
