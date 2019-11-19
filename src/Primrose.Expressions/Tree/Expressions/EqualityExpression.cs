using System;
using System.Collections.Generic;

namespace Primrose.Expressions.Tree.Expressions
{
  internal class EqualityExpression : CExpression
  {
    private bool isUnequal = false;
    private CExpression _first;
    private List<CExpression> _set = new List<CExpression>();

    internal EqualityExpression(ContextScope scope, Lexer lexer) : base(scope, lexer)
    {
      // RELATEEXPR == RELATEEXPR ...
      // RELATEEXPR != RELATEEXPR ...

      _first = new RelationalExpression(scope, lexer).Get();

      TokenEnum _type = lexer.TokenType;
      if (_type == TokenEnum.EQUAL // ==
        )
      {
        lexer.Next(); //EQUAL
        _set.Add(new RelationalExpression(scope, lexer).Get());
      }
      else if (_type == TokenEnum.NOTEQUAL // !=
      )
      {
        lexer.Next(); //NOTEQUAL
        isUnequal = true;
        _set.Add(new RelationalExpression(scope, lexer).Get());
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
      Val result = _first.Evaluate(context);
      foreach (CExpression _expr in _set)
      {
        Val adden = _expr.Evaluate(context);
        if (isUnequal)
          try { result = Ops.Do(BOp.NOT_EQUAL_TO, result, adden); } catch (Exception ex) { throw new EvalException(this, "!=", result, adden, ex); }
        else
          try { result = Ops.Do(BOp.EQUAL_TO, result, adden); } catch (Exception ex) { throw new EvalException(this, "==", result, adden, ex); }
      }
      return result;
    }
  }
}