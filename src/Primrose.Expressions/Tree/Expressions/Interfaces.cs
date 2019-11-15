namespace Primrose.Expressions.Tree.Expressions
{
  internal class CLiteral : CExpression
  {
    internal CLiteral(Script local, Lexer lexer) : base(local, lexer) { }
  }

  internal class CExpression : IExpression, ITracker
  {
    internal CExpression(Script local, Lexer lexer) { SourceName = lexer.SourceName; LineNumber = lexer.LineNumber; Position = lexer.Position; }
    public virtual CExpression Get() { return this; }
    public virtual Val Evaluate(Script local, AContext context) { return default(Val); }

    public string SourceName { get; }
    public int LineNumber { get; }
    public int Position { get; }
  }

  internal interface IExpression
  {
    CExpression Get();
    Val Evaluate(Script local, AContext context);
  }
}
