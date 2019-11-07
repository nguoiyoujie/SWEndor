namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class ParenthesizedExpression : CExpression
  {
    private CExpression _expression;

    internal ParenthesizedExpression(Script local, Lexer lexer) : base(local, lexer)
    {
      // ( TERN_EXPR )
      // ^

      bool parans = false;
      if (lexer.TokenType == TokenEnum.BRACKETOPEN)
      {
        parans = true;
        lexer.Next(); // BRACKETOPEN
      }

      _expression = new Expression(local, lexer).Get();

      if (parans)
      {
        if (lexer.TokenType != TokenEnum.BRACKETCLOSE)
          throw new ParseException(lexer, TokenEnum.BRACKETCLOSE);
        else
          lexer.Next(); // BRACKETCLOSE
      }
    }

    public override CExpression Get()
    {
      return _expression;
    }

    public override Val Evaluate(Script local, Context context)
    {
      return _expression.Evaluate(local, context);
    }
  }
}