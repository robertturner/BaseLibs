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

        public Task<T> Task
        {
            get
            {
                var ret = System.Threading.Interlocked.CompareExchange(ref val_, newVal_, null);
                if (ret == null)
                {
                    try
                    {
                        switch (creator_)
                        {
                            case Func<Action<T>, Task> tc:
                                tc(val_.SetResult).ContinueWith(r =>
                                {
                                    if (r.IsFaulted)
                                        val_.TrySetException(r.Exception);
                                });
                                break;
                            case Func<Action<T>, LazyTask<T>, Task> tc:
                                tc(val_.SetResult, this).ContinueWith(r =>
                                {
                                    if (r.IsFaulted)
                                        val_.TrySetException(r.Exception);
                                });
                                break;
                            case Action<Action<T>> tc:
                                tc(val_.SetResult);
                                break;
                            case Action<Action<T>, LazyTask<T>> tc:
                                tc(val_.SetResult, this);
                                break;
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
