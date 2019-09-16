namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class Expression : CExpression
  {
    private CExpression _expr;

    internal Expression(Lexer lexer) : base(lexer)
    {
      _expr = new TernaryExpression(lexer).Get();
    }

    public override CExpression Get()
    {
      return _expr;
    }

    public override Val Evaluate(Context context)
    {
      return _expr.Evaluate(context);
    }
  }

}
