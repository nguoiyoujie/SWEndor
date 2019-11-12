namespace Primrose.Expressions.Tree.Expressions
{
  internal class Expression : CExpression
  {
    private CExpression _expr;

    internal Expression(Script local, Lexer lexer) : base(local, lexer)
    {
      _expr = new TernaryExpression(local, lexer).Get();
    }

    public override CExpression Get()
    {
      return _expr;
    }

    public override Val Evaluate(Script local, AContext context)
    {
      return _expr.Evaluate(local, context);
    }
  }

}
