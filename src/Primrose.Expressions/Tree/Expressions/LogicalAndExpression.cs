using Primrose.Primitives.Extensions;
using System;
using System.Collections.Generic;

namespace Primrose.Expressions.Tree.Expressions
{
  public class LogicalAndExpression : CExpression
  {
    private CExpression _first;
    private List<CExpression> _set = new List<CExpression>();

    internal LogicalAndExpression(Script local, Lexer lexer) : base(local, lexer)
    {
      // EQUALEXPR && EQUALEXPR ...

      _first = new EqualityExpression(local, lexer).Get();

      while (lexer.TokenType == TokenEnum.AMPAMP // &&
        )
      {
        TokenEnum _type = lexer.TokenType;
        lexer.Next(); //AMPAMP
        _set.Add(new EqualityExpression(local, lexer).Get());
      }
    }

    public override CExpression Get()
    {
      if (_set.Count == 0)
        return _first;
      return this;
    }

    public override Val Evaluate(Script local, AContext context)
    {
      if (_set.Count == 0)
        return _first.Evaluate(local, context);

      Val result = _first.Evaluate(local, context);
      if (result.Type != ValType.BOOL) throw new EvalException(this, "Non-boolean value {0} found at start of conditional expression".F(result.Value));
      if ((bool)result)
      {
        foreach (CExpression _expr in _set)
        {
          Val adden = _expr.Evaluate(local, context);

          try { result = Ops.Do(BOp.LOGICAL_AND, result, adden); } catch (Exception ex) { throw new EvalException(this, "&&", result, adden, ex); }
          if (!(bool)result)
            break;
        }
      }
      return result;
    }
  }
}