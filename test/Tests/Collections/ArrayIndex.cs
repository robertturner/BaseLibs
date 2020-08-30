using BaseLibs.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BaseLibs.Test.Collections
{
    public class ArrayIndex
    {
        [Fact]
        public void ArrayIndex_ValueArray()
        {
            var array = new int[] { 1, 2, 3, 4 };

            var indexer = array.GetType().GetMemberGetterDelegate();

            Assert.Equal(array[2], indexer(array, 2));
        }

        [Fact]
        public void ArrayIndex_ObjectArray()
        {
            var array = new object[4];
            array[2] = new object();

            var indexer = array.GetType().GetMemberGetterDelegate();

            Assert.Equal(array[2], indexer(array, 2));
        }

        class AnObj { }

        [Fact]
        public void ArrayIndex_AnObjArray()
        {
            var array = new AnObj[4];
            array[2] = new AnObj();

            var indexer = array.GetType().GetMemberGetterDelegate();

            Assert.Equal(array[2], indexer(array, 2));
        }


    }
}
