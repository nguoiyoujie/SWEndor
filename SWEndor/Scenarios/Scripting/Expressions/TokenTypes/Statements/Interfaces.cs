namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Statements
{
  public class CStatement : IStatement
  {
    internal CStatement(Lexer lexer) { LineNumber = lexer.LineNumber; Position = lexer.Position; }
    public virtual CStatement Get() { return this; }
    public virtual void Evaluate(Context context) { }

    public readonly int LineNumber;
    public readonly int Position;
  }

  public interface IStatement
  {
    CStatement Get();
    void Evaluate(Context context);
  }
}
