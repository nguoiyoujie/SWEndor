﻿using System;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class UnaryExpression : CExpression
  {
    private CExpression _primary;
    private TokenEnum _type;

    internal UnaryExpression(Lexer lexer) : base(lexer)
    {
      // + PRIAMRY
      // - PRIAMRY
      // ! PRIAMRY
      // ~ PRIAMRY // not supported yet
      // ^

      _type = lexer.TokenType;
      if (_type == TokenEnum.PLUS // +
        || _type == TokenEnum.MINUS // -
        || _type == TokenEnum.NOT // !
        )
      {
        lexer.Next();
        _primary = new PrimaryExpression(lexer).Get();
      }
      else
      {
        _primary = new PrimaryExpression(lexer).Get();
        _type = TokenEnum.NOTHING;
      }
    }

    public override CExpression Get()
    {
      if (_type == TokenEnum.NOTHING)
        return _primary;
      return this;
    }

    public override Val Evaluate(Context context)
    {
      Val result = _primary.Evaluate(context);
      switch (_type)
      {
        default:
        case TokenEnum.PLUS:
        case TokenEnum.NOTHING:
          return result;
        case TokenEnum.MINUS:
          try { return Ops.Do(UOp.NEGATION, result); } catch (Exception ex) { throw new EvalException(this, "-", result, ex); }
        case TokenEnum.NOT:
          try { return Ops.Do(UOp.LOGICAL_NOT, result); } catch (Exception ex) { throw new EvalException(this, "!", result, ex); }
      }
    }
  }
}