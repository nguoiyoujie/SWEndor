using Microsoft.VisualStudio.TestTools.UnitTesting;
using Primrose.Primitives;
using System;
using System.Collections.Generic;

namespace Primrose.UnitTests
{
  [TestClass]
  public class Cache_Tests
  {
    [TestMethod]
    public void Cache()
    {
      Cache<string, int, int, string> test_cache = new Cache<string, int, int, string>();

      // define
      test_cache.Define("test", 0);
      // new token = value is updated
      Assert.AreEqual(4, test_cache.GetOrDefine("test", 1, (s) => { return s.Length; }, "str1", EqualityComparer<int>.Default));
      // same token = value is not updated
      Assert.AreEqual(4, test_cache.GetOrDefine("test", 1, (s) => { return s.Length; }, "str12", EqualityComparer<int>.Default));
      // different token = value is updated
      Assert.AreEqual(5, test_cache.GetOrDefine("test", 2, (s) => { return s.Length; }, "str12", EqualityComparer<int>.Default));
      Assert.AreEqual(6, test_cache.GetOrDefine("test", 3, (s) => { return s.Length; }, "str123", EqualityComparer<int>.Default));

      // clear cache
      test_cache.Clear();
      // new token = value is updated
      Assert.AreEqual(4, test_cache.GetOrDefine("test", 3, (s) => { return s.Length; }, "str1", EqualityComparer<int>.Default));
      // same token = value is not updated
      Assert.AreEqual(4, test_cache.GetOrDefine("test", 3, (s) => { return int.Parse(s); }, "123", EqualityComparer<int>.Default));
      // different token = value is updated
      Assert.AreEqual(123, test_cache.GetOrDefine("test", 4, (s) => { return int.Parse(s); }, "123", EqualityComparer<int>.Default));
    }

    private enum TestEnum { TEST, NOTEST, ABC, RANDOM };

    [TestMethod]
    public void EnumNames()
    {
      foreach (TestEnum t in Enum.GetValues(typeof(TestEnum)))
        Assert.AreEqual(t.ToString(), t.GetEnumName());
    }
  }
}
