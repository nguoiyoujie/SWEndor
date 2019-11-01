namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class Variable : CExpression
  {
    public string varName { get; protected set; }

    internal Variable(Script local, Lexer lexer, int skip) : base(local, lexer) { }

    internal Variable(Script local, Lexer lexer) : base(local, lexer)
    {
      varName = lexer.TokenContents;
      lexer.Next(); // VARIABLE
    }

    public override Val Evaluate(Script local, Context context)
    {
      return local.GetVar(varName);
    }
  }
}