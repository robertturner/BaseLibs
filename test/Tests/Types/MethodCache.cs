using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using BaseLibs;
using BaseLibs.Types;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace BaseLibs.Test.Types
{
    public class MethodCache
    {
        interface IBase
        {
            string Arg1 { get; }
            int Arg2 { get; }
            IBase Arg3 { get; }
        }
        interface IBase<T> : IBase
        {
            new IBase<T> Arg3 { get; }
            IBase IBase.Arg3 => Arg3;
        }

        class Creator<T>
        {

        }


        abstract class BaseClass
        {
            public static IBase CreateSpecialised(Type type, string arg1, int arg2, IBase arg3)
            {
                var caller = SpecialisedCallerCache<IBase, Func<string, int, IBase, IBase>>.CallForType(type, typeof(BaseClass<>), nameof(BaseClass<int>.Creator));
                return caller(arg1, arg2, arg3);
            }

            public abstract string Arg1 { get; }
            public abstract int Arg2 { get; }
            public IBase Arg3 { get; }
        }

        class BaseClass<T> : IBase<T>
        {
            public BaseClass(string arg1, int arg2, IBase<T> arg3) 
            {
                Arg1 = arg1; Arg2 = arg2; Arg3 = arg3;
            }
            public string Arg1 { get; }
            public int Arg2 { get; } 
            public IBase<T> Arg3 { get; }

            internal static IBase Creator(string a1, int a2, IBase a3) => new BaseClass<T>(a1, a2, (IBase<T>)a3);
        }


        [Fact]
        public void Test_CreateSpecialised()
        {
            var ret = BaseClass.CreateSpecialised(typeof(string), "arg1", 2, null);
            Assert.IsType<BaseClass<string>>(ret);
            Assert.Equal("arg1", ret.Arg1);
            Assert.Equal(2, ret.Arg2);
            Assert.Null(ret.Arg3);

        }

    }
}
