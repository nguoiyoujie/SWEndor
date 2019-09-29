namespace SWEndor.Primitives.Extensions
{
  public static class ObjectExts
  {
    /// <summary>
    /// Checks if a value evaluates to True or False
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool ToBool(this object value)
    {
      return !(value == null
              || value.ToString().Length == 0
              || "0".Equals(value.ToString().ToLower())
              || "false".Equals(value.ToString().ToLower())
              || "no".Equals(value.ToString().ToLower())
              );
    }
  }
}
