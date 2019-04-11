using System;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions.Literals
{
  public class DoubleLiteral : CLiteral
  {
    private double _value;

    internal DoubleLiteral(Lexer lexer) : base(lexer)
    {
      _value = Convert.ToDouble(lexer.TokenContents);
      lexer.Next();
    }

    public override object Evaluate(Context context)
    {
      return _value;
    }
  }
}