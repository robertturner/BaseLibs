using System;
using System.Collections.Generic;
using System.Text;
using BaseLibs.Tuples;
using Xunit;

namespace BaseLibs.Test.Tuples
{
    public class ValueTupleValuesGetter
    {
        [Fact]
        public void ValuesGetter_ClassTypes()
        {
            var vt = ValueTuple.Create("bob", "sfsf");
            var vg = vt.ValuesGetter();
            var vals = vg(vt);
            Assert.Equal(new object[] { "bob", "sfsf" }, vals);
        }
        [Fact]
        public void ValuesGetter_StructTypes()
        {
            var vt = ValueTuple.Create(5, 2);
            var vg = vt.ValuesGetter();
            var vals = vg(vt);
            Assert.Equal(new object[] { 5, 2 }, vals);
        }

    }
}
