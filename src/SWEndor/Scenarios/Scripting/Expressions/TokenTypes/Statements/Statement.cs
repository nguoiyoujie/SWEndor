using SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Statements
{
  public class Statement : CStatement
  {
    private CStatement _statement;

    internal Statement(Script local, Lexer lexer) : base(local, lexer)
    {
      // IFTHENELSE

      if (lexer.TokenType != TokenEnum.NOTHING && lexer.TokenType != TokenEnum.COMMENT)
      {
        _statement = new WhileStatement(local, lexer).Get();

        // comment (eliminated by lexer)
        //if (lexer.TokenType == TokenEnum.COMMENT)
        //  lexer.Next();
      }
    }

    public override void Evaluate(Script local, Context context)
    {
      _statement.Evaluate(local, context);
    }
  }
}
