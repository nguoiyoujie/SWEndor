using System;

namespace Primrose.Expressions.Tree.Expressions.Literals
{
  internal class SingleLiteral : CLiteral
  {
    private float _value;

    internal SingleLiteral(ContextScope scope, Lexer lexer) : base(scope, lexer)
    {
      _value = Convert.ToSingle(lexer.TokenContents);
      lexer.Next(); //FLOAT
    }

    public override Val Evaluate(AContext context)
    {
      return new Val(_value);
    }
  }
}