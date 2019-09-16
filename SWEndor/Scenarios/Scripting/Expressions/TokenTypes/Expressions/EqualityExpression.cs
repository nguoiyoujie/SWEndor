using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class EqualityExpression : CExpression
  {
    private bool isUnequal = false;
    private CExpression _first;
    private List<CExpression> _set = new List<CExpression>();

    internal EqualityExpression(Lexer lexer) : base(lexer)
    {
      // RELATEEXPR == RELATEEXPR ...
      // RELATEEXPR != RELATEEXPR ...

      _first = new RelationalExpression(lexer).Get();

      TokenEnum _type = lexer.TokenType;
      if (_type == TokenEnum.EQUAL // ==
        )
      {
        lexer.Next();
        _set.Add(new RelationalExpression(lexer).Get());
      }
      else if (_type == TokenEnum.NOTEQUAL // !=
      )
      {
        lexer.Next();
        isUnequal = true;
        _set.Add(new RelationalExpression(lexer).Get());
      }
    }

    public override CExpression Get()
    {
      if (_set.Count == 0)
        return _first;
      return this;
    }

    public override Val Evaluate(Context context)
    {
      Val result = _first.Evaluate(context);
      foreach (CExpression _expr in _set)
      {
        Val adden = _expr.Evaluate(context);
        if (isUnequal)
          try { result = Ops.Do(BOp.NOT_EQUAL_TO, result, adden); } catch (Exception ex) { throw new EvalException(this, "!=", result, adden, ex); }
        else
          try { result = Ops.Do(BOp.EQUAL_TO, result, adden); } catch (Exception ex) { throw new EvalException(this, "==", result, adden, ex); }
      }
      return result;
    }
  }
}