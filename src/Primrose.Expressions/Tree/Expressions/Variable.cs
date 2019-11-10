namespace Primrose.Expressions.Tree.Expressions
{
  internal class Variable : CExpression
  {
    public string varName { get; protected set; }

    internal Variable(Script local, Lexer lexer, int skip) : base(local, lexer) { }

    internal Variable(Script local, Lexer lexer) : base(local, lexer)
    {
      varName = lexer.TokenContents;
      lexer.Next(); // VARIABLE
    }

    public override Val Evaluate(Script local, AContext context)
    {
      return local.GetVar(varName);
    }
  }
}