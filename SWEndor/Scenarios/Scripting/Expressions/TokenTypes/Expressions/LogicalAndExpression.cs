using SWEndor.Primitives;
using System;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class LogicalAndExpression : CExpression
  {
    private CExpression _first;
    private ThreadSafeList<CExpression> _set = new ThreadSafeList<CExpression>();

    internal LogicalAndExpression(Lexer lexer) : base(lexer)
    {
      // EQUALEXPR && EQUALEXPR ...

      _first = new EqualityExpression(lexer).Get();

      while (lexer.TokenType == TokenEnum.AMPAMP // &&
        )
      {
        TokenEnum _type = lexer.TokenType;
        lexer.Next();
        _set.Add(new EqualityExpression(lexer).Get());
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
      if (_set.Count == 0)
        return _first.Evaluate(context);

      dynamic result = _first.Evaluate(context);
      try { result = (bool)(result as IConvertible); } catch (Exception ex) { throw new EvalException("bool cast", result, ex); }
      if (result)
      {
        foreach (CExpression _expr in _set.GetList())
        {
          dynamic adden = (dynamic)_expr.Evaluate(context) ?? false;
          try { result &= adden; } catch (Exception ex) { throw new EvalException("&&", result, adden, ex); }
          if (!result)
            break;
        }
      }
      return result;
    }
  }
}