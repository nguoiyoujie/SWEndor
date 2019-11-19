using Primrose.Expressions.Tree.Expressions;
using System.Collections.Generic;

namespace Primrose.Expressions.Tree.Statements
{
  internal class IfThenElseStatement : CStatement
  {
    private ContextScope _scopeT;
    private ContextScope _scopeF;

    private CExpression _condition;
    private List<CStatement> _actionIfTrue = new List<CStatement>();
    private List<CStatement> _actionIfFalse = new List<CStatement>();

    internal IfThenElseStatement(ContextScope scope, Lexer lexer) : base(scope, lexer)
    {
      // IF ( EXPR ) STATEMENT ELSE STATEMENT
      // IF ( EXPR ) { STATEMENT STATEMENT STATEMENT ... } ELSE { STATEMENT ... }
      // or
      // ASSIGNMENTEXPR

      if (lexer.TokenType == TokenEnum.IF)
      {
        lexer.Next(); //IF
        if (lexer.TokenType != TokenEnum.BRACKETOPEN)
          throw new ParseException(lexer, TokenEnum.BRACKETOPEN);
        lexer.Next(); //BRACKETOPEN

        _condition = new Expression(scope, lexer).Get();

        if (lexer.TokenType != TokenEnum.BRACKETCLOSE)
          throw new ParseException(lexer, TokenEnum.BRACKETCLOSE);
        lexer.Next(); //BRACKETCLOSE

        //if (lexer.TokenType != TokenEnum.THEN)
        //  throw new ParseException(lexer, TokenEnum.THEN);
        //lexer.Next(); //THEN

        if (lexer.TokenType == TokenEnum.BRACEOPEN)
        {
          _scopeT = scope.Next;
          lexer.Next(); //BRACEOPEN
          while (lexer.TokenType != TokenEnum.BRACECLOSE)
            _actionIfTrue.Add(new Statement(_scopeT, lexer).Get());
          lexer.Next(); //BRACECLOSE
        }
        else
        {
          _actionIfTrue.Add(new Statement(scope, lexer).Get());
        }

        if (lexer.TokenType == TokenEnum.ELSE)
        {
          lexer.Next(); //ELSE
          if (lexer.TokenType == TokenEnum.BRACEOPEN)
          {
            _scopeF = scope.Next;
            lexer.Next(); //BRACEOPEN
            while (lexer.TokenType != TokenEnum.BRACECLOSE)
              _actionIfFalse.Add(new Statement(_scopeF, lexer).Get());
            lexer.Next(); //BRACECLOSE
          }
          else
          {
            _actionIfFalse.Add(new Statement(scope, lexer).Get());
          }
        }
      }
      else
      {
        _actionIfTrue.Add(new SingleStatement(scope, lexer).Get());
      }
    }

    public override CStatement Get()
    {
      if (_condition == null)
        return _actionIfTrue[0];
      return this;
    }

    public override void Evaluate(AContext context)
    {
      if (_condition == null || _condition.Evaluate(context).IsTrue)
      {
        foreach (CStatement s in _actionIfTrue)
          s.Evaluate(context);
      }
      else
      {
        foreach (CStatement s in _actionIfFalse)
          s.Evaluate(context);
      }
    }
  }
}