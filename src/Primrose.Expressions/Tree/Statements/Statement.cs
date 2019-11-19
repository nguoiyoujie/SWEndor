
namespace Primrose.Expressions.Tree.Statements
{
  internal class Statement : CStatement
  {
    private CStatement _statement;

    internal Statement(ContextScope scope, Lexer lexer) : base(scope, lexer)
    {
      // IFTHENELSE

      if (lexer.TokenType != TokenEnum.NOTHING && lexer.TokenType != TokenEnum.COMMENT)
      {
        _statement = new WhileStatement(scope, lexer).Get();

        // comment (eliminated by lexer)
        //if (lexer.TokenType == TokenEnum.COMMENT)
        //  lexer.Next();
      }
    }

    public override void Evaluate(AContext context)
    {
      _statement.Evaluate(context);
    }
  }
}
