namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Statements
{
  public class CStatement : IStatement, ITracker
  {
    internal CStatement(Script local, Lexer lexer) { LineNumber = lexer.LineNumber; Position = lexer.Position; }
    public virtual CStatement Get() { return this; }
    public virtual void Evaluate(Script local, Context context) { }

    public int LineNumber { get; }
    public int Position { get; }
  }

  public interface IStatement
  {
    CStatement Get();
    void Evaluate(Script local, Context context);
  }
}
