namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class CLiteral : CExpression
  {
    internal CLiteral(Script local, Lexer lexer) : base(local, lexer) { }
  }

  public class CExpression : IExpression, ITracker
  {
    internal CExpression(Script local, Lexer lexer) { LineNumber = lexer.LineNumber; Position = lexer.Position; }
    public virtual CExpression Get() { return this; }
    public virtual Val Evaluate(Script local, Context context) { return default(Val); }

    public int LineNumber { get; }
    public int Position { get; }
  }

  public interface IExpression
  {
    CExpression Get();
    Val Evaluate(Script local, Context context);
  }
}
