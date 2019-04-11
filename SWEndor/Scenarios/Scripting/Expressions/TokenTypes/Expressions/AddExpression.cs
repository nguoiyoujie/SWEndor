using SWEndor.Primitives;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class AddExpression : CExpression
  {
    private CExpression _first;
    private ThreadSafeDictionary<CExpression, TokenEnum> _set = new ThreadSafeDictionary<CExpression, TokenEnum>();

    internal AddExpression(Lexer lexer) : base(lexer)
    {
      // MULTIEXPR + MULTIEXPR ...
      // MULTIEXPR - MULTIEXPR ...

      _first = new MultiplyExpression(lexer).Get();

      while (lexer.TokenType == TokenEnum.PLUS // +
        || lexer.TokenType == TokenEnum.MINUS // -
        )
      {
        TokenEnum _type = lexer.TokenType;
        lexer.Next();
        _set.Add(new MultiplyExpression(lexer).Get(), _type);
      }
    }

    public override CExpression Get()
    {
      if (_set.Count == 0)
        return _first;
      return this;
    }

    public override object Evaluate(Context context)
    {
      dynamic result = _first.Evaluate(context);
      foreach (CExpression _expr in _set.GetKeys())
      {
        switch (_set[_expr])
        {
          case TokenEnum.PLUS:
            result += (dynamic)_expr.Evaluate(context);
            break;
          case TokenEnum.MINUS:
            result -= (dynamic)_expr.Evaluate(context);
            break;
        }
      }
      return result;
    }
  }
}