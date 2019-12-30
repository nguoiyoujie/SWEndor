using Primrose.Primitives.Extensions;

namespace Primrose.Expressions.UnitTests.Scripting
{
  public static class MathFns
  {
    public static Val Int(Context context, float val)
    {
      return new Val((int)val);
    }

    public static Val Max(Context context, float val1, float val2)
    {
      return new Val(val1.Max(val2));
    }

    public static Val Min(Context context, float val1, float val2)
    {
      return new Val(val1.Min(val2));
    }
  }
}
