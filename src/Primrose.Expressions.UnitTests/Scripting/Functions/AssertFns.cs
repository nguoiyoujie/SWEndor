using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Primrose.Expressions.UnitTests.Scripting
{
  public static class AssertFns
  {
    public static Val AreEqual(Context context, Val v1, Val v2)
    {
      if (v1.Type == ValType.FLOAT || v2.Type == ValType.FLOAT)
        Assert.AreEqual((float)v1, (float)v2, 0.00001f);
      else if (v1.Type == ValType.INT || v2.Type == ValType.INT)
        Assert.AreEqual((int)v1, (int)v2);
      else
        Assert.AreEqual(v1.Value, v2.Value);
      return Val.NULL;
    }

    public static Val AreNotEqual(Context context, Val v1, Val v2)
    {
      if (v1.Type == ValType.FLOAT || v2.Type == ValType.FLOAT)
        Assert.AreNotEqual((float)v1, (float)v2, 0.00001f);
      else if (v1.Type == ValType.INT || v2.Type == ValType.INT)
        Assert.AreNotEqual((int)v1, (int)v2);
      else
        Assert.AreNotEqual(v1.Value, v2.Value); return Val.NULL;
    }
  }
}
