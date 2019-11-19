namespace Primrose.Expressions.Tree.Expressions
{
  internal class Expression : CExpression
  {
    private CExpression _expr;

    internal Expression(ContextScope scope, Lexer lexer) : base(scope, lexer)
    {
      _expr = new TernaryExpression(scope, lexer).Get();
    }

    public override CExpression Get()
    {
      return _expr;
    }

    public override Val Evaluate(AContext context)
    {
      return _expr.Evaluate(context);
    }
  }

}
