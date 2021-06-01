﻿using Primrose.Expressions;
using System;

namespace SWEndor.Game.Scenarios.Scripting.Functions
{
  public static class MiscFns
  {
    /// <summary>
    /// Gets the element of an array. This is functionally equivalent to the indexer 'array[i]', except that this function cannot be used on float2/float3/float4
    /// </summary>
    /// <param name="context">The game context</param>
    public static Val IsNull(IContext context, Val value)
    {
      return new Val(value.IsNull);
    }

    /// <summary>
    /// Generates a random float value between 0 and 1.
    /// </summary>
    /// <param name="context">The game context</param>
    public static Val Random(IContext context)
    {
      return new Val((float)((Context)context).Engine.Random.NextDouble());
    }

    /// <summary>
    /// Generates a random integer between 0 and an integer.
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="max">The exclusive upper bound</param>
    public static Val Random(IContext context, int max)
    {
      return new Val((float)((Context)context).Engine.Random.Next(0, max));
    }

    /// <summary>
    /// Generates a random integer between min and max.
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="min">The inclusive lower bound</param>
    /// <param name="max">The exclusive upper bound</param>
    public static Val Random(IContext context, int min, int max)
    {
      return new Val((float)((Context)context).Engine.Random.Next(min, max));
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
    public static Val GetArrayElement(IContext context, Val array, int index)
    {
      Array a;
      try
      {
        a = array.Cast<Array>();
      }
      catch
      {
        throw new Exception("Attempted to apply GetArrayElement on a non-array object.");
      }
      return new Val(a.GetValue(index));
    }
  }
}
