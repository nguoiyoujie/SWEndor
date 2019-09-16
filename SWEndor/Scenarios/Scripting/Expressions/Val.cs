using SWEndor.Primitives;
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
    [FieldOffset(sizeof(ValType))] // 2
    public readonly bool ValueB;
    [FieldOffset(sizeof(ValType))] // 2
    private readonly int valueI;
    [FieldOffset(sizeof(ValType))] // 2
    private readonly float valueF;
    [FieldOffset(8)]
    private readonly string valueS;
    [FieldOffset(8)]
    public readonly int[] ArrayI;
    [FieldOffset(8)]
    public readonly float[] ArrayF;

    public int ValueI
    {
      get
      {
        switch (Type)
        {
          case ValType.BOOL:
            return ValueB ? 1 : 0;
          case ValType.INT:
            return valueI;
          case ValType.FLOAT:
            return (int)valueF;
          case ValType.STRING:
            return Convert.ToInt32(valueS);
          default:
            return 0;
        }
      }
    }

    public float ValueF
    {
      get
      {
        switch (Type)
        {
          case ValType.BOOL:
            return ValueB ? 1 : 0;
          case ValType.INT:
            return valueI;
          case ValType.FLOAT:
            return valueF;
          case ValType.STRING:
            return Convert.ToSingle(valueS);
          default:
            return 0;
        }
      }
    }

    public string ValueS
    {
      get
      {
        switch (Type)
        {
          case ValType.BOOL:
            return ValueB.ToString();
          case ValType.INT:
            return valueI.ToString();
          case ValType.FLOAT:
            return valueF.ToString();
          case ValType.STRING:
            return valueS;
          default:
            return string.Empty;
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
            return ValueB;
          case ValType.INT:
            return ValueI;
          case ValType.FLOAT:
            return ValueF;
          case ValType.STRING:
            return ValueS;
          default:
            return null;
        }
      }
    }

    public Val(bool val)
    {
      Type = ValType.BOOL;
      ArrayI = null;
      ArrayF = null;
      valueI = 0;
      valueF = 0;
      valueS = string.Empty;
      ValueB = val;
    }

    public Val(int val)
    {
      Type = ValType.INT;
      ArrayI = null;
      ArrayF = null;
      ValueB = false;
      valueF = 0;
      valueS = string.Empty;
      valueI = val;
    }

    public Val(float val)
    {
      Type = ValType.FLOAT;
      ArrayI = null;
      ArrayF = null;
      ValueB = false;
      valueI = 0;
      valueS = string.Empty;
      valueF = val;
    }

    public Val(string val)
    {
      Type = ValType.STRING;
      ArrayI = null;
      ArrayF = null;
      ValueB = false;
      valueI = 0;
      valueF = 0;
      valueS = val ?? string.Empty;
    }

    public Val(int[] val)
    {
      Type = ValType.INT_ARRAY;
      ArrayF = null;
      ValueB = false;
      valueI = 0;
      valueF = 0;
      valueS = string.Empty;
      ArrayI = val;
    }

    public Val(float[] val)
    {
      Type = ValType.FLOAT_ARRAY;
      ArrayI = null;
      ValueB = false;
      valueI = 0;
      valueF = 0;
      valueS = string.Empty;
      ArrayF = val;
    }

    public static readonly Val NULL = new Val();
    public static readonly Val TRUE = new Val(true);
    public static readonly Val FALSE = new Val(false);

    public bool IsNull { get { return Type == ValType.NULL; } }
    public bool IsTrue { get { return Type == ValType.BOOL && ValueB == true; } }
    public bool IsFalse { get { return Type == ValType.BOOL && ValueB == false; } }

    public bool IsArray { get { return Type == ValType.FLOAT_ARRAY || Type == ValType.INT_ARRAY; } }

    public Array Array
    {
      get
      {
        if (Type == ValType.INT_ARRAY)
          return ArrayI;
        if (Type == ValType.FLOAT_ARRAY)
          return ArrayF;
        return null;
      }
    }

  }

  public static class Ops
  {
    public static Dictionary<Tuple<UOp, ValType>, Func<Val, Val>> unaryops
      = new Dictionary<Tuple<UOp, ValType>, Func<Val, Val>>
      {
        // IDENTITY
        { new Tuple<UOp, ValType>(UOp.IDENTITY, ValType.BOOL), (a) => { return a; } },
        { new Tuple<UOp, ValType>(UOp.IDENTITY, ValType.INT), (a) => { return a; } },
        { new Tuple<UOp, ValType>(UOp.IDENTITY, ValType.FLOAT), (a) => { return a; } },
        { new Tuple<UOp, ValType>(UOp.IDENTITY, ValType.STRING), (a) => { return a; } },

        // NEGATION
        { new Tuple<UOp, ValType>(UOp.LOGICAL_NOT, ValType.BOOL), (a) => { return new Val(!a.ValueB); }},

        // INVERSE
        { new Tuple<UOp, ValType>(UOp.NEGATION, ValType.INT), (a) => { return  new Val(-a.ValueI); }},
        { new Tuple<UOp, ValType>(UOp.NEGATION, ValType.FLOAT), (a) => { return  new Val(-a.ValueF); }},
      };

    public static Dictionary<Tuple<BOp, ValType, ValType>, Func<Val, Val, Val>> binaryops
      = new Dictionary<Tuple<BOp, ValType, ValType>, Func<Val, Val, Val>>
      {
        // ADD
        {new Tuple<BOp, ValType, ValType>(BOp.ADD, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI + b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.ADD, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.ValueI + b.ValueF); }},
        {new Tuple<BOp, ValType, ValType>(BOp.ADD, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.ValueF + b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.ADD, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.ValueF + b.ValueF); }},
        {new Tuple<BOp, ValType, ValType>(BOp.ADD, ValType.INT, ValType.STRING), (a,b) => { return new Val(a.ValueI + b.ValueS); }},
        {new Tuple<BOp, ValType, ValType>(BOp.ADD, ValType.STRING, ValType.INT), (a,b) => { return new Val(a.ValueS + b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.ADD, ValType.STRING, ValType.STRING), (a,b) => { return new Val(a.ValueS + b.ValueS); }},
        {new Tuple<BOp, ValType, ValType>(BOp.ADD, ValType.FLOAT, ValType.STRING), (a,b) => { return new Val(a.ValueF + b.ValueS); }},
        {new Tuple<BOp, ValType, ValType>(BOp.ADD, ValType.STRING, ValType.FLOAT), (a,b) => { return new Val(a.ValueS + b.ValueF); }},

        // SUBTRACT
        {new Tuple<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI - b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.ValueI - b.ValueF); }},
        {new Tuple<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.ValueF - b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.ValueF - b.ValueF); }},

        // MULTIPLY
        {new Tuple<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI * b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.ValueI * b.ValueF); }},
        {new Tuple<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.ValueF * b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.ValueF * b.ValueF); }},

        // DIVIDE
        {new Tuple<BOp, ValType, ValType>(BOp.DIVIDE, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI / b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.DIVIDE, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.ValueI / b.ValueF); }},
        {new Tuple<BOp, ValType, ValType>(BOp.DIVIDE, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.ValueF / b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.DIVIDE, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.ValueF / b.ValueF); }},

        // MODULUS
        {new Tuple<BOp, ValType, ValType>(BOp.MODULUS, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI % b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.MODULUS, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.ValueI % b.ValueF); }},
        {new Tuple<BOp, ValType, ValType>(BOp.MODULUS, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.ValueF % b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.MODULUS, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.ValueF % b.ValueF); }},

        // LOGICAL_OR
        {new Tuple<BOp, ValType, ValType>(BOp.LOGICAL_OR, ValType.BOOL, ValType.BOOL), (a,b) => { return new Val(a.ValueB || b.ValueB); }},
        //{new Tuple<BOp, ValType, ValType>(BOp.LOGICAL_OR, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI | b.ValueI); }},

        // LOGICAL_AND
        {new Tuple<BOp, ValType, ValType>(BOp.LOGICAL_AND, ValType.BOOL, ValType.BOOL), (a,b) => { return new Val(a.ValueB & b.ValueB); }},
        //{new Tuple<BOp, ValType, ValType>(BOp.LOGICAL_AND, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI & b.ValueI); }},

        // EQUAL_TO
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.NULL), (a,b) => { return new Val(false); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.NULL), (a,b) => { return new Val(false); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.NULL), (a,b) => { return new Val(false); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.NULL), (a,b) => { return new Val(a.ValueS == null); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.BOOL), (a,b) => { return new Val(false); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.INT), (a,b) => { return new Val(false); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.FLOAT), (a,b) => { return new Val(false); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.STRING), (a,b) => { return new Val(b.ValueS == null); }},

        { new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.BOOL), (a,b) => { return new Val(a.ValueB == b.ValueB); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.INT), (a,b) => { return new Val(false); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.BOOL), (a,b) => { return new Val(false); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI == b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.FLOAT), (a,b) => { return new Val(false); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.BOOL), (a,b) => { return new Val(false); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.ValueI == b.ValueF); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.ValueF == b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.ValueF == b.ValueF); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.STRING), (a,b) => { return new Val(false); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.BOOL), (a,b) => { return new Val(false); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.STRING), (a,b) => { return new Val(false); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.INT), (a,b) => { return new Val(false); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.STRING), (a,b) => { return new Val(a.ValueS == b.ValueS); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.STRING), (a,b) => { return new Val(false); }},
        {new Tuple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.FLOAT), (a,b) => { return new Val(false); }},

        // NOT_EQUAL_TO
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.NULL), (a,b) => { return new Val(a.ValueS != null); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.BOOL), (a,b) => { return new Val(true); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.INT), (a,b) => { return new Val(true); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.FLOAT), (a,b) => { return new Val(true); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.STRING), (a,b) => { return new Val(b.ValueS != null); }},

        { new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.BOOL), (a,b) => { return new Val(a.ValueB != b.ValueB); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.INT), (a,b) => { return new Val(true); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.BOOL), (a,b) => { return new Val(true); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI != b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.FLOAT), (a,b) => { return new Val(true); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.BOOL), (a,b) => { return new Val(true); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.ValueI != b.ValueF); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.ValueF != b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.ValueF != b.ValueF); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.STRING), (a,b) => { return new Val(true); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.BOOL), (a,b) => { return new Val(true); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.STRING), (a,b) => { return new Val(true); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.INT), (a,b) => { return new Val(true); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.STRING), (a,b) => { return new Val(a.ValueS != b.ValueS); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.STRING), (a,b) => { return new Val(true); }},
        {new Tuple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.FLOAT), (a,b) => { return new Val(true); }},

        // MORE_THAN
        {new Tuple<BOp, ValType, ValType>(BOp.MORE_THAN, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI > b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.MORE_THAN, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.ValueI > b.ValueF); }},
        {new Tuple<BOp, ValType, ValType>(BOp.MORE_THAN, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.ValueF > b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.MORE_THAN, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.ValueF > b.ValueF); }},

        // MORE_THAN_OR_EQUAL_TO
        {new Tuple<BOp, ValType, ValType>(BOp.MORE_THAN_OR_EQUAL_TO, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI >= b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.MORE_THAN_OR_EQUAL_TO, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.ValueI >= b.ValueF); }},
        {new Tuple<BOp, ValType, ValType>(BOp.MORE_THAN_OR_EQUAL_TO, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.ValueF >= b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.MORE_THAN_OR_EQUAL_TO, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.ValueF >= b.ValueF); }},

        // LESS_THAN
        {new Tuple<BOp, ValType, ValType>(BOp.LESS_THAN, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI < b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.LESS_THAN, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.ValueI < b.ValueF); }},
        {new Tuple<BOp, ValType, ValType>(BOp.LESS_THAN, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.ValueF < b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.LESS_THAN, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.ValueF < b.ValueF); }},

        // LESS_THAN_OR_EQUAL_TO
        {new Tuple<BOp, ValType, ValType>(BOp.LESS_THAN_OR_EQUAL_TO, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI <= b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.LESS_THAN_OR_EQUAL_TO, ValType.INT, ValType.FLOAT), (a,b) => { return new Val(a.ValueI <= b.ValueF); }},
        {new Tuple<BOp, ValType, ValType>(BOp.LESS_THAN_OR_EQUAL_TO, ValType.FLOAT, ValType.INT), (a,b) => { return new Val(a.ValueF <= b.ValueI); }},
        {new Tuple<BOp, ValType, ValType>(BOp.LESS_THAN_OR_EQUAL_TO, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val(a.ValueF <= b.ValueF); } }
      };

    public static Val Do(UOp op, Val v)
    {
      Func<Val, Val> fn;
      if (!unaryops.TryGetValue(new Tuple<UOp, ValType>(op, v.Type), out fn))
        throw new ArgumentException("Operation '{0}' incompatible with {1}".F(op, v.Type));

      return fn.Invoke(v);
    }

    public static Val Do(BOp op, Val v1, Val v2)
    {
      Func<Val, Val, Val> fn;
      if (!binaryops.TryGetValue(new Tuple<BOp, ValType, ValType>(op, v1.Type, v2.Type), out fn))
        throw new ArgumentException("Operation '{0}' incompatible between {1} and {2}".F(op, v1.Type, v2.Type));

      return fn.Invoke(v1, v2);
    }
  }
}
