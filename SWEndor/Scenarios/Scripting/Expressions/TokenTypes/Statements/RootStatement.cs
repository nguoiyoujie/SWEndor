using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Statements
{
  public class RootStatement : CStatement
  {
    public List<CStatement> Statements = new List<CStatement>();

    internal RootStatement(Lexer lexer) : base(lexer)
    {
      // STATEMENT STATEMENT ...

      while (!lexer.EndOfStream)
      {
        Statements.Add(new Statement(lexer).Get());
      }
    }

    public override void Evaluate(Context context)
    {
      foreach (Statement s in Statements)
      {
        s.Evaluate(context);
      }
    }
  }
}
