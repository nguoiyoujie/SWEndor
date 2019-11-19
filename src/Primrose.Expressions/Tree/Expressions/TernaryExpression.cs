using Primrose.Primitives.Extensions;

namespace Primrose.Expressions.Tree.Expressions
{
  internal class TernaryExpression : CExpression
  {
    private CExpression _question;
    private CExpression _true;
    private CExpression _false;

    internal TernaryExpression(ContextScope scope, Lexer lexer) : base(scope, lexer)
    {
      // OREXPR ? EXPR : EXPR 

      _question = new LogicalOrExpression(scope, lexer).Get();

      if (lexer.TokenType == TokenEnum.QUESTIONMARK)
      {
        lexer.Next(); // QUESTIONMARK
        _true = new Expression(scope, lexer).Get();

        if (lexer.TokenType == TokenEnum.COLON)
        {
          lexer.Next(); // COLON
          _false = new Expression(scope, lexer).Get();
        }
        else
        {
          throw new ParseException(lexer, TokenEnum.COLON);
        }
      }
    }

    public override CExpression Get()
    {
      if (_true == null)
        return _question;
      return this;
    }

    public override Val Evaluate(AContext context)
    {
      Val result = _question.Evaluate(context);
      if (result.Type != ValType.BOOL) throw new EvalException(this, "Non-boolean value {0} found at start of conditional expression".F(result.Value));
      if ((bool)result)
        return _true?.Evaluate(context) ?? new Val();
      else
        return _false?.Evaluate(context) ?? new Val();
    }
  }
}