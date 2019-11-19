﻿using Primrose.Primitives.Extensions;
using System;
using System.Collections.Generic;

namespace Primrose.Expressions.Tree.Expressions
{
  internal class LogicalAndExpression : CExpression
  {
    private CExpression _first;
    private List<CExpression> _set = new List<CExpression>();

    internal LogicalAndExpression(ContextScope scope, Lexer lexer) : base(scope, lexer)
    {
      // EQUALEXPR && EQUALEXPR ...

      _first = new EqualityExpression(scope, lexer).Get();

      while (lexer.TokenType == TokenEnum.AMPAMP // &&
        )
      {
        TokenEnum _type = lexer.TokenType;
        lexer.Next(); //AMPAMP
        _set.Add(new EqualityExpression(scope, lexer).Get());
      }
    }

    public override CExpression Get()
    {
      if (_set.Count == 0)
        return _first;
      return this;
    }

    public override Val Evaluate(AContext context)
    {
      if (_set.Count == 0)
        return _first.Evaluate(context);

      Val result = _first.Evaluate(context);
      if (result.Type != ValType.BOOL) throw new EvalException(this, "Non-boolean value {0} found at start of conditional expression".F(result.Value));
      if ((bool)result)
      {
        foreach (CExpression _expr in _set)
        {
          Val adden = _expr.Evaluate(context);

          try { result = Ops.Do(BOp.LOGICAL_AND, result, adden); } catch (Exception ex) { throw new EvalException(this, "&&", result, adden, ex); }
          if (!(bool)result)
            break;
        }
      }
      return result;
    }
  }
}