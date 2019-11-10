using Primrose.Primitives.Extensions;

namespace Primrose.Expressions.Tree.Expressions
{
  internal class TernaryExpression : CExpression
  {
    private CExpression _question;
    private CExpression _true;
    private CExpression _false;

    internal TernaryExpression(Script local, Lexer lexer) : base(local, lexer)
    {
      // OREXPR ? EXPR : EXPR 

      _question = new LogicalOrExpression(local, lexer).Get();

      if (lexer.TokenType == TokenEnum.QUESTIONMARK)
      {
        lexer.Next(); // QUESTIONMARK
        _true = new Expression(local, lexer).Get();

        if (lexer.TokenType == TokenEnum.COLON)
        {
          lexer.Next(); // COLON
          _false = new Expression(local, lexer).Get();
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

    public override Val Evaluate(Script local, AContext context)
    {
      Val result = _question.Evaluate(local, context);
      if (result.Type != ValType.BOOL) throw new EvalException(this, "Non-boolean value {0} found at start of conditional expression".F(result.Value));
      if ((bool)result)
        return _true?.Evaluate(local, context) ?? new Val();
      else
        return _false?.Evaluate(local, context) ?? new Val();
    }
  }
}