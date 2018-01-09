using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using BaseLibs.Tasks;
using System.Threading.Tasks;

namespace BaseLibs.Test.Tasks
{
    public class TCSGenerics
    {
        [Fact]
        public void SetResult_String()
        {
            var res = "bob";
            var tcs = typeof(string).AsTCSGeneric();
            tcs.SetResult(res);

            Assert.NotNull(tcs.Task);
            var task = tcs.Task as Task<string>;
            Assert.NotNull(task);
            Assert.Equal(res, task.Result);
        }
    }
}
