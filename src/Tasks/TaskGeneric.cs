using BaseLibs.Collections;
using BaseLibs.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Tasks
{
    [System.Diagnostics.DebuggerDisplay("Status = {Status}, Result = {ResultAsString}")]
    public sealed class TaskGeneric
    {
        static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, TaskGenDelCache> genTaskDelCache = new System.Collections.Concurrent.ConcurrentDictionary<Type, TaskGenDelCache>();

        private TaskGeneric() { }

        public static TaskGeneric TryCreateFromTask(Task task)
        {
            var genTask = task.GetType();
            for (; ; )
            {
                if (genTask == typeof(Task))
                    return null;
                if (genTask.IsGenericType)
                {
                    var t = genTask.GetGenericTypeDefinition();
                    if (t == typeof(Task<>))
                        break;
                }
                genTask = genTask.BaseType;
            }
            var resType = genTask.GetGenericArguments()[0];
            return new TaskGeneric
            {
                Instance = task,
                ResultType = resType,
                cache = genTaskDelCache.GetOrSet(resType, () => new TaskGenDelCache(genTask))
            };
        }

        public Task Instance { get; private set; }

        TaskGenDelCache cache;

        public Type GenTaskType { get { return cache.GenTaskType; } }
        public Type ResultType { get; private set; }

        class TaskGenDelCache
        {
            public TaskGenDelCache(Type genTaskType) { GenTaskType = genTaskType; }
            public Type GenTaskType { get; private set; }

            private MethodInvoker getResCaller;

            public MethodInvoker ResultCaller
            {
                get
                {
                    if (getResCaller == null)
                        getResCaller = GenTaskType.GetProperty("Result").GetMethod.DelegateForMethod();
                    return getResCaller;
                }
            }
        }

        string ResultAsString
        {
            get
            {
                if ((Instance.Status & (TaskStatus.RanToCompletion)) != 0)
                    return Result.ToString();
                return "<value not available>";
            }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public object Result { get { return cache.ResultCaller(Instance); } }
    }
}
