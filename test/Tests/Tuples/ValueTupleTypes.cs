using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibs.Tuples;
using Xunit;

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

        void DummyMethod((int Bob, string Jim) vtArg) { }

        [Fact]
        public void ValuesTupleTypes_ParamNames()
        {
            var mi = typeof(ValueTupleTypes).GetMethod(nameof(DummyMethod), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var pi = mi.GetParameters()[0];
            Assert.True(pi.TryGetValueTupleTypes(out Type[] argTypes, out string[] argNames));

            Assert.Equal(new[] { typeof(int), typeof(string) }, argTypes);
            Assert.Equal(new[] { "Bob", "Jim" }, argNames);
        }

    }
}
