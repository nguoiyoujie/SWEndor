using System;
using System.Collections.Generic;

namespace SWEndor.Primitives.Extensions
{
  public static class TypeExts
  {
    public static IEnumerable<Type> BaseTypes(this Type t)
    {
      while (t != null)
      {
        yield return t;
        t = t.BaseType;
      }
    }
  }
}
