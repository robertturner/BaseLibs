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
        static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, TaskDelegateCacheContainer> tdcCache = new System.Collections.Concurrent.ConcurrentDictionary<Type, TaskDelegateCacheContainer>();

        private readonly TaskDelegateCacheContainer cont;
        public Type Type => cont.Type;
        public Type TCSType => cont.TCSType;

        public object Instance { get; }

        public TaskCompletionSourceGeneric(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            cont = tdcCache.GetOrSet(type, t => new TaskDelegateCacheContainer(t));
            Instance = cont.CreateTcs();
        }

        public void SetResult(object result) { cont.SetResCaller(Instance, result); }

        public void SetException(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));
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

            public object CreateTcs() => Activator.CreateInstance(TCSType);

            public TaskDelegateCacheContainer(Type type)
            {
                Type = type;
                TCSType = typeof(TaskCompletionSource<>).MakeGenericType(type);
                SetResCaller = TCSType.GetMethod("SetResult", new[] { type }).CreateCustomDelegate<Action<object, object>>();
                GetTaskCaller = TCSType.GetProperty("Task").DelegateForGetProperty();
                SetExceptionCaller = TCSType.GetMethod("SetException", new[] { typeof(Exception) }).CreateCustomDelegate<Action<object, Exception>>();
            }
        }
    }
}
