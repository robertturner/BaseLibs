using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using BaseLibs.Collections;

namespace BaseLibs.Test.Collections
{
    public class Dictionaries
    {
        [Fact]
        public void ConcurrentDict_GetOrSet()
        {
            var dict = new System.Collections.Concurrent.ConcurrentDictionary<int, string>();
            dict.GetOrSet(3, () => "bob");
            Assert.Equal("bob", dict.GetValueOrDefault(3));
        }


        [Fact]
        public void Dict_GetValueOrDefault_KeyExists()
        {
            var dict = new Dictionary<int, string> { { 3, "bob" } };
            Assert.Equal("bob", dict.GetValueOrDefault(3));
        }
        [Fact]
        public void Dict_GetValueOrDefault_Default()
        {
            var dict = new Dictionary<int, string> { { 3, "bob" } };
            Assert.Null(dict.GetValueOrDefault(5));
        }

        [Fact]
        public void IDict_GetValueOrDefault_KeyExists()
        {
            IDictionary<int, string> dict = new Dictionary<int, string> { { 3, "bob" } };
            Assert.Equal("bob", dict.GetValueOrDefault(3));
        }
        [Fact]
        public void IDict_GetValueOrDefault_Default()
        {
            IDictionary<int, string> dict = new Dictionary<int, string> { { 3, "bob" } };
            Assert.Null(dict.GetValueOrDefault(5));
        }

        [Fact]
        public void IReadOnlyDict_GetValueOrDefault_KeyExists()
        {
            IReadOnlyDictionary<int, string> dict = new Dictionary<int, string> { { 3, "bob" } };
            Assert.Equal("bob", dict.GetValueOrDefault(3));
        }
        [Fact]
        public void IReadOnlyDict_GetValueOrDefault_Default()
        {
            IReadOnlyDictionary<int, string> dict = new Dictionary<int, string> { { 3, "bob" } };
            Assert.Null(dict.GetValueOrDefault(5));
        }

    }
}
