using SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Statements;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class AssignmentStatement: CStatement
  {
    private Variable _variable;
    private TokenEnum _assigntype;
    private CExpression _value;

    internal AssignmentStatement(Lexer lexer) : base(lexer)
    {
      // VARIABLE = EXPR;
      // VARIABLE += EXPR;
      // VARIABLE -= EXPR;
      // VARIABLE *= EXPR;
      // VARIABLE /= EXPR;
      // VARIABLE %= EXPR;
      // VARIABLE &= EXPR;
      // VARIABLE |= EXPR;
      // or
      // EXPR;

      if (lexer.TokenType == TokenEnum.VARIABLE)
      {
        _variable = new Variable(lexer).Get() as Variable;

        _assigntype = lexer.TokenType;
        if (_assigntype == TokenEnum.ASSIGN
         || _assigntype == TokenEnum.PLUSASSIGN
         || _assigntype == TokenEnum.MINUSASSIGN
         || _assigntype == TokenEnum.ASTERISKASSIGN
         || _assigntype == TokenEnum.SLASHASSIGN
         || _assigntype == TokenEnum.PERCENTASSIGN
         || _assigntype == TokenEnum.AMPASSIGN
         || _assigntype == TokenEnum.PIPEASSIGN
          )
        {
          TokenEnum _type = lexer.TokenType;
          lexer.Next();
          _value = new Expression(lexer).Get();
        }
        else
        {
          _assigntype = TokenEnum.NOTHING;
        }

        if (lexer.TokenType == TokenEnum.SEMICOLON)
          lexer.Next();
        else
          throw new ParseException(lexer, TokenEnum.SEMICOLON);
      }
      else
      {
        _assigntype = TokenEnum.NOTHING;
        _value = new Expression(lexer).Get();
      }
    }

    //public override CStatement Get()
    //{
    //  if (_assigntype == TokenEnum.NOTHING)
    //    return _value;
    //  return this;
    //}

    public override void Evaluate(Context context)
    {
      if (_assigntype != TokenEnum.NOTHING)//; _variable != null)
      {
        Context.ContextVariable v = context.Variables.Get(_variable.varName);
        if (v == null)
        {
          v = new Context.ContextVariable(_variable.varName);
          context.Variables.Add(_variable.varName, v);
        }
        switch (_assigntype)
        {
          case TokenEnum.ASSIGN:
            v.Value = (dynamic)_value?.Evaluate(context);
            break;
          case TokenEnum.PLUSASSIGN:
            v.Value += (dynamic)_value?.Evaluate(context);
            break;
          case TokenEnum.MINUSASSIGN:
            v.Value -= (dynamic)_value?.Evaluate(context);
            break;
          case TokenEnum.ASTERISKASSIGN:
            v.Value *= (dynamic)_value?.Evaluate(context);
            break;
          case TokenEnum.SLASHASSIGN:
            v.Value /= (dynamic)_value?.Evaluate(context);
            break;
          case TokenEnum.PERCENTASSIGN:
            v.Value %= (dynamic)_value?.Evaluate(context);
            break;
          case TokenEnum.AMPASSIGN:
            v.Value &= (dynamic)_value?.Evaluate(context);
            break;
          case TokenEnum.PIPEASSIGN:
            v.Value |= (dynamic)_value?.Evaluate(context);
            break;
        }
      }
      else
      {
        _value?.Evaluate(context);
      }
    }
  }
}