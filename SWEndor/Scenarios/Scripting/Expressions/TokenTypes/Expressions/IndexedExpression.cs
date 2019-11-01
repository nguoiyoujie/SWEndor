using SWEndor.Primitives.Extensions;
using System;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class IndexedExpression : CExpression
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

    public override Val Evaluate(Script local, Context context)
    {
      Val c = _expression.Evaluate(local, context);
      if (_index == null)
        return c;

      Val i = _index.Evaluate(local, context);
      if (!(i.Type == ValType.INT || (i.Type == ValType.FLOAT && i.vF == i.vI)))
        throw new EvalException(this, "Attempted to index an array with a non-integer!");

      int x = i.vI;
      try
      {
        switch (c.Type)
        {
          case ValType.BOOL_ARRAY:
            return new Val(c.aB[x]);
          case ValType.INT_ARRAY:
            return new Val(c.aI[x]);
          case ValType.FLOAT2:
          case ValType.FLOAT3:
          case ValType.FLOAT4:
          case ValType.FLOAT_ARRAY:
            return new Val(c.aF[x]);
          case ValType.STRING:
            return new Val(c.vS[x]);

          default:
            throw new EvalException(this, "Attempted to index a non-array: {0}".F(c));
        }
      }
      catch (IndexOutOfRangeException ex)
      {
        int len = 0;
        switch (c.Type)
        {
          case ValType.BOOL_ARRAY:
            len = c.aB.Length;
            break;
          case ValType.INT_ARRAY:
            len = c.aI.Length;
            break;
          case ValType.FLOAT2:
          case ValType.FLOAT3:
          case ValType.FLOAT4:
          case ValType.FLOAT_ARRAY:
            len = c.aF.Length;
            break;
          case ValType.STRING:
            len = c.vS.Length;
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
