using SWEndor.Primitives.Extensions;
using SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions.Literals;
using System;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class PrimaryExpression : CExpression
  {
    private CExpression _child;

    internal PrimaryExpression(Lexer lexer) : base(lexer)
    {
      TokenEnum t = lexer.TokenType;
      switch (t)
      {
        case TokenEnum.BRACKETOPEN:
          _child = new ParenthesizedExpression(lexer).Get();
          break;
        case TokenEnum.FUNCTION:
          _child = new Function(lexer).Get();
          break;

        case TokenEnum.VARIABLE:
          _child = new Variable(lexer).Get();
          break;

        //Literals
        case TokenEnum.BOOLEANLITERAL:
          _child = new BoolLiteral(lexer).Get();
          break;
        case TokenEnum.DECIMALINTEGERLITERAL:
          _child = new IntLiteral(lexer).Get();
          break;
        case TokenEnum.HEXINTEGERLITERAL:
          _child = new HexLiteral(lexer).Get();
          break;
        case TokenEnum.REALLITERAL:
          _child = new SingleLiteral(lexer).Get();
          break;
        case TokenEnum.STRINGLITERAL:
          _child = new StringLiteral(lexer).Get();
          break;
        case TokenEnum.BRACEOPEN:
          _child = new ArrayLiteral(lexer).Get();
          break;

        default:
          throw new ParseException(lexer);
      }
    }

    public override CExpression Get()
    {
      return _child;
    }

    public override Val Evaluate(Context context)
    {
      return _child.Evaluate(context);
    }
  }
}
