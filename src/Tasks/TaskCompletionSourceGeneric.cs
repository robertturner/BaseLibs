using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BaseLibs.Collections;
using BaseLibs.Types;

namespace BaseLibs.Tasks
{
    public abstract class TaskCompletionSourceGeneric
    {
        static readonly ConcurrentDictionary<Type, (Lazy<Func<object>> Ctor, Lazy<ConstructorInvoker> CtorState)> ctorCache = new ConcurrentDictionary<Type, (Lazy<Func<object>> ctor, Lazy<ConstructorInvoker> ctorState)>();

        public static TaskCompletionSourceGeneric Create(Type type)
        {
            type.ThrowIfNull(nameof(type));
            return (TaskCompletionSourceGeneric)GetCtors(type).Ctor.Value();
        }
        public static TaskCompletionSourceGeneric Create(Type type, object state)
        {
            type.ThrowIfNull(nameof(type));
            return (TaskCompletionSourceGeneric)GetCtors(type).CtorState.Value(state);
        }

        static (Lazy<Func<object>> Ctor, Lazy<ConstructorInvoker> CtorState) GetCtors(Type type)
        {
            return ctorCache.GetOrAdd(type, t =>
            {
                var genType = typeof(TCSWrapper<>).MakeGenericType(type);
                return (new Lazy<Func<object>>(() => genType.GetConstructor(new Type[0]).DelegateForConstructorNoArgs()),
                    new Lazy<ConstructorInvoker>(() => genType.GetConstructor(new Type[] { typeof(object) }).DelegateForConstructor()));
            });
        }

        private TaskCompletionSourceGeneric() { }

        public abstract Type Type { get; }

        public abstract void SetResult(object result);
        public abstract void TrySetResult(object result);
        public abstract void SetException(Exception exception);
        public abstract void TrySetException(Exception exception);
        public abstract void SetCanceled();
        public abstract void TrySetCanceled();
        public abstract Task Task { get; }

        sealed class TCSWrapper<T> : TaskCompletionSourceGeneric
        {
            readonly TaskCompletionSource<T> taskCompletionSource;

            public override Task Task => taskCompletionSource.Task;

            public override Type Type => typeof(T);

            public TCSWrapper() => taskCompletionSource = new TaskCompletionSource<T>();
            public TCSWrapper(object state) => taskCompletionSource = new TaskCompletionSource<T>(state);

            public override void SetResult(object result) => taskCompletionSource.SetResult((T)result);

            public override void TrySetResult(object result) => taskCompletionSource.TrySetResult((T)result);

            public override void SetException(Exception exception) => taskCompletionSource.SetException(exception);

            public override void TrySetException(Exception exception) => taskCompletionSource.TrySetException(exception);

            public override void SetCanceled() => taskCompletionSource.SetCanceled();

            public override void TrySetCanceled() => taskCompletionSource.TrySetCanceled();
        }
    }
}
