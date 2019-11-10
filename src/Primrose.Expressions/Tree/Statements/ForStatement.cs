using Primrose.Expressions.Tree.Expressions;
using System.Collections.Generic;

namespace Primrose.Expressions.Tree.Statements
{
  internal class ForStatement : CStatement
  {
    private CStatement _begin;
    private CExpression _condition;
    private CStatement _next;
    private List<CStatement> _actions = new List<CStatement>();

    internal ForStatement(Script local, Lexer lexer) : base(local, lexer)
    {
      // FOR ( STATEMENT; CONDEXPR; EXPR ) STATEMENT 
      // FOR ( STATEMENT; CONDEXPR; EXPR ) { STATEMENT STATEMENT STATEMENT ... } 
      // or
      // ASSIGNMENTEXPR

      if (lexer.TokenType == TokenEnum.FOR)
      {
        lexer.Next(); //FOR
        if (lexer.TokenType != TokenEnum.BRACKETOPEN)
          throw new ParseException(lexer, TokenEnum.BRACKETOPEN);
        lexer.Next(); //BRACKETOPEN

        _begin = new Statement(local, lexer).Get();

        _condition = new Expression(local, lexer).Get();

        if (lexer.TokenType != TokenEnum.SEMICOLON)
          throw new ParseException(lexer, TokenEnum.SEMICOLON);
        lexer.Next(); //SEMICOLON

        _next = new AssignmentStatement(local, lexer).Get();

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
        _actions.Add(new ForEachStatement(local, lexer).Get());
      }
    }

    public override CStatement Get()
    {
      if (_condition == null)
        return _actions[0];
      return this;
    }

    public override void Evaluate(Script local, AContext context)
    {
      if (_condition != null)
      {
        _begin.Evaluate(local, context);

        while (_condition.Evaluate(local, context).IsTrue)
        {
          foreach (CStatement s in _actions)
            s.Evaluate(local, context);

          _next.Evaluate(local, context);
        }
        return;
      }

      foreach (CStatement s in _actions)
        s.Evaluate(local, context);
    }
  }
}