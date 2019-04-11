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

        [Fact]
        public void GetProperty_ClassInstance_ClassProperty()
        {
            var pi = typeof(AClass).GetProperty(nameof(AClass.AClassProperty));
            var getter = pi.DelegateForGetProperty();

            var inst = new AClass() { AClassProperty = "howdy" };

            var inst2 = getter(inst);
            Assert.Equal(inst.AClassProperty, inst2);
        }

        [Fact]
        public void GetProperty_ClassInstance_ValueProperty()
        {
            var pi = typeof(AClass).GetProperty(nameof(AClass.AValProperty));
            var getter = pi.DelegateForGetProperty();

            var inst = new AClass() { AValProperty = 66 };

            var inst2 = getter(inst);
            Assert.Equal(inst.AValProperty, inst2);
        }

        [Fact]
        public void GetProperty_ClassInstance_StructProperty()
        {
            var pi = typeof(AClass).GetProperty(nameof(AClass.AStructVal));
            var getter = pi.DelegateForGetProperty();

            var inst = new AClass() { AStructVal = new StructVal { IntVal = 5 } };

            var inst2 = getter(inst);
            Assert.Equal(inst.AStructVal, inst2);
        }

        [Fact]
        public void GetPropertyT_ClassInstance_ClassProperty()
        {
            var pi = typeof(AClass).GetProperty(nameof(AClass.AClassProperty));
            var getter = pi.DelegateForGetProperty<object>();

            var inst = new AClass() { AClassProperty = "howdy" };

            var inst2 = getter(inst);
            Assert.Equal(inst.AClassProperty, inst2);
        }

        [Fact]
        public void GetPropertyT_ClassInstance_ValueProperty()
        {
            var pi = typeof(AClass).GetProperty(nameof(AClass.AValProperty));
            var getter = pi.DelegateForGetProperty();

            var inst = new AClass() { AValProperty = 66 };

            var inst2 = getter(inst);
            Assert.Equal(inst.AValProperty, inst2);
        }
#if false
        [Fact]
        public void GetPropertyT_ClassInstance_ValueProperty_Convert()
        {
            var pi = typeof(AClass).GetProperty(nameof(AClass.AValProperty));
            var getter = pi.DelegateForGetProperty<Int16>();

            var inst = new AClass() { AValProperty = 66 };

            var inst2 = getter(inst);
            Assert.Equal(inst.AValProperty, (int)inst2);
        }
#endif

        [Fact]
        public void GetPropertyT_ClassInstance_StructProperty()
        {
            var pi = typeof(AClass).GetProperty(nameof(AClass.AStructVal));
            var getter = pi.DelegateForGetProperty<StructVal>();

            var inst = new AClass() { AStructVal = new StructVal { IntVal = 5 } };

            var inst2 = getter(inst);
            Assert.Equal(inst.AStructVal, inst2);
        }

    }
}
