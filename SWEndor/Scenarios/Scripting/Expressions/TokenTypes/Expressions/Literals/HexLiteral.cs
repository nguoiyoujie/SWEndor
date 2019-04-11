using System;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions.Literals
{
  public class HexLiteral : CLiteral
  {
    private long _value;

    internal HexLiteral(Lexer lexer) : base(lexer)
    {
      _value = Convert.ToInt64(lexer.TokenContents.Substring(2, lexer.TokenContents.Length - 2));
      lexer.Next();
    }

    public override object Evaluate(Context context)
    {
      return _value;
    }
  }
}
