using System;
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
        static readonly object tdcCacheLock = new object();
        static readonly Dictionary<Type, TaskDelegateCacheContainer> tdcCache = new Dictionary<Type, TaskDelegateCacheContainer>();

        private readonly TaskDelegateCacheContainer cont;
        public Type Type => cont.Type;
        public Type TCSType => cont.TCSType;

        public object Instance { get; }

        public TaskCompletionSourceGeneric(Type type)
        {
            type.ThrowIfNull(nameof(type));
            lock (tdcCacheLock)
            {
                cont = tdcCache.GetOrSet(type, t => new TaskDelegateCacheContainer(t));
            }
            Instance = cont.Creator();
        }

        public void SetResult(object result) { cont.SetResCaller(Instance, result); }

        public void SetException(Exception exception)
        {
            exception.ThrowIfNull(nameof(exception));
            cont.SetExceptionCaller(Instance, exception);
        }

        public Task Task => (Task)cont.GetTaskCaller(Instance);

        private class TaskDelegateCacheContainer
        {
            public Type TCSType { get; }
            public Type Type { get; }

            public Action<object, object> SetResCaller { get; }
            public MemberGetter GetTaskCaller { get; }
            public Action<object, Exception> SetExceptionCaller { get; }

            public Func<object> Creator { get; }

            public TaskDelegateCacheContainer(Type type)
            {
                Type = type;
                TCSType = typeof(TaskCompletionSource<>).MakeGenericType(type);
                Creator = TCSType.GetConstructor(new Type[0]).DelegateForConstructorNoArgs();
                SetResCaller = TCSType.GetMethod("SetResult", new[] { type }).CreateCustomDelegate<Action<object, object>>();
                GetTaskCaller = TCSType.GetProperty("Task").DelegateForGetProperty();
                SetExceptionCaller = TCSType.GetMethod("SetException", new[] { typeof(Exception) }).CreateCustomDelegate<Action<object, Exception>>();
            }
        }
    }
}
