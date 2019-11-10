using Primrose.Primitives.Extensions;

namespace Primrose.Expressions.Tree.Expressions.Literals
{
  internal class BoolLiteral : CLiteral
  {
    // true or false
    private bool _value;

    internal BoolLiteral(Script local, Lexer lexer) : base(local, lexer)
    {
      _value = lexer.TokenContents.ToBool();
      lexer.Next(); //BOOL
    }

    public override Val Evaluate(Script local, AContext context)
    {
      return new Val(_value);
    }
  }
}
