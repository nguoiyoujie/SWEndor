using System;
using System.Collections.Generic;

namespace Primrose.Primitives.Extensions
{
  /// <summary>
  /// Provides extension methods for Type values
  /// </summary>
  public static class TypeExts
  {
    /// <summary>
    /// Enumerates of the base class types of a given type
    /// </summary>
    /// <param name="t">The type to be enumerated</param>
    /// <returns>A enumeration of types that is inherited by <paramref name="t"/></returns>
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
