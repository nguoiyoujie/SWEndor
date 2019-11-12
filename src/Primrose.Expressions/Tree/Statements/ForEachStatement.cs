using Primrose.Expressions.Tree.Expressions;
using System.Collections.Generic;

namespace Primrose.Expressions.Tree.Statements
{
  internal class ForEachStatement : CStatement
  {
    private CExpression _enumerable;
    private Variable _var;
    private List<CStatement> _actions = new List<CStatement>();

    internal ForEachStatement(Script local, Lexer lexer) : base(local, lexer)
    {
      // FOREACH ( DECL VAR IN EXPR ) STATEMENT 
      // FOREACH ( DECL VAR IN EXPR ) { STATEMENT STATEMENT STATEMENT ... } 
      // or
      // ASSIGNMENTEXPR

      if (lexer.TokenType == TokenEnum.FOREACH)
      {
        lexer.Next(); //FOREACH
        if (lexer.TokenType != TokenEnum.BRACKETOPEN)
          throw new ParseException(lexer, TokenEnum.BRACKETOPEN);
        lexer.Next(); //BRACKETOPEN

        _var = (DeclVariable)(new DeclVariable(local, lexer).Get());

        if (lexer.TokenType != TokenEnum.IN)
          throw new ParseException(lexer, TokenEnum.IN);
        lexer.Next(); //IN

        _enumerable = new Expression(local, lexer).Get();

        if (lexer.TokenType != TokenEnum.BRACKETCLOSE)
          throw new ParseException(lexer, TokenEnum.BRACKETCLOSE);
        lexer.Next(); //BRACKETCLOSE

        if (lexer.TokenType == TokenEnum.BRACEOPEN)
        {
          lexer.Next(); //BRACEOPEN
          while (lexer.TokenType != TokenEnum.BRACECLOSE)
            _actions.Add(new Statement(local, lexer).Get());
          lexer.Next(); //BRACECLOSE
        }
        else
        {
          _actions.Add(new Statement(local, lexer).Get());
        }
      }
      else
      {
        _actions.Add(new IfThenElseStatement(local, lexer).Get());
      }
    }

    public override CStatement Get()
    {
      if (_enumerable == null)
        return _actions[0];
      return this;
    }

    public override void Evaluate(Script local, AContext context)
    {
      if (_enumerable != null)
      {
        Val array = _enumerable.Evaluate(local, context);

        if (array.Type == ValType.BOOL_ARRAY)
        {
          foreach (bool v in (bool[])array)
          {
            local.SetVar(_var.varName, new Val(v));
            foreach (CStatement s in _actions)
              s.Evaluate(local, context);
          }
          return;
        }
        else if (array.Type == ValType.INT_ARRAY)
        {
          foreach (int v in (int[])array)
          {
            local.SetVar(_var.varName, new Val(v));
            foreach (CStatement s in _actions)
              s.Evaluate(local, context);
          }
          return;
        }
        else if (array.Type == ValType.FLOAT_ARRAY)
        {
          foreach (float v in (float[])array)
          {
            local.SetVar(_var.varName, new Val(v));
            foreach (CStatement s in _actions)
              s.Evaluate(local, context);
          }
          return;
        }
        else if (array.Type == ValType.STRING_ARRAY)
        {
          foreach (string v in (string[])array)
          {
            local.SetVar(_var.varName, new Val(v));
            foreach (CStatement s in _actions)
              s.Evaluate(local, context);
          }
          return;
        }
      }

      foreach (CStatement s in _actions)
        s.Evaluate(local, context);
    }
  }
}