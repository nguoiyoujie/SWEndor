namespace Primrose.Expressions.Tree.Expressions
{
  internal class Variable : CExpression
  {
    private ContextScope _scope;
    public string varName { get; protected set; }

    internal Variable(ContextScope scope, Lexer lexer, int skip) : base(scope, lexer) { }

    internal Variable(ContextScope scope, Lexer lexer) : base(scope, lexer)
    {
      varName = lexer.TokenContents;
      _scope = scope;
      lexer.Next(); // VARIABLE
    }

    public override Val Evaluate(AContext context)
    {
      return _scope.GetVar(this, varName);
    }
  }
}