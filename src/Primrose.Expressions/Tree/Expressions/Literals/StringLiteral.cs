namespace Primrose.Expressions.Tree.Expressions.Literals
{
  internal class StringLiteral : CLiteral
  {
    private string _value;

    internal StringLiteral(ContextScope scope, Lexer lexer) : base(scope, lexer)
    {
      _value = lexer.TokenContents.Substring(1, lexer.TokenContents.Length - 2); // strip quotes
      lexer.Next(); //STRING
    }

    public override Val Evaluate(AContext context)
    {
      return new Val(_value);
    }
  }
}