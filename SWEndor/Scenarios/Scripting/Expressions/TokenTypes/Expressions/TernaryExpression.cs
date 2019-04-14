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

    public override object Evaluate(Context context)
    {
      dynamic result = false;
      try { result = (bool)(_question.Evaluate(context) as IConvertible); } catch (Exception ex) { throw new EvalException("bool cast", result, ex); }
      if (result)
        return _true?.Evaluate(context);
      else
        return _false?.Evaluate(context);
    }
  }
}