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

    }
}
