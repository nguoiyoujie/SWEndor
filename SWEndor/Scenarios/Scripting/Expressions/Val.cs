using SWEndor.Primitives.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SWEndor.Scenarios.Scripting.Expressions
{
  public enum ValType : byte
  {
    NULL,
    BOOL,
    INT,
    FLOAT,
    STRING,
    FLOAT2,
    FLOAT3,
    FLOAT4,
    BOOL_ARRAY,
    INT_ARRAY,
    FLOAT_ARRAY
  }

  public enum UOp : byte
  {
    IDENTITY, // nothing
    LOGICAL_NOT, // !a
    NEGATION, // -a
  }

  public enum BOp : byte
  {
    LOGICAL_OR, // a || b
    LOGICAL_AND, // a && b
    ADD, // a + b
    SUBTRACT, // a - b
    MULTIPLY, // a * b
    MODULUS, // a % b
    DIVIDE, // a / b
    EQUAL_TO, // a == b
    NOT_EQUAL_TO, // a != b
    MORE_THAN, // a > b
    MORE_THAN_OR_EQUAL_TO, // a >= b
    LESS_THAN, // a < b
    LESS_THAN_OR_EQUAL_TO, // a <= b
  }

  [StructLayout(LayoutKind.Explicit)]
  public struct Val
  {
    [FieldOffset(0)]
    public readonly ValType Type;
    [FieldOffset(sizeof(ValType))]
    public readonly bool vB;
    [FieldOffset(sizeof(ValType))]
    private readonly int _vI;
    [FieldOffset(sizeof(ValType))]
    private readonly float _vF;
    [FieldOffset(8)]
    private readonly string _vS;
    [FieldOffset(8)]
    public readonly bool[] aB;
    [FieldOffset(8)]
    public readonly int[] aI;
    [FieldOffset(8)]
    public readonly float[] aF;

    public int vI
    {
      get
      {
        switch (Type)
        {
          case ValType.BOOL:
            return vB ? 1 : 0;
          case ValType.INT:
            return _vI;
          case ValType.FLOAT:
            return (int)_vF;
          case ValType.STRING:
            return Convert.ToInt32(_vS);
          default:
            throw new InvalidCastException("Attempted to read int value from a {0}".F(Type));
        }
      }
    }

    public float vF
    {
      get
      {
        switch (Type)
        {
          case ValType.BOOL:
            return vB ? 1 : 0;
          case ValType.INT:
            return _vI;
          case ValType.FLOAT:
            return _vF;
          case ValType.STRING:
            return Convert.ToSingle(_vS);
          default:
            throw new InvalidCastException("Attempted to read float value from a {0}".F(Type));
        }
      }
    }

    public string vS
    {
      get
      {
        switch (Type)
        {
          case ValType.BOOL:
            return vB.ToString();
          case ValType.INT:
            return _vI.ToString();
          case ValType.FLOAT:
            return _vF.ToString();
          case ValType.STRING:
            return _vS;
          default:
            return "({0})".F(Type);
            //throw new InvalidCastException("Attempted to read string value from a {0}".F(Type));
        }
      }
    }

    public object Value
    {
      get
      {
        switch (Type)
        {
          case ValType.BOOL:
            return vB;
          case ValType.INT:
            return vI;
          case ValType.FLOAT:
            return vF;
          case ValType.STRING:
            return vS;
          default:
            return null;
        }
      }
    }

    public Val(bool val)
    {
      Type = ValType.BOOL;
      aB = null;
      aI = null;
      aF = null;
      _vI = 0;
      _vF = 0;
      _vS = string.Empty;
      vB = val;
    }

    public Val(int val)
    {
      Type = ValType.INT;
      aB = null;
      aI = null;
      aF = null;
      vB = false;
      _vF = 0;
      _vS = string.Empty;
      _vI = val;
    }

    public Val(float val)
    {
      Type = ValType.FLOAT;
      aB = null;
      aI = null;
      aF = null;
      vB = false;
      _vI = 0;
      _vS = string.Empty;
      _vF = val;
    }

    public Val(string val)
    {
      Type = ValType.STRING;
      aB = null;
      aI = null;
      aF = null;
      vB = false;
      _vI = 0;
      _vF = 0;
      _vS = val ?? string.Empty;
    }

    public Val(bool[] val)
    {
      Type = ValType.INT_ARRAY;
      aI = null;
      aF = null;
      vB = false;
      _vI = 0;
      _vF = 0;
      _vS = string.Empty;
      aB = val;
    }

    public Val(int[] val)
    {
      Type = ValType.INT_ARRAY;
      aB = null;
      aF = null;
      vB = false;
      _vI = 0;
      _vF = 0;
      _vS = string.Empty;
      aI = val;
    }

    public Val(float[] val)
    {
      Type = ValType.FLOAT_ARRAY;
      aB = null;
      aI = null;
      vB = false;
      _vI = 0;
      _vF = 0;
      _vS = string.Empty;
      aF = val;
    }

    public static readonly Val NULL = new Val();
    public static readonly Val TRUE = new Val(true);
    public static readonly Val FALSE = new Val(false);

    public bool IsNull { get { return Type == ValType.NULL; } }
    public bool IsTrue { get { return Type == ValType.BOOL && vB == true; } }
    public bool IsFalse { get { return Type == ValType.BOOL && vB == false; } }

    public bool IsArray { get { return Type == ValType.BOOL_ARRAY || Type == ValType.FLOAT_ARRAY || Type == ValType.INT_ARRAY; } }

    public Array Array
    {
      get
      {
        if (Type == ValType.INT_ARRAY)
          return aI;
        if (Type == ValType.FLOAT_ARRAY)
          return aF;
        return null;
      }
    }

  }

  public static class Ops
  {
    public struct Pair<T,U>
    {
      T _t;
      U _u;
      public Pair(T t, U u) { _t = t;  _u = u; }
    }

    public struct Triple<T, U, V>
    {
      T _t;
      U _u;
      V _v;
      public Triple(T t, U u, V v) { _t = t; _u = u; _v = v; }
    }

    public static Dictionary<Pair<UOp, ValType>, Func<Val, Val>> unaryops
      = new Dictionary<Pair<UOp, ValType>, Func<Val, Val>>
      {
        // IDENTITY
        { new Pair<UOp, ValType>(UOp.IDENTITY, ValType.BOOL), (a) => { return a; } },
        { new Pair<UOp, ValType>(UOp.IDENTITY, ValType.INT), (a) => { return a; } },
        { new Pair<UOp, ValType>(UOp.IDENTITY, ValType.FLOAT), (a) => { return a; } },
        { new Pair<UOp, ValType>(UOp.IDENTITY, ValType.STRING), (a) => { return a; } },

        // NEGATION
        { new Pair<UOp, ValType>(UOp.LOGICAL_NOT, ValType.BOOL), (a) => { return new Val(!a.vB); }},

        // INVERSE
        { new Pair<UOp, ValType>(UOp.NEGATION, ValType.INT), (a) => { return  new Val(-a.vI); }},
        { new Pair<UOp, ValType>(UOp.NEGATION, ValType.FLOAT), (a) => { return  new Val(-a.vF); }},
      };

    public static Dictionary<Triple<BOp, ValType, ValType>, Func<Val, Val, Val>> binaryops
      = new Dictionary<Triple<BOp, ValType, ValType>, Func<Val, Val, Val>>
      {
        // ADD
        {new Triple<BOp, ValType, ValType>(BOp.ADD, ValType.INT, ValType.INT), (a,b) => { return new Val(a.vI + b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.ADD, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.vI + b.vF); }},
        {new Triple<BOp, ValType, ValType>(BOp.ADD, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.vF + b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.ADD, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.vF + b.vF); }},
        {new Triple<BOp, ValType, ValType>(BOp.ADD, ValType.INT, ValType.STRING), (a,b) => { return new Val(a.vI + b.vS); }},
        {new Triple<BOp, ValType, ValType>(BOp.ADD, ValType.STRING, ValType.INT), (a,b) => { return new Val(a.vS + b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.ADD, ValType.STRING, ValType.STRING), (a,b) => { return new Val(a.vS + b.vS); }},
        {new Triple<BOp, ValType, ValType>(BOp.ADD, ValType.FLOAT, ValType.STRING), (a,b) => { return new Val(a.vF + b.vS); }},
        {new Triple<BOp, ValType, ValType>(BOp.ADD, ValType.STRING, ValType.FLOAT), (a,b) => { return new Val(a.vS + b.vF); }},

        // SUBTRACT
        {new Triple<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.INT, ValType.INT), (a,b) => { return new Val(a.vI - b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.vI - b.vF); }},
        {new Triple<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.vF - b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.vF - b.vF); }},

        // MULTIPLY
        {new Triple<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.INT, ValType.INT), (a,b) => { return new Val(a.vI * b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.vI * b.vF); }},
        {new Triple<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.vF * b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.vF * b.vF); }},

        // DIVIDE
        {new Triple<BOp, ValType, ValType>(BOp.DIVIDE, ValType.INT, ValType.INT), (a,b) => { return new Val(a.vI / b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.DIVIDE, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.vI / b.vF); }},
        {new Triple<BOp, ValType, ValType>(BOp.DIVIDE, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.vF / b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.DIVIDE, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.vF / b.vF); }},

        // MODULUS
        {new Triple<BOp, ValType, ValType>(BOp.MODULUS, ValType.INT, ValType.INT), (a,b) => { return new Val(a.vI % b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.MODULUS, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.vI % b.vF); }},
        {new Triple<BOp, ValType, ValType>(BOp.MODULUS, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.vF % b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.MODULUS, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.vF % b.vF); }},

        // LOGICAL_OR
        {new Triple<BOp, ValType, ValType>(BOp.LOGICAL_OR, ValType.BOOL, ValType.BOOL), (a,b) => { return new Val(a.vB || b.vB); }},
        //{new Triple<BOp, ValType, ValType>(BOp.LOGICAL_OR, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI | b.ValueI); }},

        // LOGICAL_AND
        {new Triple<BOp, ValType, ValType>(BOp.LOGICAL_AND, ValType.BOOL, ValType.BOOL), (a,b) => { return new Val(a.vB & b.vB); }},
        //{new Triple<BOp, ValType, ValType>(BOp.LOGICAL_AND, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI & b.ValueI); }},

        // EQUAL_TO
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.NULL), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.NULL), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.NULL), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.NULL), (a,b) => { return new Val(a.vS == null); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.BOOL), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.INT), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.FLOAT), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.STRING), (a,b) => { return new Val(b.vS == null); }},

        { new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.BOOL), (a,b) => { return new Val(a.vB == b.vB); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.INT), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.BOOL), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.INT), (a,b) => { return new Val(a.vI == b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.FLOAT), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.BOOL), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.vI == b.vF); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.vF == b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.vF == b.vF); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.STRING), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.BOOL), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.STRING), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.INT), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.STRING), (a,b) => { return new Val(a.vS == b.vS); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.STRING), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.FLOAT), (a,b) => { return new Val(false); }},

        // NOT_EQUAL_TO
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.NULL), (a,b) => { return new Val(a.vS != null); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.BOOL), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.INT), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.FLOAT), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.STRING), (a,b) => { return new Val(b.vS != null); }},

        { new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.BOOL), (a,b) => { return new Val(a.vB != b.vB); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.INT), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.BOOL), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.INT), (a,b) => { return new Val(a.vI != b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.FLOAT), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.BOOL), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.vI != b.vF); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.vF != b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.vF != b.vF); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.STRING), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.BOOL), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.STRING), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.INT), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.STRING), (a,b) => { return new Val(a.vS != b.vS); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.STRING), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.FLOAT), (a,b) => { return new Val(true); }},

        // MORE_THAN
        {new Triple<BOp, ValType, ValType>(BOp.MORE_THAN, ValType.INT, ValType.INT), (a,b) => { return new Val(a.vI > b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.MORE_THAN, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.vI > b.vF); }},
        {new Triple<BOp, ValType, ValType>(BOp.MORE_THAN, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.vF > b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.MORE_THAN, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.vF > b.vF); }},

        // MORE_THAN_OR_EQUAL_TO
        {new Triple<BOp, ValType, ValType>(BOp.MORE_THAN_OR_EQUAL_TO, ValType.INT, ValType.INT), (a,b) => { return new Val(a.vI >= b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.MORE_THAN_OR_EQUAL_TO, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.vI >= b.vF); }},
        {new Triple<BOp, ValType, ValType>(BOp.MORE_THAN_OR_EQUAL_TO, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.vF >= b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.MORE_THAN_OR_EQUAL_TO, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.vF >= b.vF); }},

        // LESS_THAN
        {new Triple<BOp, ValType, ValType>(BOp.LESS_THAN, ValType.INT, ValType.INT), (a,b) => { return new Val(a.vI < b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.LESS_THAN, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.vI < b.vF); }},
        {new Triple<BOp, ValType, ValType>(BOp.LESS_THAN, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.vF < b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.LESS_THAN, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.vF < b.vF); }},

        // LESS_THAN_OR_EQUAL_TO
        {new Triple<BOp, ValType, ValType>(BOp.LESS_THAN_OR_EQUAL_TO, ValType.INT, ValType.INT), (a,b) => { return new Val(a.vI <= b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.LESS_THAN_OR_EQUAL_TO, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.vI <= b.vF); }},
        {new Triple<BOp, ValType, ValType>(BOp.LESS_THAN_OR_EQUAL_TO, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.vF <= b.vI); }},
        {new Triple<BOp, ValType, ValType>(BOp.LESS_THAN_OR_EQUAL_TO, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.vF <= b.vF); } }
      };

    public static Val Do(UOp op, Val v)
    {
      Func<Val, Val> fn;
      if (!unaryops.TryGetValue(new Pair<UOp, ValType>(op, v.Type), out fn))
        throw new ArgumentException(TextLocalization.Get(TextLocalKeys.SCRIPT_UNEXPECTED_UOP).F(op, v.Type));

      return fn.Invoke(v);
    }

    public static Val Do(BOp op, Val v1, Val v2)
    {
      Func<Val, Val, Val> fn;
      if (!binaryops.TryGetValue(new Triple<BOp, ValType, ValType>(op, v1.Type, v2.Type), out fn))
        throw new ArgumentException(TextLocalization.Get(TextLocalKeys.SCRIPT_UNEXPECTED_BOP).F(op, v1.Type, v2.Type));

      return fn.Invoke(v1, v2);
    }
  }
}
