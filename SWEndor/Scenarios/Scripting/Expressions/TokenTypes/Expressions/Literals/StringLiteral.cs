namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions.Literals
{
  public class StringLiteral : CLiteral
  {
    private string _value;

    internal StringLiteral(Lexer lexer) : base(lexer)
    {
      _value = lexer.TokenContents.Substring(1, lexer.TokenContents.Length - 2); // strip quotes
      lexer.Next();
    }

    public override Val Evaluate(Context context)
    {
      return new Val(_value);
    }
  }
}