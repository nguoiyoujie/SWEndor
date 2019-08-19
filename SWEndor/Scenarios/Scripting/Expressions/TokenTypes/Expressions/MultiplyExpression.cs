using SWEndor.Primitives;
using System;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class MultiplyExpression : CExpression
  {
    private CExpression _first;
    private ThreadSafeDictionary<CExpression, TokenEnum> _set = new ThreadSafeDictionary<CExpression, TokenEnum>();

    internal MultiplyExpression(Lexer lexer) : base(lexer)
    {
      // UNARYEXPR * UNARYEXPR ...
      // UNARYEXPR / UNARYEXPR ...
      // UNARYEXPR % UNARYEXPR ...

      _first = new UnaryExpression(lexer).Get();

      while (lexer.TokenType == TokenEnum.ASTERISK // *
        || lexer.TokenType == TokenEnum.SLASH // /
        || lexer.TokenType == TokenEnum.PERCENT // %
        )
      {
        TokenEnum _type = lexer.TokenType;
        lexer.Next();
        _set.Add(new UnaryExpression(lexer).Get(), _type);
      }
    }

    public override CExpression Get()
    {
      if (_set.Count == 0)
        return _first;
      return this;
    }

    public override object Evaluate(Context context)
    {
      dynamic result = _first.Evaluate(context);
      foreach (CExpression _expr in _set.Keys)
      {
        dynamic adden = _expr.Evaluate(context);
        if (adden != null)
        {
          switch (_set[_expr])
          {
            case TokenEnum.ASTERISK:
              try { result *= adden; } catch (Exception ex) { throw new EvalException("*", result, adden, ex); }
              break;
            case TokenEnum.SLASH:
              try { result /= adden; } catch (Exception ex) { throw new EvalException("/", result, adden, ex); }
              break;
            case TokenEnum.PERCENT:
              try { result %= adden; } catch (Exception ex) { throw new EvalException("%", result, adden, ex); }
              break;
          }
        }
      }

      return result;
    }
  }
}