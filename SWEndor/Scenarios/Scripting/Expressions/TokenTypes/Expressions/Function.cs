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
      // or FUNCNAME()

      _funcName = lexer.TokenContents;
      lexer.Next(); //FUNCTION

      if (lexer.TokenType != TokenEnum.BRACKETOPEN)
        throw new ParseException(lexer, TokenEnum.BRACKETOPEN);
      lexer.Next(); //BRACKETOPEN

      while (lexer.TokenType != TokenEnum.BRACKETCLOSE)
      {
        _param.Add(new Expression(lexer).Get());

        while (lexer.TokenType == TokenEnum.COMMA)
        {
          lexer.Next(); //COMMA
          _param.Add(new Expression(lexer).Get());
        }
      }

      lexer.Next(); //BRACKETCLOSE
    }

    public override object Evaluate(Context context)
    {
      ThreadSafeList<object> result = new ThreadSafeList<object>();
      foreach (CExpression expr in _param.GetList())
      {
        result.Add(expr.Evaluate(context));
      }
      FunctionDelegate fd = context.Functions.Get(_funcName.ToLower());
      if (fd == null)
        throw new EvalException("The function '" + _funcName + "' does not exist!");
      return fd.Invoke(context, result.GetList());
    }
  }
}