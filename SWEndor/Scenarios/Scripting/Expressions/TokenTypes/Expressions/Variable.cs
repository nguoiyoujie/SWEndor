namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class Variable : CExpression
  {
    public string varName { get; private set; }

    internal Variable(Lexer lexer) : base(lexer)
    {
      varName = lexer.TokenContents;
      lexer.Next();
    }

    public override object Evaluate(Context context)
    {
      return context.Variables.Get(varName)?.Value; // can be null.
    }
  }
}