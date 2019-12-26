using Primrose.Primitives.Extensions;
using Primrose.Primitives.ValueTypes;
using System;
using System.Collections.Generic;

namespace Primrose.Expressions
{
  /// <summary>
  /// The operation class
  /// </summary>
  public static class Ops
  {
    internal static List<Pair<ValType, ValType>> CoerceTypes
      = new List<Pair<ValType, ValType>>
      {
        new Pair<ValType, ValType>(ValType.FLOAT2, ValType.FLOAT_ARRAY),
        new Pair<ValType, ValType>(ValType.FLOAT3, ValType.FLOAT_ARRAY),
        new Pair<ValType, ValType>(ValType.FLOAT4, ValType.FLOAT_ARRAY),
        new Pair<ValType, ValType>(ValType.FLOAT2, ValType.INT_ARRAY),
        new Pair<ValType, ValType>(ValType.FLOAT3, ValType.INT_ARRAY),
        new Pair<ValType, ValType>(ValType.FLOAT4, ValType.INT_ARRAY),
        new Pair<ValType, ValType>(ValType.FLOAT_ARRAY, ValType.INT_ARRAY),
      };

    internal static Dictionary<Pair<UOp, ValType>, Func<Val, Val>> unaryops
      = new Dictionary<Pair<UOp, ValType>, Func<Val, Val>>
      {
        // IDENTITY
        { new Pair<UOp, ValType>(UOp.IDENTITY, ValType.BOOL), (a) => { return a; } },
        { new Pair<UOp, ValType>(UOp.IDENTITY, ValType.INT), (a) => { return a; } },
        { new Pair<UOp, ValType>(UOp.IDENTITY, ValType.FLOAT), (a) => { return a; } },
        { new Pair<UOp, ValType>(UOp.IDENTITY, ValType.STRING), (a) => { return a; } },

        // NEGATION
        { new Pair<UOp, ValType>(UOp.LOGICAL_NOT, ValType.BOOL), (a) => { return new Val(!(bool)a); }},

        // INVERSE
        { new Pair<UOp, ValType>(UOp.NEGATION, ValType.INT), (a) => { return  new Val(-(int)a); }},
        { new Pair<UOp, ValType>(UOp.NEGATION, ValType.FLOAT), (a) => { return  new Val(-(float)a); }},
      };

    internal static Dictionary<Trip<BOp, ValType, ValType>, Func<Val, Val, Val>> binaryops
      = new Dictionary<Trip<BOp, ValType, ValType>, Func<Val, Val, Val>>
      {
        // ADD
        {new Trip<BOp, ValType, ValType>(BOp.ADD, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a + (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.ADD, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a + (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.ADD, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a + (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.ADD, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a + (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.ADD, ValType.INT, ValType.STRING), (a,b) => { return new Val((int)a + (string)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.ADD, ValType.STRING, ValType.INT), (a,b) => { return new Val((string)a + (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.ADD, ValType.STRING, ValType.STRING), (a,b) => { return new Val((string)a + (string)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.ADD, ValType.FLOAT, ValType.STRING), (a,b) => { return new Val((float)a + (string)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.ADD, ValType.STRING, ValType.FLOAT), (a,b) => { return new Val((string)a + (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.ADD, ValType.FLOAT2, ValType.FLOAT2), (a,b) => { return new Val((float2)a + (float2)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.ADD, ValType.FLOAT3, ValType.FLOAT3), (a,b) => { return new Val((float3)a + (float3)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.ADD, ValType.FLOAT4, ValType.FLOAT4), (a,b) => { return new Val((float4)a + (float4)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.ADD, ValType.FLOAT_ARRAY, ValType.FLOAT_ARRAY), (a,b) => { return new Val(MemberwiseAdd((float[])a, (float[])b)); }},
        {new Trip<BOp, ValType, ValType>(BOp.ADD, ValType.FLOAT_ARRAY, ValType.INT_ARRAY), (a,b) => { return new Val(MemberwiseAdd((float[])a, (int[])b)); }},
        {new Trip<BOp, ValType, ValType>(BOp.ADD, ValType.INT_ARRAY, ValType.FLOAT_ARRAY), (a,b) => { return new Val(MemberwiseAdd((int[])a, (float[])b)); }},
        {new Trip<BOp, ValType, ValType>(BOp.ADD, ValType.INT_ARRAY, ValType.INT_ARRAY), (a,b) => { return new Val(MemberwiseAdd((int[])a, (int[])b)); }},

        // SUBTRACT
        {new Trip<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a - (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a - (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a - (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a - (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.FLOAT2, ValType.FLOAT2), (a,b) => { return new Val((float2)a - (float2)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.FLOAT3, ValType.FLOAT3), (a,b) => { return new Val((float3)a - (float3)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.FLOAT4, ValType.FLOAT4), (a,b) => { return new Val((float4)a - (float4)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.FLOAT_ARRAY, ValType.FLOAT_ARRAY), (a,b) => { return new Val(MemberwiseSubstract((float[])a, (float[])b)); }},
        {new Trip<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.FLOAT_ARRAY, ValType.INT_ARRAY), (a,b) => { return new Val(MemberwiseSubstract((float[])a, (int[])b)); }},
        {new Trip<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.INT_ARRAY, ValType.FLOAT_ARRAY), (a,b) => { return new Val(MemberwiseSubstract((int[])a, (float[])b)); }},
        {new Trip<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.INT_ARRAY, ValType.INT_ARRAY), (a,b) => { return new Val(MemberwiseSubstract((int[])a, (int[])b)); }},

        // MULTIPLY
        {new Trip<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a * (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a * (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a * (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a * (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.FLOAT2, ValType.FLOAT), (a,b) => { return new Val((float2)a * (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.FLOAT3, ValType.FLOAT), (a,b) => { return new Val((float3)a * (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.FLOAT4, ValType.FLOAT), (a,b) => { return new Val((float4)a * (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.FLOAT2, ValType.INT), (a,b) => { return new Val((float2)a * (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.FLOAT3, ValType.INT), (a,b) => { return new Val((float3)a * (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.FLOAT4, ValType.INT), (a,b) => { return new Val((float4)a * (int)b); }},

        // DIVIDE
        {new Trip<BOp, ValType, ValType>(BOp.DIVIDE, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a / (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.DIVIDE, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a / (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.DIVIDE, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a / (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.DIVIDE, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a / (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.DIVIDE, ValType.FLOAT2, ValType.FLOAT), (a,b) => { return new Val((float2)a / (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.DIVIDE, ValType.FLOAT3, ValType.FLOAT), (a,b) => { return new Val((float3)a / (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.DIVIDE, ValType.FLOAT4, ValType.FLOAT), (a,b) => { return new Val((float4)a / (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.DIVIDE, ValType.FLOAT2, ValType.INT), (a,b) => { return new Val((float2)a / (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.DIVIDE, ValType.FLOAT3, ValType.INT), (a,b) => { return new Val((float3)a / (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.DIVIDE, ValType.FLOAT4, ValType.INT), (a,b) => { return new Val((float4)a / (int)b); }},

        // MODULUS
        {new Trip<BOp, ValType, ValType>(BOp.MODULUS, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a % (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MODULUS, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a % (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MODULUS, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a % (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MODULUS, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a % (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MODULUS, ValType.FLOAT2, ValType.FLOAT), (a,b) => { return new Val((float2)a % (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MODULUS, ValType.FLOAT3, ValType.FLOAT), (a,b) => { return new Val((float3)a % (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MODULUS, ValType.FLOAT4, ValType.FLOAT), (a,b) => { return new Val((float4)a % (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MODULUS, ValType.FLOAT2, ValType.INT), (a,b) => { return new Val((float2)a % (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MODULUS, ValType.FLOAT3, ValType.INT), (a,b) => { return new Val((float3)a % (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MODULUS, ValType.FLOAT4, ValType.INT), (a,b) => { return new Val((float4)a % (int)b); }},

        // LOGICAL_OR
        {new Trip<BOp, ValType, ValType>(BOp.LOGICAL_OR, ValType.BOOL, ValType.BOOL), (a,b) => { return new Val((bool)a || (bool)b); }},
        //{new Trip<BOp, ValType, ValType>(BOp.LOGICAL_OR, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI | b.ValueI); }},

        // LOGICAL_AND
        {new Trip<BOp, ValType, ValType>(BOp.LOGICAL_AND, ValType.BOOL, ValType.BOOL), (a,b) => { return new Val((bool)a & (bool)b); }},
        //{new Trip<BOp, ValType, ValType>(BOp.LOGICAL_AND, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI & b.ValueI); }},

        // EQUAL_TO
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.NULL), (a,b) => { return new Val((string)a == null); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.STRING), (a,b) => { return new Val((string)b == null); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a == (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a == (int)b); }},

        // MORE_THAN
        {new Trip<BOp, ValType, ValType>(BOp.MORE_THAN, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a > (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MORE_THAN, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a > (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MORE_THAN, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a > (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MORE_THAN, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a > (float)b); }},

        // MORE_THAN_OR_EQUAL_TO
        {new Trip<BOp, ValType, ValType>(BOp.MORE_THAN_OR_EQUAL_TO, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a >= (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MORE_THAN_OR_EQUAL_TO, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a >= (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MORE_THAN_OR_EQUAL_TO, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a >= (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MORE_THAN_OR_EQUAL_TO, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a >= (float)b); }},

        // LESS_THAN
        {new Trip<BOp, ValType, ValType>(BOp.LESS_THAN, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a < (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.LESS_THAN, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a < (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.LESS_THAN, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a < (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.LESS_THAN, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a < (float)b); }},

        // LESS_THAN_OR_EQUAL_TO
        {new Trip<BOp, ValType, ValType>(BOp.LESS_THAN_OR_EQUAL_TO, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a <= (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.LESS_THAN_OR_EQUAL_TO, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a <= (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.LESS_THAN_OR_EQUAL_TO, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a <= (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.LESS_THAN_OR_EQUAL_TO, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a <= (float)b); } }
      };

    /// <summary>
    /// Performs an unary operation 
    /// </summary>
    /// <param name="op">The operator</param>
    /// <param name="v">The value</param>
    /// <returns></returns>
    public static Val Do(UOp op, Val v)
    {
      Func<Val, Val> fn;
      if (!unaryops.TryGetValue(new Pair<UOp, ValType>(op, v.Type), out fn))
        throw new ArgumentException("Operation '{0}' incompatible with {1}".F(op, v.Type));

      return fn.Invoke(v);
    }

    /// <summary>
    /// Performs a binary operation 
    /// </summary>
    /// <param name="op">The operator</param>
    /// <param name="v1">The first value</param>
    /// <param name="v2">The second value</param>
    /// <returns></returns>
    public static Val Do(BOp op, Val v1, Val v2)
    {
      Func<Val, Val, Val> fn;
      if (CoerceTypes.Contains(new Pair<ValType, ValType>(v1.Type, v2.Type)))
        v2 = Coerce(v1.Type, v2);

      if (!binaryops.TryGetValue(new Trip<BOp, ValType, ValType>(op, v1.Type, v2.Type), out fn))
        throw new ArgumentException("Operation '{0}' incompatible between {1} and {2}".F(op, v1.Type, v2.Type));

      return fn.Invoke(v1, v2);
    }

    /// <summary>
    /// Performs an equal operation 
    /// </summary>
    /// <param name="v1">The first value</param>
    /// <param name="v2">The second value</param>
    /// <returns></returns>
    public static Val IsEqual(Val v1, Val v2)
    {
      Func<Val, Val, Val> fn;
      if (CoerceTypes.Contains(new Pair<ValType, ValType>(v1.Type, v2.Type)))
        v2 = Coerce(v1.Type, v2);

      if (binaryops.TryGetValue(new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, v1.Type, v2.Type), out fn))
        return fn.Invoke(v1, v2);

      if (v1.Type == v2.Type)
        return new Val(v1.Value?.Equals(v2.Value) ?? (v2.Value == null));

      return new Val(false);
    }

    /// <summary>
    /// Performs a not equal operation 
    /// </summary>
    /// <param name="v1">The first value</param>
    /// <param name="v2">The second value</param>
    /// <returns></returns>
    public static Val IsNotEqual(Val v1, Val v2)
    {
      return new Val(!(bool)IsEqual(v1, v2));
    }

    private static float[] MemberwiseAdd(float[] v1, float[] v2)
    {
      if (v1.Length != v2.Length)
        throw new InvalidOperationException("Attempted operation between two arrays of different length");

      float[] ret = new float[v1.Length];
      for (int i = 0; i < ret.Length; i++)
        ret[i] = v1[i] + v2[i];

      return ret;
    }

    private static float[] MemberwiseSubstract(float[] v1, float[] v2)
    {
      if (v1.Length != v2.Length)
        throw new InvalidOperationException("Attempted operation between two arrays of different length");

      float[] ret = new float[v1.Length];
      for (int i = 0; i < ret.Length; i++)
        ret[i] = v1[i] - v2[i];

      return ret;
    }

    private static int[] MemberwiseAdd(int[] v1, int[] v2)
    {
      if (v1.Length != v2.Length)
        throw new InvalidOperationException("Attempted operation between two arrays of different length");

      int[] ret = new int[v1.Length];
      for (int i = 0; i < ret.Length; i++)
        ret[i] = v1[i] + v2[i];

      return ret;
    }

    private static int[] MemberwiseSubstract(int[] v1, int[] v2)
    {
      if (v1.Length != v2.Length)
        throw new InvalidOperationException("Attempted operation between two arrays of different length");

      int[] ret = new int[v1.Length];
      for (int i = 0; i < ret.Length; i++)
        ret[i] = v1[i] - v2[i];

      return ret;
    }

    private static float[] MemberwiseAdd(float[] v1, int[] v2)
    {
      if (v1.Length != v2.Length)
        throw new InvalidOperationException("Attempted operation between two arrays of different length");

      float[] ret = new float[v1.Length];
      for (int i = 0; i < ret.Length; i++)
        ret[i] = v1[i] + v2[i];

      return ret;
    }

    private static float[] MemberwiseSubstract(float[] v1, int[] v2)
    {
      if (v1.Length != v2.Length)
        throw new InvalidOperationException("Attempted operation between two arrays of different length");

      float[] ret = new float[v1.Length];
      for (int i = 0; i < ret.Length; i++)
        ret[i] = v1[i] - v2[i];

      return ret;
    }

    private static float[] MemberwiseAdd(int[] v1, float[] v2)
    {
      if (v1.Length != v2.Length)
        throw new InvalidOperationException("Attempted operation between two arrays of different length");

      float[] ret = new float[v1.Length];
      for (int i = 0; i < ret.Length; i++)
        ret[i] = v1[i] + v2[i];

      return ret;
    }

    private static float[] MemberwiseSubstract(int[] v1, float[] v2)
    {
      if (v1.Length != v2.Length)
        throw new InvalidOperationException("Attempted operation between two arrays of different length");

      float[] ret = new float[v1.Length];
      for (int i = 0; i < ret.Length; i++)
        ret[i] = v1[i] - v2[i];

      return ret;
    }

    internal static Val Coerce(ValType t, Val val)
    {
      if (t == val.Type)
        return val;

      // float can accept int
      else if (t == ValType.FLOAT && val.Type == ValType.INT)
        return new Val((float)val);

      // int can accept float
      else if (t == ValType.INT && val.Type == ValType.FLOAT)
        return new Val((int)val);

      // float_array can accept int_array of same length
      else if (t == ValType.FLOAT_ARRAY && val.Type == ValType.INT_ARRAY)
      {
        int[] iv = (int[])val;
        float[] ret = new float[iv.Length];
        for (int i = 0; i < iv.Length; i++)
          ret[i] = iv[i];
        return new Val(ret);
      }

      // float2/3/4 can accept float_array/int_array of same length
      else if (t == ValType.FLOAT2 && val.Type == ValType.FLOAT_ARRAY)
      {
        float[] fv = (float[])val;
        int len = fv.Length;
        if (len == 2)
          return new Val(float2.FromArray(fv));
        else
          throw new InvalidOperationException("Attempted coercion of an array of length {0} to a type '{1}".F(len, t));
      }

      else if (t == ValType.FLOAT3 && val.Type == ValType.FLOAT_ARRAY)
      {
        float[] fv = (float[])val;
        int len = fv.Length;
        if (len == 3)
          return new Val(float3.FromArray(fv));
        else
          throw new InvalidOperationException("Attempted coercion of an array of length {0} to a type '{1}".F(len, t));
      }

      else if (t == ValType.FLOAT4 && val.Type == ValType.FLOAT_ARRAY)
      {
        float[] fv = (float[])val;
        int len = fv.Length;
        if (len == 4)
          return new Val(float4.FromArray(fv));
        else
          throw new InvalidOperationException("Attempted coercion of an array of length {0} to a type '{1}".F(len, t));
      }

      else if (t == ValType.FLOAT2 && val.Type == ValType.INT_ARRAY)
      {
        int[] fv = (int[])val;
        int len = fv.Length;
        if (len == 2)
          return new Val(float2.FromArray(fv));
        else
          throw new InvalidOperationException("Attempted coercion of an array of length {0} to a type '{1}".F(len, t));
      }

      else if (t == ValType.FLOAT3 && val.Type == ValType.INT_ARRAY)
      {
        int[] fv = (int[])val;
        int len = fv.Length;
        if (len == 3)
          return new Val(float3.FromArray(fv));
        else
          throw new InvalidOperationException("Attempted coercion of an array of length {0} to a type '{1}".F(len, t));
      }

      else if (t == ValType.FLOAT4 && val.Type == ValType.INT_ARRAY)
      {
        int[] fv = (int[])val;
        int len = fv.Length;
        if (len == 4)
          return new Val(float4.FromArray(fv));
        else
          throw new InvalidOperationException("Attempted coercion of an array of length {0} to a type '{1}".F(len, t));
      }
      throw new InvalidOperationException("Attempted coercion of value of type '{0}' to a type '{1}'".F(val.Type, t));
    }
  }
}
