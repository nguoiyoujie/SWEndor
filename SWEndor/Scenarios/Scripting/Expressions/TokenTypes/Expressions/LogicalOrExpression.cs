using SWEndor.Primitives;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class LogicalOrExpression : CExpression
  {
    private CExpression _first;
    private ThreadSafeList<CExpression> _set = new ThreadSafeList<CExpression>();

    internal LogicalOrExpression(Lexer lexer) : base(lexer)
    {
      // ANDEXPR || ANDEXPR ...

      _first = new LogicalAndExpression(lexer).Get();

      while (lexer.TokenType == TokenEnum.PIPEPIPE // ||
        )
      {
        TokenEnum _type = lexer.TokenType;
        lexer.Next();
        _set.Add(new LogicalAndExpression(lexer).Get());
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
      foreach (CExpression _expr in _set.GetList())
      {
        result = result || (dynamic)_expr.Evaluate(context);
      }
      return result;
    }
  }
}