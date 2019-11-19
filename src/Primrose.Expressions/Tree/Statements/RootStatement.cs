using System.Collections.Generic;

namespace Primrose.Expressions.Tree.Statements
{
  internal class RootStatement : CStatement
  {
    public List<CStatement> Statements = new List<CStatement>();

    internal RootStatement(ContextScope scope, Lexer lexer) : base(scope, lexer)
    {
      // STATEMENT STATEMENT ...

      while (!lexer.EndOfStream)
      {
        Statements.Add(new Statement(scope, lexer).Get());
      }
    }

    public override void Evaluate(AContext context)
    {
      foreach (Statement s in Statements)
      {
        s.Evaluate(context);
      }
    }
  }
}
