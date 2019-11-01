using System;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions.Literals
{
  public class SingleLiteral : CLiteral
  {
    private float _value;

    internal SingleLiteral(Script local, Lexer lexer) : base(local, lexer)
    {
      _value = Convert.ToSingle(lexer.TokenContents);
      lexer.Next(); //FLOAT
    }

    public override Val Evaluate(Script local, Context context)
    {
      return new Val(_value);
    }
  }
}