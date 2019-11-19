using System;

namespace Primrose.Expressions.Tree.Expressions.Literals
{
  internal class HexLiteral : CLiteral
  {
    private int _value;

    internal HexLiteral(ContextScope scope, Lexer lexer) : base(scope, lexer)
    {
      _value = Convert.ToInt32(lexer.TokenContents.Substring(2, lexer.TokenContents.Length - 2));
      lexer.Next(); //HEXINT
    }

    public override Val Evaluate(AContext context)
    {
      return new Val(_value);
    }
  }
}
