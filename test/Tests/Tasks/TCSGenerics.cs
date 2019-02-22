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



        [Fact]
        public void CastAs_String()
        {
            var tcs = new TaskCompletionSource<object>();

            var strTask = tcs.Task.CastResultAs(typeof(string));

            Assert.NotNull(strTask);
            Assert.IsType<Task<string>>(strTask);
            tcs.SetResult("bob");
            Assert.Equal("bob", ((Task<string>)strTask).Result);
        }

        [Fact]
        public void CastAs_BadString()
        {
            var tcs = new TaskCompletionSource<object>();

            var strTask = tcs.Task.CastResultAs(typeof(string));

            Assert.NotNull(strTask);
            Assert.IsType<Task<string>>(strTask);
            tcs.SetResult(5);
            Assert.True(strTask.IsFaulted);
            Exception ex = strTask.Exception;
            if (ex is AggregateException aex)
            {
                Assert.Single(aex.InnerExceptions);
                ex = aex.InnerExceptions[0];
            }
            Assert.IsType<InvalidCastException>(ex);
        }


        [Fact]
        public void CastAs_Cancelled()
        {
            var tcs = new TaskCompletionSource<object>();

            var strTask = tcs.Task.CastResultAs(typeof(string));

            Assert.NotNull(strTask);
            Assert.IsType<Task<string>>(strTask);
            tcs.SetCanceled();
            Assert.True(strTask.IsCanceled);
        }

        [Fact]
        public void CastAs_Exception()
        {
            var tcs = new TaskCompletionSource<object>();
            var strTask = tcs.Task.CastResultAs(typeof(string));
            Assert.NotNull(strTask);
            Assert.IsType<Task<string>>(strTask);
            tcs.SetException(new Exception());
            Assert.True(strTask.IsFaulted);
        }

        [Fact]
        public void CastAs_ValueType()
        {
            var tcs = new TaskCompletionSource<object>();
            var intTask = tcs.Task.CastResultAs(typeof(int));
            Assert.NotNull(intTask);
            Assert.IsType<Task<int>>(intTask);
            tcs.SetResult(5);
            Assert.True(intTask.IsCompleted);
            Assert.Equal(5, ((Task<int>)intTask).Result);
        }

    }
}
