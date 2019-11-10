namespace Primrose.Expressions.Tree.Statements
{
  internal class SingleStatement : CStatement
  {
    private CStatement _statement;

    internal SingleStatement(Script local, Lexer lexer) : base(local, lexer)
    {
      _statement = new AssignmentStatement(local, lexer).Get();

      if (lexer.TokenType == TokenEnum.SEMICOLON)
        lexer.Next(); // SEMICOLON
      else
        throw new ParseException(lexer, TokenEnum.SEMICOLON);
    }

    public override CStatement Get()
    {
      return _statement;
    }

    public override void Evaluate(Script local, AContext context)
    {
      _statement.Evaluate(local, context);
    }
  }
}
