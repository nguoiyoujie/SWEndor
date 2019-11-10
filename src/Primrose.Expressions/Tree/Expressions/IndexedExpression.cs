using Primrose.Primitives.Extensions;
using Primrose.Primitives.ValueTypes;
using System;

namespace Primrose.Expressions.Tree.Expressions
{
  internal class IndexedExpression : CExpression
  {
    private CExpression _expression;
    private CExpression _index;

    internal IndexedExpression(Script local, Lexer lexer) : base(local, lexer)
    {
      // EXPR[EXPR]
      // ^

      _expression = new TernaryExpression(local, lexer).Get();

      // indexer
      if (lexer.TokenType == TokenEnum.SQBRACKETOPEN)
      {
        lexer.Next(); // SQBRACKETOPEN
        _index = new Expression(local, lexer).Get();

        if (lexer.TokenType != TokenEnum.SQBRACKETCLOSE)
          throw new ParseException(lexer, TokenEnum.SQBRACKETCLOSE);
        lexer.Next(); // SQBRACKETCLOSE
      }
    }

    public override CExpression Get()
    {
      return (_index == null) ? _expression : this;
    }

    public override Val Evaluate(Script local, AContext context)
    {
      Val c = _expression.Evaluate(local, context);
      if (_index == null)
        return c;

      Val i = _index.Evaluate(local, context);
      if (!(i.Type == ValType.INT || (i.Type == ValType.FLOAT && (float)i == (int)i)))
        throw new EvalException(this, "Attempted to index an array with a non-integer!");

      int x = (int)i;
      try
      {
        switch (c.Type)
        {
          case ValType.BOOL_ARRAY:
            return new Val(((bool[])c)[x]);
          case ValType.INT_ARRAY:
            return new Val(((int[])c)[x]);
          case ValType.FLOAT2:
            return new Val(((float2)c)[x]);
          case ValType.FLOAT3:
            return new Val(((float3)c)[x]);
          case ValType.FLOAT4:
            return new Val(((float4)c)[x]);
          case ValType.FLOAT_ARRAY:
            return new Val(((float[])c)[x]);
          case ValType.STRING:
            return new Val(((string)c)[x]);

          default:
            throw new EvalException(this, "Attempted to index a non-array: {0}".F(c));
        }
      }
      catch (IndexOutOfRangeException)
      {
        int len = 0;
        switch (c.Type)
        {
          case ValType.BOOL_ARRAY:
            len = ((bool[])c).Length;
            break;
          case ValType.INT_ARRAY:
            len = ((int[])c).Length;
            break;
          case ValType.FLOAT2:
            len = 2;
            break;
          case ValType.FLOAT3:
            len = 3;
            break;
          case ValType.FLOAT4:
            len = 4;
            break;
          case ValType.FLOAT_ARRAY:
            len = ((float[])c).Length;
            break;
          case ValType.STRING:
            len = ((string)c).Length;
            break;
        }
        throw new EvalException(this, "Index ({0}) for an array (length: {1}) is out of range!".F(x, len));
      }
      catch (Exception ex)
      {
        throw new EvalException(this, ex.Message);
      }
    }
  }
}
