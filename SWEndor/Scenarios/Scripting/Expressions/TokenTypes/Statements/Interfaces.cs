namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Statements
{
  public class CStatement : IStatement
  {
    internal CStatement(Lexer lexer) { }
    public virtual CStatement Get() { return this; }
    public virtual void Evaluate(Context context) { }
  }

  public interface IStatement
  {
    CStatement Get();
    void Evaluate(Context context);
  }
}
