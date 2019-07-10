using BaseLibs.Collections;
using BaseLibs.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Tasks
{
    [System.Diagnostics.DebuggerDisplay("Status = {Instance.Status}, Result = {ResultAsString}")]
    public abstract class TaskGeneric
    {
        static readonly ConcurrentDictionary<Type, ConstructorInvoker> ctorCache = new ConcurrentDictionary<Type, ConstructorInvoker>();

        public static TaskGeneric Create(Task task)
        {
            var type = TaskType(task);
            var ctor = ctorCache.GetOrAdd(type, t => typeof(TaskGen<>).MakeGenericType(t).GetConstructor(new Type[] { typeof(Task) }).DelegateForConstructor());
            return (TaskGeneric)ctor(task);
        }

        static Type TaskType(Task task)
        {
            var genTask = task.GetType();
            for (; ; )
            {
                if (genTask == typeof(Task))
                    ExThrowers.ThrowArgEx("task does not have Result");
                if (genTask.IsGenericType)
                {
                    var t = genTask.GetGenericTypeDefinition();
                    if (t == typeof(Task<>))
                        break;
                }
                genTask = genTask.BaseType;
            }
            return genTask.GetGenericArguments()[0];
        }

        private TaskGeneric() { }

        public abstract Task Instance { get; }

        public abstract Type ResultType { get; }

        sealed class TaskGen<T> : TaskGeneric
        {
            public override Task Instance => instance;
            public Task<T> instance;

            public override Type ResultType => typeof(T);

            public override object Result => instance.Result;

            public TaskGen(Task instance) => this.instance = (Task<T>)instance;
        }

        string ResultAsString => (Instance.Status == TaskStatus.RanToCompletion) ? Result.ToString() : "<value not computed yet>";

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public abstract object Result { get; }
    }
}
