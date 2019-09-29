using SWEndor.Primitives;
using SWEndor.Primitives.Extensions;
using System;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class TernaryExpression : CExpression
  {
    private CExpression _question;
    private CExpression _true;
    private CExpression _false;

    internal TernaryExpression(Lexer lexer) : base(lexer)
    {
      // OREXPR ? EXPR : EXPR 

      _question = new LogicalOrExpression(lexer).Get();

      if (lexer.TokenType == TokenEnum.QUESTIONMARK)
      {
        lexer.Next();
        _true = new Expression(lexer).Get();

        if (lexer.TokenType == TokenEnum.COLON)
        {
          lexer.Next();
          _false = new Expression(lexer).Get();
        }
        else
        {
          throw new ParseException(lexer, TokenEnum.COLON);
        }
      }
    }

    public override CExpression Get()
    {
      if (_true == null)
        return _question;
      return this;
    }

    public override Val Evaluate(Context context)
    {
      Val result = _question.Evaluate(context);
      if (result.Type != ValType.BOOL) throw new EvalException(this, TextLocalization.Get(TextLocalKeys.SCRIPT_UNEXPECTED_NONBOOL).F(result.Value));
      if (result.ValueB)
        return _true?.Evaluate(context) ?? new Val();
      else
        return _false?.Evaluate(context) ?? new Val();
    }
  }
}