using Primrose.Primitives.Extensions;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions.Literals
{
  public class BoolLiteral : CLiteral
  {
    // true or false
    private bool _value;

    internal BoolLiteral(Script local, Lexer lexer) : base(local, lexer)
    {
      _value = lexer.TokenContents.ToBool();
      lexer.Next(); //BOOL
    }

    public override Val Evaluate(Script local, Context context)
    {
      return new Val(_value);
    }
  }
}
