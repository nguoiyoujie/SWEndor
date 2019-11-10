namespace Primrose.Expressions.Tree.Expressions.Literals
{
  internal class StringLiteral : CLiteral
  {
    private string _value;

    internal StringLiteral(Script local, Lexer lexer) : base(local, lexer)
    {
      _value = lexer.TokenContents.Substring(1, lexer.TokenContents.Length - 2); // strip quotes
      lexer.Next(); //STRING
    }

    public override Val Evaluate(Script local, AContext context)
    {
      return new Val(_value);
    }
  }
}