namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class UnaryExpression : CExpression
  {
    private CExpression _primary;
    private TokenEnum _type;

    internal UnaryExpression(Lexer lexer) : base(lexer)
    {
      // + PRIAMRY
      // - PRIAMRY
      // ! PRIAMRY
      // ~ PRIAMRY // not supported yet
      // ^

      _type = lexer.TokenType;
      if (_type == TokenEnum.PLUS // +
        || _type == TokenEnum.MINUS // -
        || _type == TokenEnum.NOT // !
        )
      {
        lexer.Next();
        _primary = new PrimaryExpression(lexer).Get();
      }
      else
      {
        _primary = new PrimaryExpression(lexer).Get();
        _type = TokenEnum.NOTHING;
      }
    }

    public override CExpression Get()
    {
      if (_type == TokenEnum.NOTHING)
        return _primary;
      return this;
    }

    public override object Evaluate(Context context)
    {
      switch (_type)
      {
        default:
        case TokenEnum.PLUS:
        case TokenEnum.NOTHING:
          return _primary.Evaluate(context);
        case TokenEnum.MINUS:
          return -(dynamic)_primary.Evaluate(context);
        case TokenEnum.NOT:
          return !(dynamic)_primary.Evaluate(context);
      }
    }
  }
}