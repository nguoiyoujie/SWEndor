using SWEndor.Primitives;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Statements
{
  public class RootStatement : CStatement
  {
    public ThreadSafeList<CStatement> Statements = new ThreadSafeList<CStatement>();

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
      foreach (Statement s in Statements.GetList())
      {
        s.Evaluate(context);
      }
    }
  }
}
