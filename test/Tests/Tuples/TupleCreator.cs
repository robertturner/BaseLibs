using System;
using System.Collections.Generic;
using System.Text;
using BaseLibs.Tuples;
using Xunit;

namespace BaseLibs.Test.Tuples
{
    public class TupleCreator
    {

        [Fact]
        public void TupleCreator_ClassTypes()
        {
            var creator = (new[] { typeof(string), typeof(string) }).AsValueTupleCreator();
            var testVals = new object[] { "bob", "jim" };
            var tuple = creator(testVals);

            Assert.IsType<(string, string)>(tuple);
            var vals = typeof((string, string)).ValueTupleValuesGetter()(tuple);
            Assert.Equal(testVals, vals);
        }

        [Fact]
        public void TupleCreator_StructTypes()
        {
            var creator = (new[] { typeof(int), typeof(int) }).AsValueTupleCreator();
            var testVals = new object[] { 5, 4 };
            var tuple = creator(testVals);

            Assert.IsType<(int, int)>(tuple);
            var vals = typeof((int, int)).ValueTupleValuesGetter()(tuple);
            Assert.Equal(testVals, vals);
        }
    }
}
