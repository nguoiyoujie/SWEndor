using SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Statements
{
  public class Statement : CStatement
  {
    private CStatement _statement;

    internal Statement(Lexer lexer) : base(lexer)
    {
      // IFTHENELSE
      // dont grab semicolons here becuase IfThenElse is multiple statements

      if (lexer.TokenType != TokenEnum.NOTHING && lexer.TokenType != TokenEnum.COMMENT)
      {
        _statement = new ForEachStatement(lexer).Get();

        // comment (eliminated by lexer)
        //if (lexer.TokenType == TokenEnum.COMMENT)
        //  lexer.Next();
      }
    }

    public override void Evaluate(Context context)
    {
      _statement.Evaluate(context);
    }
  }
}
