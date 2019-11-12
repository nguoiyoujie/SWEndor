namespace Primrose.Expressions.Tree.Statements
{
  internal class CStatement : IStatement, ITracker
  {
    internal CStatement(Script local, Lexer lexer)
    {
      SourceName = lexer.SourceName;
      LineNumber = lexer.LineNumber;
      Position = lexer.Position;
    }

    public virtual CStatement Get() { return this; }
    public virtual void Evaluate(Script local, AContext context) { }

    public string SourceName { get; }
    public int LineNumber { get; }
    public int Position { get; }
  }

  internal interface IStatement
  {
    CStatement Get();
    void Evaluate(Script local, AContext context);
  }
}
