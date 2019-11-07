using Primrose.Primitives.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SWEndor.Scenarios.Scripting.Expressions
{
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
        { new Pair<UOp, ValType>(UOp.LOGICAL_NOT, ValType.BOOL), (a) => { return new Val(!(bool)a); }},

        // INVERSE
        { new Pair<UOp, ValType>(UOp.NEGATION, ValType.INT), (a) => { return  new Val(-(int)a); }},
        { new Pair<UOp, ValType>(UOp.NEGATION, ValType.FLOAT), (a) => { return  new Val(-(float)a); }},
      };

    public static Dictionary<Triple<BOp, ValType, ValType>, Func<Val, Val, Val>> binaryops
      = new Dictionary<Triple<BOp, ValType, ValType>, Func<Val, Val, Val>>
      {
        // ADD
        {new Triple<BOp, ValType, ValType>(BOp.ADD, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a + (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.ADD, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a + (float)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.ADD, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a + (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.ADD, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a + (float)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.ADD, ValType.INT, ValType.STRING), (a,b) => { return new Val((int)a + (string)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.ADD, ValType.STRING, ValType.INT), (a,b) => { return new Val((string)a + (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.ADD, ValType.STRING, ValType.STRING), (a,b) => { return new Val((string)a + (string)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.ADD, ValType.FLOAT, ValType.STRING), (a,b) => { return new Val((float)a + (string)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.ADD, ValType.STRING, ValType.FLOAT), (a,b) => { return new Val((string)a + (float)b); }},

        // SUBTRACT
        {new Triple<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a - (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a - (float)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a - (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a - (float)b); }},

        // MULTIPLY
        {new Triple<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a * (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a * (float)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a * (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a * (float)b); }},

        // DIVIDE
        {new Triple<BOp, ValType, ValType>(BOp.DIVIDE, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a / (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.DIVIDE, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a / (float)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.DIVIDE, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a / (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.DIVIDE, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a / (float)b); }},

        // MODULUS
        {new Triple<BOp, ValType, ValType>(BOp.MODULUS, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a % (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.MODULUS, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a % (float)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.MODULUS, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a % (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.MODULUS, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a % (float)b); }},

        // LOGICAL_OR
        {new Triple<BOp, ValType, ValType>(BOp.LOGICAL_OR, ValType.BOOL, ValType.BOOL), (a,b) => { return new Val((bool)a || (bool)b); }},
        //{new Triple<BOp, ValType, ValType>(BOp.LOGICAL_OR, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI | b.ValueI); }},

        // LOGICAL_AND
        {new Triple<BOp, ValType, ValType>(BOp.LOGICAL_AND, ValType.BOOL, ValType.BOOL), (a,b) => { return new Val((bool)a & (bool)b); }},
        //{new Triple<BOp, ValType, ValType>(BOp.LOGICAL_AND, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI & b.ValueI); }},

        // EQUAL_TO
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.NULL), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.NULL), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.NULL), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.NULL), (a,b) => { return new Val((string)a == null); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.BOOL), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.INT), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.FLOAT), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.STRING), (a,b) => { return new Val((string)b == null); }},

        { new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.BOOL), (a,b) => { return new Val((bool)a == (bool)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.INT), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.BOOL), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a == (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.FLOAT), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.BOOL), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a == (float)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a == (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a == (float)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.STRING), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.BOOL), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.STRING), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.INT), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.STRING), (a,b) => { return new Val((string)a == (string)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.STRING), (a,b) => { return new Val(false); }},
        {new Triple<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.FLOAT), (a,b) => { return new Val(false); }},

        // NOT_EQUAL_TO
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.NULL), (a,b) => { return new Val((string)a != null); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.BOOL), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.INT), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.FLOAT), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.STRING), (a,b) => { return new Val((string)b != null); }},

        { new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.BOOL), (a,b) => { return new Val((bool)a != (bool)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.INT), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.BOOL), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a != (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.FLOAT), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.BOOL), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a != (float)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a != (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a != (float)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.STRING), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.BOOL), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.STRING), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.INT), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.STRING), (a,b) => { return new Val((string)a != (string)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.STRING), (a,b) => { return new Val(true); }},
        {new Triple<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.FLOAT), (a,b) => { return new Val(true); }},

        // MORE_THAN
        {new Triple<BOp, ValType, ValType>(BOp.MORE_THAN, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a > (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.MORE_THAN, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a > (float)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.MORE_THAN, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a > (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.MORE_THAN, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a > (float)b); }},

        // MORE_THAN_OR_EQUAL_TO
        {new Triple<BOp, ValType, ValType>(BOp.MORE_THAN_OR_EQUAL_TO, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a >= (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.MORE_THAN_OR_EQUAL_TO, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a >= (float)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.MORE_THAN_OR_EQUAL_TO, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a >= (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.MORE_THAN_OR_EQUAL_TO, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a >= (float)b); }},

        // LESS_THAN
        {new Triple<BOp, ValType, ValType>(BOp.LESS_THAN, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a < (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.LESS_THAN, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a < (float)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.LESS_THAN, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a < (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.LESS_THAN, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a < (float)b); }},

        // LESS_THAN_OR_EQUAL_TO
        {new Triple<BOp, ValType, ValType>(BOp.LESS_THAN_OR_EQUAL_TO, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a <= (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.LESS_THAN_OR_EQUAL_TO, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a <= (float)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.LESS_THAN_OR_EQUAL_TO, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a <= (int)b); }},
        {new Triple<BOp, ValType, ValType>(BOp.LESS_THAN_OR_EQUAL_TO, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a <= (float)b); } }
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
