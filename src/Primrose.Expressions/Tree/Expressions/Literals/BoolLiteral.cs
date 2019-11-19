using Primrose.Primitives.Extensions;

namespace Primrose.Expressions.Tree.Expressions.Literals
{
  internal class BoolLiteral : CLiteral
  {
    // true or false
    private bool _value;

    internal BoolLiteral(ContextScope scope, Lexer lexer) : base(scope, lexer)
    {
      _value = lexer.TokenContents.ToBool();
      lexer.Next(); //BOOL
    }

    public override Val Evaluate(AContext context)
    {
      return new Val(_value);
    }
  }
}
