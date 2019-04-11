using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibs.Collections;
using BaseLibs.Types;

namespace BaseLibs.Tasks
{
    public sealed class TaskCompletionSourceGeneric
    {
        static readonly ConcurrentDictionary<Type, TaskDelegateCacheContainer> tdcCache = new ConcurrentDictionary<Type, TaskDelegateCacheContainer>();

        private readonly TaskDelegateCacheContainer cont;
        public Type Type => cont.Type;
        public Type TCSType => cont.TCSType;

        public object Instance { get; }

        public TaskCompletionSourceGeneric(Type type)
        {
            type.ThrowIfNull(nameof(type));
            cont = tdcCache.GetOrAdd(type, t => new TaskDelegateCacheContainer(t));
            Instance = cont.Creator.Value();
        }
        public TaskCompletionSourceGeneric(Type type, object state)
        {
            type.ThrowIfNull(nameof(type));
            cont = tdcCache.GetOrAdd(type, t => new TaskDelegateCacheContainer(t));
            Instance = cont.CreatorState.Value(state);
        }

        public void SetResult(object result) { cont.SetResCaller(Instance, result); }
        public void TrySetResult(object result) { cont.TrySetResCaller.Value(Instance, result); }

        public void SetException(Exception exception)
        {
            exception.ThrowIfNull(nameof(exception));
            cont.SetExceptionCaller.Value(Instance, exception);
        }
        public void TrySetException(Exception exception)
        {
            exception.ThrowIfNull(nameof(exception));
            cont.TrySetExceptionCaller.Value(Instance, exception);
        }

        public void SetCanceled() => cont.SetCanceledCaller.Value(Instance);
        public void TrySetCanceled() => cont.TrySetCanceledCaller.Value(Instance);

        public Task Task => cont.GetTaskCaller(Instance);

        private class TaskDelegateCacheContainer
        {
            public Type TCSType { get; }
            public Type Type { get; }

            public Action<object, object> SetResCaller { get; }
            public Lazy<Action<object, object>> TrySetResCaller { get; }
            public MemberGetter<Task> GetTaskCaller { get; }
            public Lazy<Action<object, Exception>> SetExceptionCaller { get; }
            public Lazy<Action<object, Exception>> TrySetExceptionCaller { get; }
            public Lazy<Action<object>> SetCanceledCaller { get; }
            public Lazy<Action<object>> TrySetCanceledCaller { get; }

            public Lazy<Func<object>> Creator { get; }
            public Lazy<ConstructorInvoker> CreatorState { get; }

            public TaskDelegateCacheContainer(Type type)
            {
                Type = type;
                TCSType = typeof(TaskCompletionSource<>).MakeGenericType(type);
                Creator = new Lazy<Func<object>>(() => TCSType.GetConstructor(new Type[0]).DelegateForConstructorNoArgs(), true);
                CreatorState = new Lazy<ConstructorInvoker>(() => TCSType.GetConstructor(new Type[] { typeof(object) }).DelegateForConstructor(), true);
                SetResCaller = TCSType.GetMethod(nameof(TaskCompletionSource<object>.SetResult), new[] { type }).CreateCustomDelegate<Action<object, object>>();
                TrySetResCaller = new Lazy<Action<object, object>>(() => TCSType.GetMethod(nameof(TaskCompletionSource<object>.TrySetResult), new[] { type }).CreateCustomDelegate<Action<object, object>>(), true);
                GetTaskCaller = TCSType.GetProperty(nameof(TaskCompletionSource<object>.Task)).DelegateForGetProperty<Task>();
                SetExceptionCaller = new Lazy<Action<object, Exception>>(() => TCSType.GetMethod(nameof(TaskCompletionSource<object>.SetException), new[] { typeof(Exception) }).CreateCustomDelegate<Action<object, Exception>>(), true);
                TrySetExceptionCaller = new Lazy<Action<object, Exception>>(() => TCSType.GetMethod(nameof(TaskCompletionSource<object>.TrySetException), new[] { typeof(Exception) }).CreateCustomDelegate<Action<object, Exception>>(), true);
                SetCanceledCaller = new Lazy<Action<object>>(() => TCSType.GetMethod(nameof(TaskCompletionSource<object>.SetCanceled)).CreateCustomDelegate<Action<object>>(), true);
                TrySetCanceledCaller = new Lazy<Action<object>>(() => TCSType.GetMethod(nameof(TaskCompletionSource<object>.TrySetCanceled)).CreateCustomDelegate<Action<object>>(), true);
            }
        }
    }
}
