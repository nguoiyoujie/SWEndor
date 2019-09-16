using System;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions.Literals
{
  public class SingleLiteral : CLiteral
  {
    private float _value;

    internal SingleLiteral(Lexer lexer) : base(lexer)
    {
      _value = Convert.ToSingle(lexer.TokenContents);
      lexer.Next();
    }

    public override Val Evaluate(Context context)
    {
      return new Val(_value);
    }
  }
}