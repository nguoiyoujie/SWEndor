using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Statements
{
  public class RootStatement : CStatement
  {
    public List<CStatement> Statements = new List<CStatement>();

    internal RootStatement(Script local, Lexer lexer) : base(local, lexer)
    {
      // STATEMENT STATEMENT ...

      while (!lexer.EndOfStream)
      {
        Statements.Add(new Statement(local, lexer).Get());
      }
    }

    public override void Evaluate(Script local, Context context)
    {
      foreach (Statement s in Statements)
      {
        s.Evaluate(local, context);
      }
    }
  }
}
