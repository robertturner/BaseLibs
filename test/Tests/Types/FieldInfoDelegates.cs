using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using BaseLibs.Types;

namespace BaseLibs.Test.Types
{
    public class FieldInfoDelegates
    {
        class AClassWithFields
        {
            public string Bob;
        }

        [Fact]
        public void SetField_Instance()
        {
            var fi = typeof(AClassWithFields).GetField(nameof(AClassWithFields.Bob));
            var setter = fi.DelegateForSetField();

            var inst = new AClassWithFields();

            setter(inst, "howdy");
            Assert.Equal("howdy", inst.Bob);
        }

        [Fact]
        public void GetField_Instance()
        {
            var fi = typeof(AClassWithFields).GetField(nameof(AClassWithFields.Bob));
            var getter = fi.DelegateForGetField();

            var inst = new AClassWithFields() { Bob = "howdy" };

            Assert.Equal("howdy", getter(inst));
        }

    }
}
