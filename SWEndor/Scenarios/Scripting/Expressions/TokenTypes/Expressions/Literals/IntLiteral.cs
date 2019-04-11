using System;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions.Literals
{
  public class IntLiteral : CLiteral
  {
    private long _value;

    internal IntLiteral(Lexer lexer) : base(lexer)
    {
      _value = Convert.ToInt64(lexer.TokenContents);
      lexer.Next();
    }

    public override object Evaluate(Context context)
    {
      return _value;
    }
  }
}
