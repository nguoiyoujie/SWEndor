using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Primrose.Primitives;
using System.Collections.Generic;

namespace Primrose.UnitTests
{
  [TestClass]
  public class Cache_Tests
  {
    [TestMethod]
    public void Cache_Test()
    {
      Cache<string, int, int, string> testcache = new Cache<string, int, int, string>();

      testcache.Define("str1", 0);
      Assert.AreEqual(4, testcache.GetOrDefine("str1", 1, (s) => { return s.Length; }, "str1", EqualityComparer<int>.Default));
      Assert.AreEqual(4, testcache.GetOrDefine("str1", 1, (s) => { return s.Length; }, "str12", EqualityComparer<int>.Default));
      Assert.AreEqual(5, testcache.GetOrDefine("str1", 2, (s) => { return s.Length; }, "str12", EqualityComparer<int>.Default));
      Assert.AreEqual(6, testcache.GetOrDefine("str1", 3, (s) => { return s.Length; }, "str123", EqualityComparer<int>.Default));
    }
  }
}
