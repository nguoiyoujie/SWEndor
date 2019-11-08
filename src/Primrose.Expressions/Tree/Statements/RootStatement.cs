using System.Collections.Generic;

namespace Primrose.Expressions.Tree.Statements
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

    public override void Evaluate(Script local, AContext context)
    {
      foreach (Statement s in Statements)
      {
        s.Evaluate(local, context);
      }
    }
  }
}
