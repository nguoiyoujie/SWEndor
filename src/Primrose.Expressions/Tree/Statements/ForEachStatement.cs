using Primrose.Expressions.Tree.Expressions;
using System.Collections.Generic;

namespace Primrose.Expressions.Tree.Statements
{
  internal class ForEachStatement : CStatement
  {
    private ContextScope _scope;
    private CExpression _enumerable;
    private Variable _var;
    private List<CStatement> _actions = new List<CStatement>();

    internal ForEachStatement(ContextScope scope, Lexer lexer) : base(scope, lexer)
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

        _scope = scope.Next;
        _var = (DeclVariable)(new DeclVariable(_scope, lexer).Get());

        if (lexer.TokenType != TokenEnum.IN)
          throw new ParseException(lexer, TokenEnum.IN);
        lexer.Next(); //IN

        _enumerable = new Expression(_scope, lexer).Get();

        if (lexer.TokenType != TokenEnum.BRACKETCLOSE)
          throw new ParseException(lexer, TokenEnum.BRACKETCLOSE);
        lexer.Next(); //BRACKETCLOSE

        if (lexer.TokenType == TokenEnum.BRACEOPEN)
        {
          lexer.Next(); //BRACEOPEN
          while (lexer.TokenType != TokenEnum.BRACECLOSE)
            _actions.Add(new Statement(_scope, lexer).Get());
          lexer.Next(); //BRACECLOSE
        }
        else
        {
          _actions.Add(new Statement(scope, lexer).Get());
        }
      }
      else
      {
        _actions.Add(new IfThenElseStatement(scope, lexer).Get());
      }
    }

    public override CStatement Get()
    {
      if (_enumerable == null)
        return _actions[0];
      return this;
    }

    public override void Evaluate(AContext context)
    {
      if (_enumerable != null)
      {
        Val array = _enumerable.Evaluate(context);

        if (array.Type == ValType.BOOL_ARRAY)
        {
          foreach (bool v in (bool[])array)
          {
            _scope.SetVar(this, _var.varName, new Val(v));
            foreach (CStatement s in _actions)
              s.Evaluate(context);
          }
          return;
        }
        else if (array.Type == ValType.INT_ARRAY)
        {
          foreach (int v in (int[])array)
          {
            _scope.SetVar(this, _var.varName, new Val(v));
            foreach (CStatement s in _actions)
              s.Evaluate(context);
          }
          return;
        }
        else if (array.Type == ValType.FLOAT_ARRAY)
        {
          foreach (float v in (float[])array)
          {
            _scope.SetVar(this, _var.varName, new Val(v));
            foreach (CStatement s in _actions)
              s.Evaluate(context);
          }
          return;
        }
        else if (array.Type == ValType.STRING_ARRAY)
        {
          foreach (string v in (string[])array)
          {
            _scope.SetVar(this, _var.varName, new Val(v));
            foreach (CStatement s in _actions)
              s.Evaluate(context);
          }
          return;
        }
      }

      foreach (CStatement s in _actions)
        s.Evaluate(context);
    }
  }
}