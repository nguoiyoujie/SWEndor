namespace Primrose.Expressions.Tree.Expressions.Literals
{
  internal class NullLiteral : CLiteral
  {
    internal NullLiteral(ContextScope scope, Lexer lexer) : base(scope, lexer)
    {
      lexer.Next(); //NULL
    }

    public override Val Evaluate(AContext context)
    {
      return Val.NULL;
    }
  }
}
