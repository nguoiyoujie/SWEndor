using Primrose.Expressions;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.ValueTypes;
using System;
using System.Collections.Generic;

namespace Primrose.Expressions.UnitTests.Scripting
{
  public static class ValParamContract
  {
    static Func<Val, bool> v_bool = new Func<Val, bool>((v) => { return (bool)v; });
    static Func<Val, int> v_int = new Func<Val, int>((v) => { return (int)v; });
    static Func<Val, float> v_float = new Func<Val, float>((v) => { return (float)v; });
    static Func<Val, float2> v_float2 = new Func<Val, float2>((v) => { return (float2)v; });
    static Func<Val, float3> v_float3 = new Func<Val, float3>((v) => { return (float3)v; });
    static Func<Val, float4> v_float4 = new Func<Val, float4>((v) => { return (float4)v; });
    static Func<Val, string> v_string = new Func<Val, string>((v) => { return (string)v; });
    static Func<Val, bool[]> v_bool_a = new Func<Val, bool[]>((v) => { return (bool[])v; });
    static Func<Val, int[]> v_int_a = new Func<Val, int[]>((v) => { return (int[])v; });
    static Func<Val, float[]> v_float_a = new Func<Val, float[]>((v) => { return (float[])v; });
    static Func<Val, string[]> v_string_a = new Func<Val, string[]>((v) => { return (string[])v; });

    static Func<Val, Val> v_val = new Func<Val, Val>((v) => { return v; });


    static Dictionary<Pair<Type, ValType>, object> contracts = new Dictionary<Pair<Type, ValType>, object>
    {
      { new Pair<Type, ValType>(typeof(bool), ValType.BOOL), v_bool },
      { new Pair<Type, ValType>(typeof(int), ValType.INT), v_int },
      { new Pair<Type, ValType>(typeof(float), ValType.INT), v_float },
      { new Pair<Type, ValType>(typeof(float), ValType.FLOAT), v_float },
      { new Pair<Type, ValType>(typeof(string), ValType.STRING), v_string },
      { new Pair<Type, ValType>(typeof(float2), ValType.FLOAT2), v_float2 },
      { new Pair<Type, ValType>(typeof(float2), ValType.FLOAT_ARRAY), v_float2 },
      { new Pair<Type, ValType>(typeof(float2), ValType.INT_ARRAY), v_float2 },
      { new Pair<Type, ValType>(typeof(float3), ValType.FLOAT3), v_float3 },
      { new Pair<Type, ValType>(typeof(float3), ValType.FLOAT_ARRAY), v_float3 },
      { new Pair<Type, ValType>(typeof(float3), ValType.INT_ARRAY), v_float3 },
      { new Pair<Type, ValType>(typeof(float4), ValType.FLOAT4), v_float4 },
      { new Pair<Type, ValType>(typeof(float4), ValType.FLOAT_ARRAY), v_float4 },
      { new Pair<Type, ValType>(typeof(float4), ValType.INT_ARRAY), v_float4 },
      { new Pair<Type, ValType>(typeof(bool[]), ValType.BOOL_ARRAY), v_bool_a },
      { new Pair<Type, ValType>(typeof(int[]), ValType.INT_ARRAY), v_int_a },
      { new Pair<Type, ValType>(typeof(float[]), ValType.FLOAT_ARRAY), v_float_a },
      { new Pair<Type, ValType>(typeof(float[]), ValType.FLOAT2), v_float_a },
      { new Pair<Type, ValType>(typeof(float[]), ValType.FLOAT3), v_float_a },
      { new Pair<Type, ValType>(typeof(float[]), ValType.FLOAT4), v_float_a },
      { new Pair<Type, ValType>(typeof(string[]), ValType.STRING_ARRAY), v_string_a },

      { new Pair<Type, ValType>(typeof(Val), ValType.BOOL), v_val },
      { new Pair<Type, ValType>(typeof(Val), ValType.INT), v_val },
      { new Pair<Type, ValType>(typeof(Val), ValType.FLOAT), v_val },
      { new Pair<Type, ValType>(typeof(Val), ValType.STRING), v_val },
      { new Pair<Type, ValType>(typeof(Val), ValType.FLOAT2), v_val },
      { new Pair<Type, ValType>(typeof(Val), ValType.FLOAT3), v_val },
      { new Pair<Type, ValType>(typeof(Val), ValType.FLOAT4), v_val },
      { new Pair<Type, ValType>(typeof(Val), ValType.BOOL_ARRAY), v_val },
      { new Pair<Type, ValType>(typeof(Val), ValType.INT_ARRAY), v_val },
      { new Pair<Type, ValType>(typeof(Val), ValType.FLOAT_ARRAY), v_val },
      { new Pair<Type, ValType>(typeof(Val), ValType.STRING_ARRAY), v_val },
    };

    public static T Convert<T>(ITracker caller, string _funcName, int argnum, Val v)
    {
      Pair<Type, ValType> tv = new Pair<Type, ValType>(typeof(T), v.Type);
      object ofunc = null;
      if (!contracts.TryGetValue(tv, out ofunc))
        throw new EvalException(caller, "Argument type mismatch for argument {0} of function '{1}': expected {2}, received {3}".F(argnum, _funcName, typeof(T).Name, v.Type));

      Func<Val, T> func = (Func<Val, T>)ofunc;
      return func(v);
    }
  }
}
