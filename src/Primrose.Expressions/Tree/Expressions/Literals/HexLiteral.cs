using System;

namespace Primrose.Expressions.Tree.Expressions.Literals
{
  public class HexLiteral : CLiteral
  {
    private int _value;

    internal HexLiteral(Script local, Lexer lexer) : base(local, lexer)
    {
      _value = Convert.ToInt32(lexer.TokenContents.Substring(2, lexer.TokenContents.Length - 2));
      lexer.Next(); //HEXINT
    }

    public override Val Evaluate(Script local, AContext context)
    {
      return new Val(_value);
    }
  }
}
