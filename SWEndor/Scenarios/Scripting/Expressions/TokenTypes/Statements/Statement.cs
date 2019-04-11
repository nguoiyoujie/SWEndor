using SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Statements
{
  public class Statement : CStatement
  {
    private CStatement _statement;

    internal Statement(Lexer lexer) : base(lexer)
    {
      _statement = new AssignmentStatement(lexer).Get();

      // comment
      if (lexer.TokenType == TokenEnum.COMMENT)
        lexer.Next();
    }

    public override void Evaluate(Context context)
    {
      _statement.Evaluate(context);
    }
  }
}
