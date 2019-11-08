using System;

namespace Primrose.Expressions.Tree.Expressions
{
  public class RelationalExpression : CExpression
  {
    private CExpression _first;
    private TokenEnum _type = TokenEnum.NOTHING;
    private CExpression _second;

    internal RelationalExpression(Script local, Lexer lexer) : base(local, lexer)
    {
      // ADDEXPR < ADDEXPR
      // ADDEXPR > ADDEXPR
      // ADDEXPR <= ADDEXPR
      // ADDEXPR <= ADDEXPR

      _first = new AddExpression(local, lexer).Get();

      _type = lexer.TokenType;
      if (_type == TokenEnum.LESSTHAN // <
        || _type == TokenEnum.GREATERTHAN // >
        || _type == TokenEnum.LESSEQUAL // <=
        || _type == TokenEnum.GREATEREQUAL // >=
        )
      {
        lexer.Next(); //LESSTHAN / GREATERTHAN / LESSEQUAL / GREATEREQUAL
        _second = new AddExpression(local, lexer).Get();
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

    public override Val Evaluate(Script local, AContext context)
    {
      Val v1 = _first.Evaluate(local, context);
      Val v2 = _second.Evaluate(local, context);

      switch (_type)
      {
        case TokenEnum.LESSTHAN:
          try { return Ops.Do(BOp.LESS_THAN, v1, v2); } catch (Exception ex) { throw new EvalException(this, "<", v1, v2, ex); }
        case TokenEnum.GREATERTHAN:
          try { return Ops.Do(BOp.MORE_THAN, v1, v2); } catch (Exception ex) { throw new EvalException(this, ">", v1, v2, ex); }
        case TokenEnum.LESSEQUAL:
          try { return Ops.Do(BOp.LESS_THAN_OR_EQUAL_TO, v1, v2); } catch (Exception ex) { throw new EvalException(this, "<=", v1, v2, ex); }
        case TokenEnum.GREATEREQUAL:
          try { return Ops.Do(BOp.MORE_THAN_OR_EQUAL_TO, v1, v2); } catch (Exception ex) { throw new EvalException(this, ">=", v1, v2, ex); }
        default:
          return v1;
      }
    }
  }
}