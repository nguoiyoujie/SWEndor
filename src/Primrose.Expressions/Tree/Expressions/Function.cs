using System.Collections.Generic;

namespace Primrose.Expressions.Tree.Expressions
{
  internal class Function : CExpression
  {
    private string _funcName;
    private List<CExpression> _param = new List<CExpression>();

    internal Function(ContextScope scope, Lexer lexer) : base(scope, lexer)
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
        _param.Add(new Expression(scope, lexer).Get());

        while (lexer.TokenType == TokenEnum.COMMA)
        {
          lexer.Next(); //COMMA
          _param.Add(new Expression(scope, lexer).Get());
        }
      }

      lexer.Next(); //BRACKETCLOSE
    }

    public override Val Evaluate(AContext context)
    {
      List<Val> parsed = new List<Val>();
      foreach (CExpression expr in _param)
        parsed.Add(expr.Evaluate(context));

      return context.RunFunction(this, _funcName, parsed.ToArray());
    }
  }
}