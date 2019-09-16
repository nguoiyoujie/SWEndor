using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class AddExpression : CExpression
  {
    private CExpression _first;
    private Dictionary<CExpression, TokenEnum> _set = new Dictionary<CExpression, TokenEnum>();

    internal AddExpression(Lexer lexer) : base(lexer)
    {
      // MULTIEXPR + MULTIEXPR ...
      // MULTIEXPR - MULTIEXPR ...

      _first = new MultiplyExpression(lexer).Get();

      while (lexer.TokenType == TokenEnum.PLUS // +
        || lexer.TokenType == TokenEnum.MINUS // -
        )
      {
        TokenEnum _type = lexer.TokenType;
        lexer.Next();
        _set.Add(new MultiplyExpression(lexer).Get(), _type);
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
      Val result = _first.Evaluate(context);
      foreach (CExpression _expr in _set.Keys)
      {
        Val adden = _expr.Evaluate(context);
        switch (_set[_expr])
        {
          case TokenEnum.PLUS:
            try { result = Ops.Do(BOp.ADD, result, adden); } catch (Exception ex) { throw new EvalException(this, "+", result, adden, ex); }
            break;
          case TokenEnum.MINUS:
            try { result = Ops.Do(BOp.SUBTRACT, result, adden); ; } catch (Exception ex) { throw new EvalException(this, "-", result, adden, ex); }
            break;
        }
      }
      return result;
    }
  }
}