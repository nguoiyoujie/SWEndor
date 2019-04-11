using SWEndor.Primitives;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class EqualityExpression : CExpression
  {
    private CExpression _first;
    private ThreadSafeList<CExpression> _set = new ThreadSafeList<CExpression>();

    internal EqualityExpression(Lexer lexer) : base(lexer)
    {
      // RELATEEXPR || RELATEEXPR ...

      _first = new RelationalExpression(lexer).Get();

      TokenEnum _type = lexer.TokenType;
      while (_type == TokenEnum.EQUAL // ||
        )
      {
        lexer.Next();
        _set.Add(new RelationalExpression(lexer).Get());
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
        result = (dynamic)_first.Evaluate(context) && (dynamic)_expr.Evaluate(context);
      }
      return result;
    }
  }
}