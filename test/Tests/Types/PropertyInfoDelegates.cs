using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using BaseLibs.Types;


namespace BaseLibs.Test.Types
{
    public class PropertyInfoDelegates
    {
        struct StructVal
        {
            public object ObjVal;
            public int IntVal;
        }

        class AClass
        {

            public string AClassProperty { get; set; }
            public int AValProperty { get; set; }
            public StructVal AStructVal { get; set; }
        }

        struct AStruct
        {
            public string AClassProperty { get; set; }
            public int AValProperty { get; set; }
            public StructVal AStructVal { get; set; }
        }


        [Fact]
        public void SetProperty_ClassInstance_ClassProperty()
        {
            var pi = typeof(AClass).GetProperty(nameof(AClass.AClassProperty));
            var setter = pi.DelegateForSetProperty();

            

            var inst = new AClass();

            var inst2 = setter(inst, "howdy");
            Assert.Equal("howdy", inst.AClassProperty);
        }

        [Fact]
        public void SetProperty_ClassInstance_ValueProperty()
        {
            var pi = typeof(AClass).GetProperty(nameof(AClass.AValProperty));
            var setter = pi.DelegateForSetProperty();

            var inst = new AClass();

            var inst2 = setter(inst, 77);
            Assert.Equal(77, inst.AValProperty);
        }
        [Fact]
        public void SetProperty_ClassInstance_StructProperty()
        {
            var pi = typeof(AClass).GetProperty(nameof(AClass.AStructVal));
            var setter = pi.DelegateForSetProperty();

            var inst = new AClass();

            var setVal = new StructVal { IntVal = 55, ObjVal = "yo" };

            var inst2 = setter(inst, setVal);
            Assert.Equal(setVal, inst.AStructVal);
        }

        [Fact]
        public void SetProperty_StructInstance_ClassProperty()
        {
            var pi = typeof(AStruct).GetProperty(nameof(AStruct.AClassProperty));
            var setter = pi.DelegateForSetProperty();

            var inst = new AStruct();

            inst = (AStruct)setter(inst, "howdy");
            Assert.Equal("howdy", inst.AClassProperty);
        }

        [Fact]
        public void SetProperty_StructInstance_ValueProperty()
        {
            var pi = typeof(AStruct).GetProperty(nameof(AStruct.AValProperty));
            var setter = pi.DelegateForSetProperty();

            var inst = new AStruct();

            inst = (AStruct)setter(inst, 77);
            Assert.Equal(77, inst.AValProperty);
        }

        [Fact]
        public void SetProperty_StructInstance_StructProperty()
        {
            var pi = typeof(AStruct).GetProperty(nameof(AStruct.AStructVal));
            var setter = pi.DelegateForSetProperty();

            var inst = new AStruct();

            var setVal = new StructVal { IntVal = 55, ObjVal = "yo" };

            inst = (AStruct)setter(inst, setVal);
            Assert.Equal(setVal, inst.AStructVal);
        }
    }
}
