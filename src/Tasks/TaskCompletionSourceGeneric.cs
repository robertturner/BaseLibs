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
            Instance = cont.Creator();
        }

        public void SetResult(object result) { cont.SetResCaller(Instance, result); }

        public void SetException(Exception exception)
        {
            exception.ThrowIfNull(nameof(exception));
            cont.SetExceptionCaller(Instance, exception);
        }

        public void SetCanceled()
        {
            cont.SetCanceledCaller(Instance);
        }

        public Task Task => (Task)cont.GetTaskCaller(Instance);

        private class TaskDelegateCacheContainer
        {
            public Type TCSType { get; }
            public Type Type { get; }

            public Action<object, object> SetResCaller { get; }
            public MemberGetter GetTaskCaller { get; }
            public Action<object, Exception> SetExceptionCaller { get; }
            public Action<object> SetCanceledCaller { get; }

            public Func<object> Creator { get; }

            public TaskDelegateCacheContainer(Type type)
            {
                Type = type;
                TCSType = typeof(TaskCompletionSource<>).MakeGenericType(type);
                Creator = TCSType.GetConstructor(new Type[0]).DelegateForConstructorNoArgs();
                SetResCaller = TCSType.GetMethod(nameof(TaskCompletionSource<object>.SetResult), new[] { type }).CreateCustomDelegate<Action<object, object>>();
                GetTaskCaller = TCSType.GetProperty(nameof(TaskCompletionSource<object>.Task)).DelegateForGetProperty();
                SetExceptionCaller = TCSType.GetMethod(nameof(TaskCompletionSource<object>.SetException), new[] { typeof(Exception) }).CreateCustomDelegate<Action<object, Exception>>();
                SetCanceledCaller = TCSType.GetMethod(nameof(TaskCompletionSource<object>.SetCanceled)).CreateCustomDelegate<Action<object>>();
            }
        }
    }
}
