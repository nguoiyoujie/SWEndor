using System;

namespace Primrose.Expressions.Tree.Expressions.Literals
{
  public class IntLiteral : CLiteral
  {
    private int _value;

    internal IntLiteral(Script local, Lexer lexer) : base(local, lexer)
    {
      _value = Convert.ToInt32(lexer.TokenContents);
      lexer.Next(); //INT
    }

    public override Val Evaluate(Script local, AContext context)
    {
      return new Val(_value);
    }
  }
}
