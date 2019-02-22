using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using BaseLibs.Types;
using System.Threading.Tasks;

namespace BaseLibs.Test.Types
{
    public class MethodInfoDelegates
    {

        public class ClassWithGenMethod
        {
            public T GetVal<T>(string arg)
            {
                if (typeof(T) == typeof(string))
                    return (T)(object)"bob";
                if (typeof(T) == typeof(int))
                    return (T)(object)5;
                return default(T);
            }

            public Task<T> GetValAsync<T>(string arg)
            {
                if (typeof(T) == typeof(string))
                    return Task.FromResult((T)(object)"bob");
                if (typeof(T) == typeof(int))
                    return Task.FromResult((T)(object)5);
                return Task.FromResult(default(T));
            }
        }

        [Fact]
        public void DelForMethod_GenArgs()
        {
            var m = typeof(ClassWithGenMethod).GetMethod(nameof(ClassWithGenMethod.GetVal));
            var mDel = m.DelegateForMethod(typeof(string));
            var inst = new ClassWithGenMethod();
            var obj = mDel(inst, "bob");
            Assert.IsType<string>(obj);
            Assert.Equal("bob", obj);
        }

        [Fact]
        public void CreateCustomDelegate_GenArgs_string()
        {
            var m = typeof(ClassWithGenMethod).GetMethod(nameof(ClassWithGenMethod.GetVal));
            var mDel = m.CreateCustomDelegate<Func<ClassWithGenMethod, string, string>>(typeof(string));
            var inst = new ClassWithGenMethod();
            var obj = mDel(inst, "bob");
            Assert.Equal("bob", obj);
        }

        [Fact]
        public void CreateCustomDelegate_GenArgs_int()
        {
            var m = typeof(ClassWithGenMethod).GetMethod(nameof(ClassWithGenMethod.GetVal));
            var mDel = m.CreateCustomDelegate<Func<ClassWithGenMethod, string, int>>(typeof(int));
            var inst = new ClassWithGenMethod();
            var obj = mDel(inst, "bob");
            Assert.Equal(5, obj);
        }

        [Fact]
        public void CreateCustomDelegate_GenArgsAsync_string()
        {
            var m = typeof(ClassWithGenMethod).GetMethod(nameof(ClassWithGenMethod.GetValAsync));
            var mDel = m.CreateCustomDelegate<Func<ClassWithGenMethod, string, Task<string>>>(typeof(string));
            var inst = new ClassWithGenMethod();
            var obj = mDel(inst, "bob");
            Assert.Equal("bob", obj.Result);
        }

        [Fact]
        public void CreateCustomDelegate_GenArgsAsync_stringObj()
        {
            var m = typeof(ClassWithGenMethod).GetMethod(nameof(ClassWithGenMethod.GetValAsync));
            var mDel = m.CreateCustomDelegate<Func<ClassWithGenMethod, string, Task>>(typeof(string));
            var inst = new ClassWithGenMethod();
            var obj = mDel(inst, "bob");
            Assert.IsType<Task<string>>(obj);
            Assert.Equal("bob", ((Task<string>)obj).Result);
        }

    }
}
