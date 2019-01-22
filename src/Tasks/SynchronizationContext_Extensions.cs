using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaseLibs.Tasks
{
    public static class SynchronizationContext_Extensions
    {
        static Exception UnwindException(Exception ex)
        {
            if (ex is AggregateException aex && aex.InnerExceptions.Count == 1)
                return UnwindException(aex.InnerExceptions[0]);
            return ex;
        }

        public static Task<TReturn> Invoke<TReturn>(this SynchronizationContext sync, Func<TReturn> fnc, bool async = true)
        {
            if (sync == null)
                ExThrowers.ThrowArgNull(nameof(sync));
            if (fnc == null)
                ExThrowers.ThrowArgNull(nameof(fnc));
            var tcs = new TaskCompletionSource<TReturn>();
            void ExecuteFnc()
            {
                try
                {
                    tcs.SetResult(fnc());
                }
                catch (Exception ex)
                {
                    tcs.SetException(UnwindException(ex));
                }
            }
            if (async)
                sync.Post(_ => ExecuteFnc(), null);
            else
                sync.Send(_ => ExecuteFnc(), null);
            return tcs.Task;
        }

        public static Task Invoke(this SynchronizationContext sync, Action fnc, bool async = true)
        {
            if (sync == null)
                ExThrowers.ThrowArgNull(nameof(sync));
            if (fnc == null)
                ExThrowers.ThrowArgNull(nameof(fnc));
            var tcs = new TaskCompletionSource<bool>();
            void ExecuteFnc()
            {
                try
                {
                    fnc();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(UnwindException(ex));
                }
            }
            if (async)
                sync.Post(_ => ExecuteFnc(), null);
            else
                sync.Send(_ => ExecuteFnc(), null);
            return tcs.Task;
        }
    }
}
