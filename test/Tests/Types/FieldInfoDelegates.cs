using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using BaseLibs.Types;

namespace BaseLibs.Test.Types
{
    public class FieldInfoDelegates
    {
        enum AnEnum : byte
        {
            Bob = 0,
            Jim = 10
        }

        class AClassWithFields
        {
            public string Bob;
            public AnEnum EnumVal;
        }

        public enum FieldCode : byte
        {
            Invalid,
            Path,
            Interface,
            Member,
            ErrorName,
            ReplySerial,
            Destination,
            Sender,
            Signature,
            UnixFds
        }


        struct FieldCodeEntry
        {
            public FieldCode Code;
            public object Value;
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
        public void SetField_StructInstance_Enum()
        {
            var fi = typeof(FieldCodeEntry).GetField(nameof(FieldCodeEntry.Code));
            var setter = fi.DelegateForSetField();

            object inst = new FieldCodeEntry();
            var fieldVal = (byte)6;
            inst = setter(inst, fieldVal);
            Assert.Equal(FieldCode.Destination, ((FieldCodeEntry)inst).Code);
        }

        [Fact]
        public void GetField_StructInstance_Enum()
        {
            var fi = typeof(FieldCodeEntry).GetField(nameof(FieldCodeEntry.Code));
            var getter = fi.DelegateForGetField();

            object inst = new FieldCodeEntry() { Code = FieldCode.ReplySerial };
            var fieldVal = getter(inst);
            Assert.Equal(FieldCode.ReplySerial, fieldVal);
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
