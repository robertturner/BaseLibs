using BaseLibs.Collections;
using BaseLibs.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Tasks
{
    [System.Diagnostics.DebuggerDisplay("Status = {Instance.Status}, Result = {ResultAsString}")]
    public sealed class TaskGeneric
    {
        static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, TaskGenDelCache> genTaskDelCache = new System.Collections.Concurrent.ConcurrentDictionary<Type, TaskGenDelCache>();

        public TaskGeneric(Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));
            var genTask = task.GetType();
            for (;;)
            {
                if (genTask == typeof(Task))
                    throw new ArgumentException("task does not have Result");
                if (genTask.IsGenericType)
                {
                    var t = genTask.GetGenericTypeDefinition();
                    if (t == typeof(Task<>))
                        break;
                }
                genTask = genTask.BaseType;
            }
            Instance = task;
            ResultType = genTask.GetGenericArguments()[0];
            cache = genTaskDelCache.GetOrSet(ResultType, () => new TaskGenDelCache(genTask));
        }

        public Task Instance { get; }

        readonly TaskGenDelCache cache;

        public Type GenTaskType => cache.GenTaskType;
        public Type ResultType { get; }

        class TaskGenDelCache
        {
            public TaskGenDelCache(Type genTaskType)
            {
                GenTaskType = genTaskType;
                ResultCaller = GenTaskType.GetProperty("Result").DelegateForGetProperty();
            }
            public Type GenTaskType { get; }

            public MemberGetter ResultCaller { get; }
        }

        string ResultAsString => (Instance.Status == TaskStatus.RanToCompletion) ? Result.ToString() : "<value not available>";

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public object Result => cache.ResultCaller(Instance);
    }
}
