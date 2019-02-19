using BaseLibs.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xunit;

namespace BaseLibs.Test.Enums
{
    public class GetDescription
    {

        enum Enum_NoVals1
        {
            [Description("Bob is a guy")]
            Bob,
            Jim
        }
#if false
        enum Enum_SomeVals
        {
            Jim,
            Tim = 5,
            Tom
        }
        enum Enum_NoVals_Typed1 : int
        {
            Bob,
            Jim
        }
        enum Enum_SomeVals_Typed : int
        {
            Jim,
            Tim = 5,
            Tom
        }
#endif
        [Fact]
        public void NoVals_NoType()
        {
            var bobDesc = Enum_NoVals1.Bob.GetDescription();
            Assert.Equal("Bob is a guy", bobDesc);
            Assert.Equal("Jim", Enum_NoVals1.Jim.GetDescription());
        }

        [Fact]
        public void Enum_EnumFromDesc()
        {
            var bobDesc = Enum_Extensions.TryParseFromDescription<Enum_NoVals1>("Bob is a guy");
            Assert.Equal(Enum_NoVals1.Bob, bobDesc);
            Assert.Equal(Enum_NoVals1.Jim, Enum_Extensions.TryParseFromDescription<Enum_NoVals1>("Jim"));
        }
    }
}
