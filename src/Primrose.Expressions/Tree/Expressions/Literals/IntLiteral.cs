using System;

namespace Primrose.Expressions.Tree.Expressions.Literals
{
  internal class IntLiteral : CLiteral
  {
    private int _value;

    internal IntLiteral(ContextScope scope, Lexer lexer) : base(scope, lexer)
    {
      _value = Convert.ToInt32(lexer.TokenContents);
      lexer.Next(); //INT
    }

    public override Val Evaluate(AContext context)
    {
      return new Val(_value);
    }
  }
}
