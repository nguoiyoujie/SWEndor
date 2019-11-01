using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class MultiplyExpression : CExpression
  {
    private CExpression _first;
    private Dictionary<CExpression, TokenEnum> _set = new Dictionary<CExpression, TokenEnum>();

    internal MultiplyExpression(Script local, Lexer lexer) : base(local, lexer)
    {
      // UNARYEXPR * UNARYEXPR ...
      // UNARYEXPR / UNARYEXPR ...
      // UNARYEXPR % UNARYEXPR ...

      _first = new UnaryExpression(local, lexer).Get();

      while (lexer.TokenType == TokenEnum.ASTERISK // *
        || lexer.TokenType == TokenEnum.SLASH // /
        || lexer.TokenType == TokenEnum.PERCENT // %
        )
      {
        TokenEnum _type = lexer.TokenType;
        lexer.Next(); //ASTERISK / SLASH / PERCENT
        _set.Add(new UnaryExpression(local, lexer).Get(), _type);
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
      Val result = _first.Evaluate(local, context);
      foreach (CExpression _expr in _set.Keys)
      {
        Val adden = _expr.Evaluate(local, context);

        switch (_set[_expr])
        {
          case TokenEnum.ASTERISK:
            try { result = Ops.Do(BOp.MULTIPLY, result, adden); } catch (Exception ex) { throw new EvalException(this, "*", result, adden, ex); }
            break;
          case TokenEnum.SLASH:
            try { result = Ops.Do(BOp.DIVIDE, result, adden); } catch (Exception ex) { throw new EvalException(this, "/", result, adden, ex); }
            break;
          case TokenEnum.PERCENT:
            try { result = Ops.Do(BOp.MODULUS, result, adden); } catch (Exception ex) { throw new EvalException(this, "%", result, adden, ex); }
            break;
        }
      }

      return result;
    }
  }
}