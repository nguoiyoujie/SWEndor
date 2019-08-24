using System;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class RelationalExpression : CExpression
  {
    private CExpression _first;
    private TokenEnum _type = TokenEnum.NOTHING;
    private CExpression _second;

    internal RelationalExpression(Lexer lexer) : base(lexer)
    {
      // ADDEXPR < ADDEXPR
      // ADDEXPR > ADDEXPR
      // ADDEXPR <= ADDEXPR
      // ADDEXPR <= ADDEXPR

      _first = new AddExpression(lexer).Get();

      _type = lexer.TokenType;
      if (_type == TokenEnum.LESSTHAN // <
        || _type == TokenEnum.GREATERTHAN // >
        || _type == TokenEnum.LESSEQUAL // <=
        || _type == TokenEnum.GREATEREQUAL // >=
        )
      {
        lexer.Next();
        _second = new AddExpression(lexer).Get();
      }
      else
      {
        _type = TokenEnum.NOTHING;
      }
    }

    public override CExpression Get()
    {
      if (_type == TokenEnum.NOTHING)
        return _first;
      return this;
    }

    public override object Evaluate(Context context)
    {
      dynamic v1 = _first.Evaluate(context);
      dynamic v2 = _second.Evaluate(context);

      switch (_type)
      {
        case TokenEnum.LESSTHAN:
          try { return v1 < v2; } catch (Exception ex) { throw new EvalException(this, "<", v1, v2, ex); }
        case TokenEnum.GREATERTHAN:
          try { return v1 > v2; } catch (Exception ex) { throw new EvalException(this, ">", v1, v2, ex); }
        case TokenEnum.LESSEQUAL:
          try { return v1 <= v2; } catch (Exception ex) { throw new EvalException(this, "<=", v1, v2, ex); }
        case TokenEnum.GREATEREQUAL:
          try { return v1 >= v2; } catch (Exception ex) { throw new EvalException(this, ">=", v1, v2, ex); }
        default:
          return _first.Evaluate(context);
      }
    }
  }
}