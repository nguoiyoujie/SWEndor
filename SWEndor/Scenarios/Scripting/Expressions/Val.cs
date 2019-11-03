using SWEndor.Primitives.Extensions;
using SWEndor.Scenarios.Scripting.Expressions.Primitives;
using System;
using System.Runtime.InteropServices;

namespace SWEndor.Scenarios.Scripting.Expressions
{
  [StructLayout(LayoutKind.Explicit)]
  internal struct ValObj
  {
    [FieldOffset(0)]
    internal readonly string vS;
    [FieldOffset(0)]
    internal readonly bool[] aB;
    [FieldOffset(0)]
    internal readonly int[] aI;
    [FieldOffset(0)]
    internal readonly float[] aF;

    public ValObj(string val)
    {
      aB = null;
      aI = null;
      aF = null;
      vS = val ?? string.Empty;
    }

    public ValObj(float2 val)
    {
      vS = null;
      aB = null;
      aI = null;
      aF = val.ToArray();
    }

    public ValObj(float3 val)
    {
      vS = null;
      aB = null;
      aI = null;
      aF = val.ToArray();
    }

    public ValObj(float4 val)
    {
      vS = null;
      aB = null;
      aI = null;
      aF = val.ToArray();
    }

    public ValObj(bool[] val)
    {
      vS = null;
      aI = null;
      aF = null;
      aB = val;
    }

    public ValObj(int[] val)
    {
      vS = null;
      aB = null;
      aF = null;
      aI = val;
    }

    public ValObj(float[] val)
    {
      vS = null;
      aB = null;
      aI = null;
      aF = val;
    }
  }

  [StructLayout(LayoutKind.Explicit)]
  public struct Val
  {
    [FieldOffset(0)]
    internal readonly ValType Type;
    [FieldOffset(sizeof(ValType))]
    private readonly bool _vB; // size 1
    [FieldOffset(sizeof(ValType))]
    private readonly int _vI; // size 4
    [FieldOffset(sizeof(ValType))]
    private readonly float _vF; // size 4
    [FieldOffset(8)]
    private readonly ValObj _obj;

    public static explicit operator bool(Val a)
    {
      switch (a.Type)
      {
        case ValType.BOOL:
          return a._vB;
        default:
          throw new InvalidCastException("Attempted to read bool value from a {0}".F(a.Type));
      }
    }

    public static explicit operator int(Val a)
    {
      switch (a.Type)
      {
        case ValType.INT:
          return a._vI;
        case ValType.FLOAT:
          return (int)a._vF;
        case ValType.STRING:
          return Convert.ToInt32(a._obj.vS);
        default:
          throw new InvalidCastException("Attempted to read int value from a {0}".F(a.Type));
      }
    }

    public static explicit operator float(Val a)
    {
      switch (a.Type)
      {
        case ValType.INT:
          return a._vI;
        case ValType.FLOAT:
          return a._vF;
        case ValType.STRING:
          return Convert.ToSingle(a._obj.vS);
        default:
          throw new InvalidCastException("Attempted to read float value from a {0}".F(a.Type));
      }
    }

    public static explicit operator string(Val a)
    {
      switch (a.Type)
      {
        case ValType.BOOL:
          return a._vB.ToString();
        case ValType.INT:
          return a._vI.ToString();
        case ValType.FLOAT:
          return a._vF.ToString();
        case ValType.STRING:
          return a._obj.vS;
        default:
          return "({0})".F(a.Type);
      }
    }

    public static explicit operator float2 (Val a)
    {
      switch (a.Type)
      {
        case ValType.FLOAT2:
        case ValType.FLOAT_ARRAY:
          return float2.FromArray(a._obj.aF);
        case ValType.INT_ARRAY:
          return float2.FromArray(a._obj.aI);
        default:
          throw new InvalidCastException("Attempted to read float2 value from a {0}".F(a.Type));
      }
    }

    public static explicit operator float3(Val a)
    {
      switch (a.Type)
      {
        case ValType.FLOAT3:
        case ValType.FLOAT_ARRAY:
          return float3.FromArray(a._obj.aF);
        case ValType.INT_ARRAY:
          return float3.FromArray(a._obj.aI);
        default:
          throw new InvalidCastException("Attempted to read float3 value from a {0}".F(a.Type));
      }
    }

    public static explicit operator float4(Val a)
    {
      switch (a.Type)
      {
        case ValType.FLOAT4:
        case ValType.FLOAT_ARRAY:
          return float4.FromArray(a._obj.aF);
        case ValType.INT_ARRAY:
          return float4.FromArray(a._obj.aI);
        default:
          throw new InvalidCastException("Attempted to read float4 value from a {0}".F(a.Type));
      }
    }

    public static explicit operator bool[](Val a)
    {
      try { return a._obj.aB; }
      catch { throw new InvalidCastException("Attempted to read bool[] value from a {0}".F(a.Type)); }
    }

    public static explicit operator int[] (Val a)
    {
      try { return a._obj.aI; }
      catch { throw new InvalidCastException("Attempted to read int[] value from a {0}".F(a.Type)); }
    }

    public static explicit operator float[] (Val a)
    {
      try { return a._obj.aF; }
      catch { throw new InvalidCastException("Attempted to read float[] value from a {0}".F(a.Type)); }
    }

    public object Value
    {
      get
      {
        switch (Type)
        {
          case ValType.BOOL:
            return _vB;
          case ValType.INT:
            return _vI;
          case ValType.FLOAT:
            return _vF;
          case ValType.STRING:
            return _obj.vS;
          case ValType.BOOL_ARRAY:
            return _obj.aB;
          case ValType.INT_ARRAY:
            return _obj.aI;
          case ValType.FLOAT2:
          case ValType.FLOAT3:
          case ValType.FLOAT4:
          case ValType.FLOAT_ARRAY:
            return _obj.aF;
          default:
            return null;
        }
      }
    }

    public Val(ValType type)
    {
      Type = type;

      _obj = new ValObj();
      _vI = 0;
      _vF = 0;
      _vB = false;
    }

    public Val(bool val)
    {
      Type = ValType.BOOL;
      _obj = new ValObj();
      _vI = 0;
      _vF = 0;
      _vB = val;
    }

    public Val(int val)
    {
      Type = ValType.INT;
      _obj = new ValObj();
      _vB = false;
      _vF = 0;
      _vI = val;
    }

    public Val(float val)
    {
      Type = ValType.FLOAT;
      _obj = new ValObj();
      _vB = false;
      _vI = 0;
      _vF = val;
    }

    public Val(string val)
    {
      Type = ValType.STRING;
      _vB = false;
      _vI = 0;
      _vF = 0;
      _obj = new ValObj(val ?? string.Empty);
    }

    public Val(float2 val)
    {
      Type = ValType.FLOAT2;
      _vB = false;
      _vI = 0;
      _vF = 0;
      _obj = new ValObj(val);
    }

    public Val(float3 val)
    {
      Type = ValType.FLOAT3;
      _vB = false;
      _vI = 0;
      _vF = 0;
      _obj = new ValObj(val);
    }

    public Val(float4 val)
    {
      Type = ValType.FLOAT4;
      _vB = false;
      _vI = 0;
      _vF = 0;
      _obj = new ValObj(val);
    }

    public Val(bool[] val)
    {
      Type = ValType.INT_ARRAY;
      _vB = false;
      _vI = 0;
      _vF = 0;
      _obj = new ValObj(val);
    }

    public Val(int[] val)
    {
      Type = ValType.INT_ARRAY;
      _vB = false;
      _vI = 0;
      _vF = 0;
      _obj = new ValObj(val);
    }

    public Val(float[] val)
    {
      Type = ValType.FLOAT_ARRAY;
      _vB = false;
      _vI = 0;
      _vF = 0;
      _obj = new ValObj(val);
    }

    public static readonly Val NULL = new Val();
    public static readonly Val TRUE = new Val(true);
    public static readonly Val FALSE = new Val(false);

    public bool IsNull { get { return Type == ValType.NULL; } }
    public bool IsTrue { get { return Type == ValType.BOOL && _vB == true; } }
    public bool IsFalse { get { return Type == ValType.BOOL && _vB == false; } }

    public bool IsArray { get { return Type == ValType.BOOL_ARRAY || Type == ValType.FLOAT_ARRAY || Type == ValType.INT_ARRAY; } }
  }
}
