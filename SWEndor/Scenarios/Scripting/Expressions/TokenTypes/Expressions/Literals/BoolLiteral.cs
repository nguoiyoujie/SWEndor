namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions.Literals
{
  public class BoolLiteral : CLiteral
  {
    // true or false
    private bool _value;

    internal BoolLiteral(Lexer lexer) : base(lexer)
    {
      _value = lexer.TokenContents.ToBool();
      lexer.Next();
    }

    public override object Evaluate(Context context)
    {
      return _value;
    }
  }
}
