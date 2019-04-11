namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class ParenthesizedExpression : CExpression
  {
    private CExpression _expression;

    internal ParenthesizedExpression(Lexer lexer) : base(lexer)
    { 
      // ( EXPR )
      // ^

      if (lexer.TokenType != TokenEnum.BRACKETOPEN)
        throw new ParseException(lexer, TokenEnum.BRACKETOPEN);
      lexer.Next();

      _expression = new Expression(lexer).Get();

      if (lexer.TokenType != TokenEnum.BRACKETCLOSE)
        throw new ParseException(lexer, TokenEnum.BRACKETCLOSE);
      lexer.Next();
    }

    public override CExpression Get()
    {
      return _expression;
    }

    public override object Evaluate(Context context)
    {
      return _expression.Evaluate(context);
    }
  }
}