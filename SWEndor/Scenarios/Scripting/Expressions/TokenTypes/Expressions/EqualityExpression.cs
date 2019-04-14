using SWEndor.Primitives;
using System;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class EqualityExpression : CExpression
  {
    private CExpression _first;
    private ThreadSafeList<CExpression> _set = new ThreadSafeList<CExpression>();

    internal EqualityExpression(Lexer lexer) : base(lexer)
    {
      // RELATEEXPR == RELATEEXPR ...

      _first = new RelationalExpression(lexer).Get();

      TokenEnum _type = lexer.TokenType;
      if (_type == TokenEnum.EQUAL // ==
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
        dynamic adden = _expr.Evaluate(context);
        try { result = _first.Evaluate(context) == adden; } catch (Exception ex) { throw new EvalException("==", result, adden, ex); }
      }
      return result;
    }
  }
}