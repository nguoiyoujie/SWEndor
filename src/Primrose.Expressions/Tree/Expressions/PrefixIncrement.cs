namespace Primrose.Expressions.Tree.Expressions
{
  /*
  public class PrefixIncrement : CExpression
  {
    private Variable _child;
    private int _incre = 0;

    public void Parse(Parser parser, Lexer lexer)
    {
      // nothing POSTFIXEXPR
      //  ^ 
      //  OR
      // ++/-- VARIABLE
      //  ^       

      switch (lexer.TokenType)
      {
        case TokenEnum.PLUSPLUS:
          _incre = 1;
          lexer.Next();
          break;
        case TokenEnum.MINUSMINUS:
          _incre = -1;
          lexer.Next();
          break;

        default:
          break;
      }

      if (_incre != 0)
        _child = new Variable();
      _child.Parse(parser, lexer);
    }

    public object Evaluate(Context context)
    {
      if (_incre == 0)
        return _child.Evaluate(context);

      // find a way to assign to current variable..
      return (dynamic)_child.Evaluate(context) + _incre;
    }
  }
  */
}