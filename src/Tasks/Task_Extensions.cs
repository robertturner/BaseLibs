using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Tasks
{
    public static class Task_Extensions
    {
        public static TaskGeneric TryGetAsGenericTask(this Task task)
        {
            return new TaskGeneric(task);
        }
    }
}
