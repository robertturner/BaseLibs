using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Tasks
{
    public class LazyTask<T>
    {
        readonly TaskCompletionSource<T> newVal_ = new TaskCompletionSource<T>();
        TaskCompletionSource<T> val_;
        readonly object creator_;

        public LazyTask(Action<Action<T>> creator)
        {
            creator.ThrowIfNull(nameof(creator));
            creator_ = creator;
        }
        public LazyTask(Action<Action<T>, LazyTask<T>> creator)
        {
            creator.ThrowIfNull(nameof(creator));
            creator_ = creator;
        }
        public LazyTask(Func<Action<T>, Task> creator)
        {
            creator.ThrowIfNull(nameof(creator));
            creator_ = creator;
        }
        public LazyTask(Func<Action<T>, LazyTask<T>, Task> creator)
        {
            creator.ThrowIfNull(nameof(creator));
            creator_ = creator;
        }

        public Task<T> Task
        {
            get
            {
                var ret = System.Threading.Interlocked.CompareExchange(ref val_, newVal_, null);
                if (ret == null)
                {
                    try
                    {
                        if (creator_ is Func<Action<T>, Task> tc1)
                        {
                            tc1(val_.SetResult).ContinueWith(r =>
                            {
                                if (r.IsFaulted)
                                    val_.TrySetException(r.Exception);
                            });
                        }
                        else if (creator_ is Func<Action<T>, LazyTask<T>, Task> tc2)
                        {
                            tc2(val_.SetResult, this).ContinueWith(r =>
                            {
                                if (r.IsFaulted)
                                    val_.TrySetException(r.Exception);
                            });
                        }
                        else if (creator_ is Action<Action<T>> c1)
                        {
                            c1(val_.SetResult);
                        }
                        else if (creator_ is Action<Action<T>, LazyTask<T>> c2)
                        {
                            c2(val_.SetResult, this);
                        }
                    }
                    catch (Exception ex)
                    {
                        val_.TrySetException(ex.InnerException ?? ex);
                    }
                }
                return val_.Task;
            }
        }

        public Task<T> GetWithoutStarting() => val_?.Task;

    }
}
