using Primrose.Primitives.Extensions;
using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class Function : CExpression
  {
    private string _funcName;
    private List<CExpression> _param = new List<CExpression>();

    internal Function(Script local, Lexer lexer) : base(local, lexer)
    {
      // FUNCNAME ( PARAM , PARAM , PARAM , ...)
      //  ^
      // or FUNCNAME()

      _funcName = lexer.TokenContents;
      lexer.Next(); //FUNCTION

      if (lexer.TokenType != TokenEnum.BRACKETOPEN)
        throw new ParseException(lexer, TokenEnum.BRACKETOPEN);
      lexer.Next(); //BRACKETOPEN

      while (lexer.TokenType != TokenEnum.BRACKETCLOSE)
      {
        _param.Add(new Expression(local, lexer).Get());

        while (lexer.TokenType == TokenEnum.COMMA)
        {
          lexer.Next(); //COMMA
          _param.Add(new Expression(local, lexer).Get());
        }
      }

      lexer.Next(); //BRACKETCLOSE
    }

    public override Val Evaluate(Script local, Context context)
    {
      List<Val> parsed = new List<Val>();
      foreach (CExpression expr in _param)
      {
        parsed.Add(expr.Evaluate(local, context));
      }
      FunctionDelegate fd = context.Functions.Get(_funcName);
      if (fd == null)
        throw new EvalException(this, "The function '{0}' does not exist!".F(_funcName));
      return fd.Invoke(context, parsed.ToArray());
    }
  }
}