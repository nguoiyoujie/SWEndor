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
      if (_question.Evaluate(context).Equals(true))
        return _true?.Evaluate(context);
      else
        return _false?.Evaluate(context);
    }
  }
}