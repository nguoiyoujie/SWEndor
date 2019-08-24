namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class CLiteral : CExpression
  {
    internal CLiteral(Lexer lexer) : base(lexer) { }
  }

  public class CExpression : IExpression, ITracker
  {
    internal CExpression(Lexer lexer) { LineNumber = lexer.LineNumber; Position = lexer.Position; }
    public virtual CExpression Get() { return this; }
    public virtual object Evaluate(Context context) { return null; }

    public int LineNumber { get; }
    public int Position { get; }
  }

  public interface IExpression
  {
    CExpression Get();
    object Evaluate(Context context);
  }
}
