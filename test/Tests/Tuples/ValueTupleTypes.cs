using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibs.Tuples;
using Xunit;
using static BaseLibs.Tuples.ParameterInfo_Extensions;

namespace BaseLibs.Test.Tuples
{
    public class ValueTupleTypes
    {
        [Fact]
        public void ValuesTupleTypes_3Strings()
        {
            var tArgs = new Type[] { typeof(string), typeof(string), typeof(string) };
            var vtT = tArgs.AsValueTupleType();
            Assert.Equal(typeof((string, string, string)), vtT);
        }

        void DummyTupleMethod(
            (int Bob, 
                (int Suba1, 
                    (int SubaSub1, int SubaSub2) SubaSub) Suba, 
            string Jim, 
                (int Sub1, 
                    (int SubSub1, string SubSub2) SubSub, 
                string Sub2) SubTuple) vtArg) { }

        [Fact]
        public void ValuesTupleTypes_ParamNames()
        {
            var mi = typeof(ValueTupleTypes).GetMethod(nameof(DummyTupleMethod), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var pi = mi.GetParameters()[0];
            Assert.True(pi.TryGetValueTupleTypes(out TupleArg[] args));

            //Assert.Equal(new[] { typeof(int), typeof(string) }, argTypes);
            //Assert.Equal(new[] { "Bob", "Jim" }, argNames);
        }

        void DummyNoTupleMethod(int Bob) { }
        [Fact]
        public void ValuesTupleTypes_NoTuples()
        {
            var mi = typeof(ValueTupleTypes).GetMethod(nameof(DummyNoTupleMethod), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var pi = mi.GetParameters()[0];
            Assert.False(pi.TryGetValueTupleTypes(out TupleArg[] args));

            //Assert.Equal(new[] { typeof(int), typeof(string) }, argTypes);
            //Assert.Equal(new[] { "Bob", "Jim" }, argNames);
        }
    }
}
