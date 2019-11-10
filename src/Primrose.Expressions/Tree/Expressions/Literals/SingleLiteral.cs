using System;

namespace Primrose.Expressions.Tree.Expressions.Literals
{
  internal class SingleLiteral : CLiteral
  {
    private float _value;

    internal SingleLiteral(Script local, Lexer lexer) : base(local, lexer)
    {
      _value = Convert.ToSingle(lexer.TokenContents);
      lexer.Next(); //FLOAT
    }

    public override Val Evaluate(Script local, AContext context)
    {
      return new Val(_value);
    }
  }
}