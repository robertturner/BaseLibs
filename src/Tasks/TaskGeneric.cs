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

        public Task Instance { get; private set; }

        TaskGenDelCache cache;

        public Type GenTaskType { get { return cache.GenTaskType; } }
        public Type ResultType { get; private set; }

        class TaskGenDelCache
        {
            public TaskGenDelCache(Type genTaskType)
            {
                GenTaskType = genTaskType;
                ResultCaller = GenTaskType.GetProperty("Result").DelegateForGetProperty();
            }
            public Type GenTaskType { get; private set; }

            public MemberGetter ResultCaller { get; private set; }
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
