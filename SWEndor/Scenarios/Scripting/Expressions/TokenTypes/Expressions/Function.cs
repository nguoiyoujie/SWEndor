using SWEndor.Primitives;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class Function : CExpression
  {
    private string _funcName;
    private ThreadSafeList<CExpression> _param = new ThreadSafeList<CExpression>();

    internal Function(Lexer lexer) : base(lexer)
    {
      // FUNCNAME ( PARAM , PARAM , PARAM , ...)
      //  ^

      _funcName = lexer.TokenContents;
      lexer.Next();

      if (lexer.TokenType != TokenEnum.BRACKETOPEN)
        throw new ParseException(lexer, TokenEnum.BRACKETOPEN);
      lexer.Next();

      _param.Add(new Expression(lexer).Get());

      while (lexer.TokenType == TokenEnum.COMMA)
      {
        lexer.Next();
        _param.Add(new Expression(lexer).Get());
      }

      if (lexer.TokenType != TokenEnum.BRACKETCLOSE)
        throw new ParseException(lexer, TokenEnum.BRACKETCLOSE);
      lexer.Next();
    }

    public override object Evaluate(Context context)
    {
      ThreadSafeList<object> result = new ThreadSafeList<object>();
      foreach (CExpression expr in _param.GetList())
      {
        result.Add(expr.Evaluate(context));
      }
      return context.Functions.Get(_funcName)?.Invoke(context, result.GetList());
    }
  }
}