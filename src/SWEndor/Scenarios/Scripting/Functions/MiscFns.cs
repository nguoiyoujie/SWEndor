﻿using Primrose.Expressions;
using System;

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
    public static Val GetArrayElement(Context context, Val array, int index)
    {
      if (array.Type == ValType.BOOL_ARRAY)
        return new Val(((bool[])array)[index]);
      else if (array.Type == ValType.INT_ARRAY)
        return new Val(((int[])array)[index]);
      else if (array.Type == ValType.FLOAT_ARRAY)
        return new Val(((float[])array)[index]);
      else if (array.Type == ValType.STRING_ARRAY)
        return new Val(((string[])array)[index]);

      else
        throw new Exception("Attempted to apply GetArrayElement on an non-array object.");
    }
  }
}
