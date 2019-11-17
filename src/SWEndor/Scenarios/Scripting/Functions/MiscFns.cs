using Primrose.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class MiscFns
  {
    /// <summary>
    /// Gets the element of an array. This is functionally equivalent to the indexer 'array[i]', except that this function cannot be used on float2/float3/float4
    /// </summary>
    /// <param name="context">The game context</param>
    public static Val IsNull(AContext context, Val value)
    {
      return new Val(value.IsNull);
    }

    /// <summary>
    /// Gets the element of an array. This is functionally equivalent to the indexer 'array[i]', except that this function cannot be used on float2/float3/float4
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     BOOL[]/INT[]/FLOAT[] array
    /// </param>
    /// <returns>BOOL/INT/FLOAT, depending on the input</returns>
    public static Val GetArrayElement(Context context, Val[] ps)
    {
      if (ps[0].Type == ValType.BOOL_ARRAY)
        return new Val(((bool[])ps[0])[(int)ps[1]]);

      if (ps[0].Type == ValType.INT_ARRAY)
        return new Val(((int[])ps[0])[(int)ps[1]]);

      if (ps[0].Type == ValType.FLOAT_ARRAY)
        return new Val(((float[])ps[0])[(int)ps[1]]);

      return Val.NULL;
    }

    public static Val GetArrayElement(Context context, bool[] array, int index) { return new Val(array[index]); }
    public static Val GetArrayElement(Context context, int[] array, int index) { return new Val(array[index]); }
    public static Val GetArrayElement(Context context, float[] array, int index) { return new Val(array[index]); }
    public static Val GetArrayElement(Context context, string[] array, int index) { return new Val(array[index]); }
  }
}
