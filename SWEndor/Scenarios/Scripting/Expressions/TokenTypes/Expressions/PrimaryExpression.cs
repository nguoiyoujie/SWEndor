using SWEndor.Primitives.Extensions;
using SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions.Literals;
using System;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class PrimaryExpression : CExpression
  {
    private CExpression _child;

    internal PrimaryExpression(Script local, Lexer lexer) : base(local, lexer)
    {
      TokenEnum t = lexer.TokenType;
      switch (t)
      {
        case TokenEnum.BRACKETOPEN:
          _child = new ParenthesizedExpression(local, lexer).Get();
          break;
        case TokenEnum.FUNCTION:
          _child = new Function(local, lexer).Get();
          break;

        case TokenEnum.VARIABLE:
          _child = new Variable(local, lexer).Get();
          break;

        //Literals
        case TokenEnum.BOOLEANLITERAL:
          _child = new BoolLiteral(local, lexer).Get();
          break;
        case TokenEnum.DECIMALINTEGERLITERAL:
          _child = new IntLiteral(local, lexer).Get();
          break;
        case TokenEnum.HEXINTEGERLITERAL:
          _child = new HexLiteral(local, lexer).Get();
          break;
        case TokenEnum.REALLITERAL:
          _child = new SingleLiteral(local, lexer).Get();
          break;
        case TokenEnum.STRINGLITERAL:
          _child = new StringLiteral(local, lexer).Get();
          break;
        case TokenEnum.BRACEOPEN:
          _child = new ArrayLiteral(local, lexer).Get();
          break;

        default:
          throw new ParseException(lexer);
      }
    }

    public override CExpression Get()
    {
      return _child;
    }

    public override Val Evaluate(Script local, Context context)
    {
      return _child.Evaluate(local, context);
    }
  }
}
