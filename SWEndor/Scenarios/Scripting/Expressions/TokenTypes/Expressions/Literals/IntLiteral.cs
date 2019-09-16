using System;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions.Literals
{
  public class IntLiteral : CLiteral
  {
    private int _value;

    internal IntLiteral(Lexer lexer) : base(lexer)
    {
      _value = Convert.ToInt32(lexer.TokenContents);
      lexer.Next();
    }

    public override Val Evaluate(Context context)
    {
      return new Val(_value);
    }
  }
}
