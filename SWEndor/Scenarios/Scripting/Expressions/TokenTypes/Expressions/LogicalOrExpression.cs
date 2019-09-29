using SWEndor.Primitives.Extensions;
using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class LogicalOrExpression : CExpression
  {
    private CExpression _first;
    private List<CExpression> _set = new List<CExpression>();

    internal LogicalOrExpression(Lexer lexer) : base(lexer)
    {
      // ANDEXPR || ANDEXPR ...

      _first = new LogicalAndExpression(lexer).Get();

      while (lexer.TokenType == TokenEnum.PIPEPIPE // ||
        )
      {
        TokenEnum _type = lexer.TokenType;
        lexer.Next();
        _set.Add(new LogicalAndExpression(lexer).Get());
      }
    }

    public override CExpression Get()
    {
      if (_set.Count == 0)
        return _first;
      return this;
    }

    public override Val Evaluate(Context context)
    {
      if (_set.Count == 0)
        return _first.Evaluate(context);

      Val result = _first.Evaluate(context);
      if (result.Type != ValType.BOOL) throw new EvalException(this, TextLocalization.Get(TextLocalKeys.SCRIPT_UNEXPECTED_NONBOOL).F(result.Value));
      if (!result.ValueB)
      {
        foreach (CExpression _expr in _set)
        {
          Val adden = _expr.Evaluate(context);

          try { result = Ops.Do(BOp.LOGICAL_OR, result, adden); } catch (Exception ex) { throw new EvalException(this, "||", result, adden, ex); }
          if (result.ValueB)
            break;
        }
      }
      return result;
    }
  }
}