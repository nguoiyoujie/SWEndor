using Primrose.Expressions.Tree.Expressions;

namespace Primrose.Expressions.Tree.Statements
{
  internal class AssignmentStatement: CStatement
  {
    private Variable _variable;
    private TokenEnum _assigntype;
    private CExpression _value;

    internal AssignmentStatement(Script local, Lexer lexer) : base(local, lexer)
    {
      // VARIABLE = EXPR;
      // VARIABLE += EXPR;
      // VARIABLE -= EXPR;
      // VARIABLE *= EXPR;
      // VARIABLE /= EXPR;
      // VARIABLE %= EXPR;
      // VARIABLE &= EXPR;
      // VARIABLE |= EXPR;
      // TO DO: VARIABLE++/--;
      // or
      // EXPR;

      switch (lexer.TokenType)
      {
        case TokenEnum.DECL_BOOL:
        case TokenEnum.DECL_INT:
        case TokenEnum.DECL_FLOAT:
        case TokenEnum.DECL_FLOAT2:
        case TokenEnum.DECL_FLOAT3:
        case TokenEnum.DECL_FLOAT4:
        case TokenEnum.DECL_STRING:
        case TokenEnum.DECL_BOOL_ARRAY:
        case TokenEnum.DECL_INT_ARRAY:
        case TokenEnum.DECL_FLOAT_ARRAY:
        case TokenEnum.VARIABLE:
          {
            if (lexer.TokenType == TokenEnum.VARIABLE)
              _variable = new Variable(local, lexer).Get() as Variable;
            else
              _variable = new DeclVariable(local, lexer).Get() as DeclVariable;

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
              lexer.Next(); //ASSIGN
              _value = new Expression(local, lexer).Get();
            }
            else
            {
              _assigntype = TokenEnum.NOTHING;
            }
          }
          break;
        default:
          {
            _assigntype = TokenEnum.NOTHING;
            _value = new Expression(local, lexer).Get();
          }
          break;
      }
    }

    public override void Evaluate(Script local, AContext context)
    {
      if (_assigntype != TokenEnum.NOTHING)
      {
        Val v = local.GetVar(_variable.varName);

        switch (_assigntype)
        {
          case TokenEnum.ASSIGN:
            v = _value?.Evaluate(local, context) ?? Val.NULL;
            break;
          case TokenEnum.PLUSASSIGN:
            v = Ops.Do(BOp.ADD, v, _value?.Evaluate(local, context) ?? Val.NULL); ;
            break;
          case TokenEnum.MINUSASSIGN:
            v = Ops.Do(BOp.SUBTRACT, v, _value?.Evaluate(local, context) ?? Val.NULL); ;
            break;
          case TokenEnum.ASTERISKASSIGN:
            v = Ops.Do(BOp.MULTIPLY, v, _value?.Evaluate(local, context) ?? Val.NULL); ;
            break;
          case TokenEnum.SLASHASSIGN:
            v = Ops.Do(BOp.DIVIDE, v, _value?.Evaluate(local, context) ?? Val.NULL); ;
            break;
          case TokenEnum.PERCENTASSIGN:
            v = Ops.Do(BOp.MODULUS, v, _value?.Evaluate(local, context) ?? Val.NULL); ;
            break;
          case TokenEnum.AMPASSIGN:
            v = Ops.Do(BOp.LOGICAL_AND, v, _value?.Evaluate(local, context) ?? Val.NULL); ;
            break;
          case TokenEnum.PIPEASSIGN:
            v = Ops.Do(BOp.LOGICAL_OR, v, _value?.Evaluate(local, context) ?? Val.NULL); ;
            break;
        }
        local.SetVar(_variable.varName, v);
      }
      else
      {
        _value?.Evaluate(local, context);
      }
    }
  }
}