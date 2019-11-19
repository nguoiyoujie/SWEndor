namespace Primrose.Expressions.Tree.Statements
{
  internal class CStatement : IStatement, ITracker
  {
    internal CStatement(ContextScope scope, Lexer lexer)
    {
      SourceName = lexer.SourceName;
      LineNumber = lexer.LineNumber;
      Position = lexer.Position;
    }

    public virtual CStatement Get() { return this; }
    public virtual void Evaluate(AContext context) { }

    public string SourceName { get; }
    public int LineNumber { get; }
    public int Position { get; }
  }

  internal interface IStatement
  {
    CStatement Get();
    void Evaluate(AContext context);
  }
}
