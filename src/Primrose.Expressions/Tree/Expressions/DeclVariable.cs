namespace Primrose.Expressions.Tree.Expressions
{
  public class DeclVariable : Variable
  {
    internal DeclVariable(Script local, Lexer lexer) : base(local, lexer, 0)
    {
      ValType varType;
      switch (lexer.TokenType)
      {
        case TokenEnum.DECL_BOOL:
          varType = ValType.BOOL;
          break;

        case TokenEnum.DECL_INT:
          varType = ValType.INT;
          break;

        case TokenEnum.DECL_FLOAT:
          varType = ValType.FLOAT;
          break;

        case TokenEnum.DECL_FLOAT2:
          varType = ValType.FLOAT2;
          break;

        case TokenEnum.DECL_FLOAT3:
          varType = ValType.FLOAT3;
          break;

        case TokenEnum.DECL_FLOAT4:
          varType = ValType.FLOAT4;
          break;

        case TokenEnum.DECL_STRING:
          varType = ValType.STRING;
          break;

        case TokenEnum.DECL_BOOL_ARRAY:
          varType = ValType.BOOL_ARRAY;
          break;

        case TokenEnum.DECL_INT_ARRAY:
          varType = ValType.INT_ARRAY;
          break;

        case TokenEnum.DECL_FLOAT_ARRAY:
          varType = ValType.FLOAT_ARRAY;
          break;

        default:
          throw new ParseException(lexer);
      }
      lexer.Next(); //DECL

      if (lexer.TokenType != TokenEnum.VARIABLE)
        throw new ParseException(lexer, TokenEnum.VARIABLE);

      varName = lexer.TokenContents;
      local.DeclVar(varName, varType);
      
      lexer.Next(); //VARIABLE
    }

    public override Val Evaluate(Script local, AContext context)
    {
      return Val.NULL; 
    }
  }
}