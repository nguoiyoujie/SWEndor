namespace Primrose.Expressions.Tree.Statements
{
  internal class SingleStatement : CStatement
  {
    private CStatement _statement;

    internal SingleStatement(ContextScope scope, Lexer lexer) : base(scope, lexer)
    {
      _statement = new AssignmentStatement(scope, lexer).Get();

      if (lexer.TokenType == TokenEnum.SEMICOLON)
        lexer.Next(); // SEMICOLON
      else
        throw new ParseException(lexer, TokenEnum.SEMICOLON);
    }

    public override CStatement Get()
    {
      return _statement;
    }

    public override void Evaluate(AContext context)
    {
      _statement.Evaluate(context);
    }
  }
}
