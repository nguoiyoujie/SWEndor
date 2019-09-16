using System;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions.Literals
{
  public class HexLiteral : CLiteral
  {
    private int _value;

    internal HexLiteral(Lexer lexer) : base(lexer)
    {
      _value = Convert.ToInt32(lexer.TokenContents.Substring(2, lexer.TokenContents.Length - 2));
      lexer.Next();
    }

    public override Val Evaluate(Context context)
    {
      return new Val(_value);
    }
  }
}
