using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using BaseLibs.Types;


namespace BaseLibs.Test.Types
{
    public class ConstructorInfoDelegates
    {
        [Fact]
        public void Ctor_ObjectNoArgs1()
        {
            var c = typeof(object).GetConstructor(new Type[0]);
            var constructorDel = c.DelegateForConstructor();
            var obj = constructorDel.Invoke();
            Assert.IsType<object>(obj);
        }

        [Fact]
        public void Ctor_ObjectNoArgs2()
        {
            var c = typeof(object).GetConstructor(new Type[0]);
            var constructorDel = c.DelegateForConstructorNoArgs();
            var obj = constructorDel.Invoke();
            Assert.IsType<object>(obj);
        }

        [Fact]
        public void Ctor_ObjectNoArgs3()
        {
            var constructorDel = typeof(object).GetParameterlessConstructor();
            var obj = constructorDel.Invoke();
            Assert.IsType<object>(obj);
        }

        [Fact]
        public void Ctor_ValueTypeNoArgs()
        {
            var constructorDel = typeof(int).GetParameterlessConstructor();
            var obj = constructorDel.Invoke();
            Assert.IsType<int>(obj);
        }

        [Fact]
        public void Ctor_StringOneArg()
        {
            var c = typeof(string).GetConstructor(new[] { typeof(char[]) });
            var constructorDel = c.DelegateForConstructor();
            var obj = constructorDel.Invoke("bob".ToCharArray());
            Assert.IsType<string>(obj);
            Assert.Equal("bob", (string)obj);
        }

        [Fact]
        public void Ctor_DictionaryNoArgs()
        {
            var c = typeof(Dictionary<string, string>).GetConstructor(new Type[0]);
            var constructorDel = c.DelegateForConstructorNoArgs();
            var obj = constructorDel();
            Assert.IsType<Dictionary<string, string>>(obj);
        }

    }
}
