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

        if (lexer.TokenType == TokenEnum.SEMICOLON)
          lexer.Next();
        else
          throw new ParseException(lexer, TokenEnum.SEMICOLON);
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
            v.Value = _value?.Evaluate(context) ?? Val.NULL;
            break;
          case TokenEnum.PLUSASSIGN:
            v.Value = Ops.Do(BOp.ADD, v.Value, _value?.Evaluate(context) ?? Val.NULL); ;
            break;
          case TokenEnum.MINUSASSIGN:
            v.Value = Ops.Do(BOp.SUBTRACT, v.Value, _value?.Evaluate(context) ?? Val.NULL); ;
            break;
          case TokenEnum.ASTERISKASSIGN:
            v.Value = Ops.Do(BOp.MULTIPLY, v.Value, _value?.Evaluate(context) ?? Val.NULL); ;
            break;
          case TokenEnum.SLASHASSIGN:
            v.Value = Ops.Do(BOp.DIVIDE, v.Value, _value?.Evaluate(context) ?? Val.NULL); ;
            break;
          case TokenEnum.PERCENTASSIGN:
            v.Value = Ops.Do(BOp.MODULUS, v.Value, _value?.Evaluate(context) ?? Val.NULL); ;
            break;
          case TokenEnum.AMPASSIGN:
            v.Value = Ops.Do(BOp.LOGICAL_AND, v.Value, _value?.Evaluate(context) ?? Val.NULL); ;
            break;
          case TokenEnum.PIPEASSIGN:
            v.Value = Ops.Do(BOp.LOGICAL_OR, v.Value, _value?.Evaluate(context) ?? Val.NULL); ;
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