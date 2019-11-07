using Primrose.Primitives.Extensions;
using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions.Literals
{
  public class ArrayLiteral : CLiteral
  {
    // {x,y,z}
    private List<CExpression> _param = new List<CExpression>();
    Val[] vL;
    bool[] bL;
    float[] fL;
    int[] iL;

    internal ArrayLiteral(Script local, Lexer lexer) : base(local, lexer)
    {
      if (lexer.TokenType != TokenEnum.BRACEOPEN)
        throw new ParseException(lexer, TokenEnum.BRACEOPEN);
      lexer.Next(); //BRACEOPEN

      while (lexer.TokenType != TokenEnum.BRACECLOSE)
      {
        _param.Add(new Expression(local, lexer).Get());

        while (lexer.TokenType == TokenEnum.COMMA)
        {
          lexer.Next(); //COMMA
          _param.Add(new Expression(local, lexer).Get());
        }
      }

      vL = new Val[_param.Count];
      bL = new bool[_param.Count];
      fL = new float[_param.Count];
      iL = new int[_param.Count];
      lexer.Next(); //BRACECLOSE
    }

    public override Val Evaluate(Script local, Context context)
    {
      ValType t = ValType.NULL;
      for (int i = 0; i < _param.Count; i++)
      {
        vL[i] = _param[i].Evaluate(local, context);
        if (t == ValType.NULL)
          t = vL[i].Type;
        else if (t != vL[i].Type)
        {
          if (t == ValType.INT && vL[i].Type == ValType.FLOAT)
            t = ValType.FLOAT;
          else if (t == ValType.FLOAT && vL[i].Type == ValType.INT)
            continue;
          else
            throw new EvalException(this, "Incompatible types detected in array: {0}, {1}".F(t, vL[i].Type));
        }
      }

      switch (t)
      {
        case ValType.BOOL:
          for (int i = 0; i < bL.Length; i++)
            bL[i] = (bool)vL[i];
          return new Val(bL);

        case ValType.INT:
          for (int i = 0; i < iL.Length; i++)
            iL[i] = (int)vL[i];
          return new Val(iL);

        case ValType.FLOAT:
          for (int i = 0; i < fL.Length; i++)
            fL[i] = (float)vL[i];
          return new Val(fL);

        default:
          throw new EvalException(this, "Unsupported array member type: {0}".F(t));
      }

    }
  }
}
