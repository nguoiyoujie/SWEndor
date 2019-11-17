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

        // SUBTRACT
        {new Trip<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a - (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a - (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a - (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.SUBTRACT, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a - (float)b); }},

        // MULTIPLY
        {new Trip<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a * (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a * (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a * (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MULTIPLY, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a * (float)b); }},

        // DIVIDE
        {new Trip<BOp, ValType, ValType>(BOp.DIVIDE, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a / (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.DIVIDE, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a / (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.DIVIDE, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a / (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.DIVIDE, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a / (float)b); }},

        // MODULUS
        {new Trip<BOp, ValType, ValType>(BOp.MODULUS, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a % (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MODULUS, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a % (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MODULUS, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a % (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.MODULUS, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a % (float)b); }},

        // LOGICAL_OR
        {new Trip<BOp, ValType, ValType>(BOp.LOGICAL_OR, ValType.BOOL, ValType.BOOL), (a,b) => { return new Val((bool)a || (bool)b); }},
        //{new Trip<BOp, ValType, ValType>(BOp.LOGICAL_OR, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI | b.ValueI); }},

        // LOGICAL_AND
        {new Trip<BOp, ValType, ValType>(BOp.LOGICAL_AND, ValType.BOOL, ValType.BOOL), (a,b) => { return new Val((bool)a & (bool)b); }},
        //{new Trip<BOp, ValType, ValType>(BOp.LOGICAL_AND, ValType.INT, ValType.INT), (a,b) => { return new Val(a.ValueI & b.ValueI); }},

        // EQUAL_TO
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.NULL), (a,b) => { return new Val(false); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.NULL), (a,b) => { return new Val(false); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.NULL), (a,b) => { return new Val(false); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.NULL), (a,b) => { return new Val((string)a == null); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.BOOL), (a,b) => { return new Val(false); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.INT), (a,b) => { return new Val(false); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.FLOAT), (a,b) => { return new Val(false); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.NULL, ValType.STRING), (a,b) => { return new Val((string)b == null); }},

        { new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.BOOL), (a,b) => { return new Val((bool)a == (bool)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.INT), (a,b) => { return new Val(false); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.BOOL), (a,b) => { return new Val(false); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a == (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.FLOAT), (a,b) => { return new Val(false); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.BOOL), (a,b) => { return new Val(false); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a == (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a == (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a == (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.BOOL, ValType.STRING), (a,b) => { return new Val(false); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.BOOL), (a,b) => { return new Val(false); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.INT, ValType.STRING), (a,b) => { return new Val(false); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.INT), (a,b) => { return new Val(false); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.STRING), (a,b) => { return new Val((string)a == (string)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.FLOAT, ValType.STRING), (a,b) => { return new Val(false); }},
        {new Trip<BOp, ValType, ValType>(BOp.EQUAL_TO, ValType.STRING, ValType.FLOAT), (a,b) => { return new Val(false); }},

        // NOT_EQUAL_TO
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.NULL), (a,b) => { return new Val((string)a != null); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.NULL), (a,b) => { return new Val(true); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.BOOL), (a,b) => { return new Val(true); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.INT), (a,b) => { return new Val(true); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.FLOAT), (a,b) => { return new Val(true); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.NULL, ValType.STRING), (a,b) => { return new Val((string)b != null); }},

        { new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.BOOL), (a,b) => { return new Val((bool)a != (bool)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.INT), (a,b) => { return new Val(true); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.BOOL), (a,b) => { return new Val(true); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.INT), (a,b) => { return new Val((int)a != (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.FLOAT), (a,b) => { return new Val(true); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.BOOL), (a,b) => { return new Val(true); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.FLOAT), (a,b) => { return new Val((int)a != (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.INT), (a,b) => { return new Val((float)a != (int)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.FLOAT), (a,b) => { return new Val((float)a != (float)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.BOOL, ValType.STRING), (a,b) => { return new Val(true); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.BOOL), (a,b) => { return new Val(true); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.INT, ValType.STRING), (a,b) => { return new Val(true); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.INT), (a,b) => { return new Val(true); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.STRING), (a,b) => { return new Val((string)a != (string)b); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.FLOAT, ValType.STRING), (a,b) => { return new Val(true); }},
        {new Trip<BOp, ValType, ValType>(BOp.NOT_EQUAL_TO, ValType.STRING, ValType.FLOAT), (a,b) => { return new Val(true); }},

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
      if (!binaryops.TryGetValue(new Trip<BOp, ValType, ValType>(op, v1.Type, v2.Type), out fn))
        throw new ArgumentException("Operation '{0}' incompatible between {1} and {2}".F(op, v1.Type, v2.Type));

      return fn.Invoke(v1, v2);
    }
  }
}
