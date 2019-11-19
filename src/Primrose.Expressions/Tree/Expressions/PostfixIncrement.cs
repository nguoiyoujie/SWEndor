namespace Primrose.Expressions.Tree.Expressions
{
  /*
  public class PostfixIncrement : CExpression
  {
    private Variable _child;
    private int _incre = 0;

    public void Parse(Parser parser, Lexer lexer)
    {
      // VARIABLE ++/--/nothing
      //  ^       

      _child = new Variable();
      _child.Parse(parser, lexer);

      lexer.Next();
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
    }

    public object Evaluate(Context context)
    {
      if (_incre == 0)
        return _child.Evaluate(context);

      Context.ContextVariable var = context.Variables.Get(_child.varName);
      if (var == null)
      {
        var = new Context.ContextVariable(_child.varName, _incre);
        context.Variables.Add(_child.varName, var);
      }
      else
      {
        var.Value += _incre;
      }

      return var.Value;
    }
  }
  */
}