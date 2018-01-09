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

        private TaskDelegateCacheContainer cont;
        public Type Type => cont.Type;
        public Type TCSType => cont.TCSType;

        public object Instance { get; private set; }

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

        public Task Task { get { return (Task)cont.GetTaskCaller(Instance); } }

        private class TaskDelegateCacheContainer
        {
            public Type TCSType { get; private set; }
            public Type Type { get; private set; }

            public Action<object, object> SetResCaller { get; private set; }
            public MemberGetter GetTaskCaller { get; private set; }
            public Action<object, Exception> SetExceptionCaller { get; private set; }

            public object CreateTcs()
            {
                return Activator.CreateInstance(TCSType);
            }

            public TaskDelegateCacheContainer(Type type)
            {
                Type = type;
                TCSType = typeof(TaskCompletionSource<>).MakeGenericType(type);
                SetResCaller = TCSType.GetMethod("SetResult", new Type[] { type }).CreateCustomDelegate<Action<object, object>>();
                GetTaskCaller = TCSType.GetProperty("Task").DelegateForGetProperty();
                SetExceptionCaller = TCSType.GetMethod("SetException", new Type[] { typeof(Exception) }).CreateCustomDelegate<Action<object, Exception>>();
            }
        }
    }
}
