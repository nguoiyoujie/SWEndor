using SWEndor.Primitives.Extensions;
using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class LogicalOrExpression : CExpression
  {
    private CExpression _first;
    private List<CExpression> _set = new List<CExpression>();

    internal LogicalOrExpression(Script local, Lexer lexer) : base(local, lexer)
    {
      // ANDEXPR || ANDEXPR ...

      _first = new LogicalAndExpression(local, lexer).Get();

      while (lexer.TokenType == TokenEnum.PIPEPIPE // ||
        )
      {
        TokenEnum _type = lexer.TokenType;
        lexer.Next(); //PIPEPIPE
        _set.Add(new LogicalAndExpression(local, lexer).Get());
      }
    }

    public override CExpression Get()
    {
      if (_set.Count == 0)
        return _first;
      return this;
    }

    public override Val Evaluate(Script local, Context context)
    {
      if (_set.Count == 0)
        return _first.Evaluate(local, context);

      Val result = _first.Evaluate(local, context);
      if (result.Type != ValType.BOOL) throw new EvalException(this, TextLocalization.Get(TextLocalKeys.SCRIPT_UNEXPECTED_NONBOOL).F(result.Value));
      if (!(bool)result)
      {
        foreach (CExpression _expr in _set)
        {
          Val adden = _expr.Evaluate(local, context);

          try { result = Ops.Do(BOp.LOGICAL_OR, result, adden); } catch (Exception ex) { throw new EvalException(this, "||", result, adden, ex); }
          if ((bool)result)
            break;
        }
      }
      return result;
    }
  }
}