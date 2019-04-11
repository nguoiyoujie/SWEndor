namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class RelationalExpression : CExpression
  {
    private CExpression _first;
    private TokenEnum _type = TokenEnum.NOTHING;
    private CExpression _second;

    internal RelationalExpression(Lexer lexer) : base(lexer)
    {
      // ADDEXPR < ADDEXPR
      // ADDEXPR > ADDEXPR
      // ADDEXPR <= ADDEXPR
      // ADDEXPR <= ADDEXPR

      _first = new AddExpression(lexer).Get();

      TokenEnum _type = lexer.TokenType;
      if (_type == TokenEnum.LESSTHAN // <
        || _type == TokenEnum.GREATERTHAN // >
        || _type == TokenEnum.LESSEQUAL // <=
        || _type == TokenEnum.GREATEREQUAL // >=
        )
      {
        lexer.Next();
        _second = new AddExpression(lexer).Get();
      }
      else
      {
        _type = TokenEnum.NOTHING;
      }
    }

    public override CExpression Get()
    {
      if (_type == TokenEnum.NOTHING)
        return _first;
      return this;
    }

    public override object Evaluate(Context context)
    {
      switch (_type)
      {
        case TokenEnum.LESSTHAN:
          return (dynamic)_first.Evaluate(context) < (dynamic)_second.Evaluate(context);
        case TokenEnum.GREATERTHAN:
          return (dynamic)_first.Evaluate(context) > (dynamic)_second.Evaluate(context);
        case TokenEnum.LESSEQUAL:
          return (dynamic)_first.Evaluate(context) <= (dynamic)_second.Evaluate(context);
        case TokenEnum.GREATEREQUAL:
          return (dynamic)_first.Evaluate(context) >= (dynamic)_second.Evaluate(context);
        default:
          return (dynamic)_first.Evaluate(context);
      }
    }
  }
}