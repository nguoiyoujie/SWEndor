using Primrose.Expressions.Tree.Expressions;
using System.Collections.Generic;

namespace Primrose.Expressions.Tree.Statements
{
  public class WhileStatement : CStatement
  {
    private CExpression _condition;
    private List<CStatement> _action = new List<CStatement>();

    internal WhileStatement(Script local, Lexer lexer) : base(local, lexer)
    {
      // WHILE ( EXPR ) STATEMENT 
      // WHILE ( EXPR ) { STATEMENT STATEMENT STATEMENT ... } 
      // or
      // 

      if (lexer.TokenType == TokenEnum.WHILE)
      {
        lexer.Next(); //WHILE
        if (lexer.TokenType != TokenEnum.BRACKETOPEN)
          throw new ParseException(lexer, TokenEnum.BRACKETOPEN);
        lexer.Next(); //BRACKETOPEN

        _condition = new Expression(local, lexer).Get();

        if (lexer.TokenType != TokenEnum.BRACKETCLOSE)
          throw new ParseException(lexer, TokenEnum.BRACKETCLOSE);
        lexer.Next(); //BRACKETCLOSE

        if (lexer.TokenType == TokenEnum.BRACEOPEN)
        {
          lexer.Next(); //BRACEOPEN
          while (lexer.TokenType != TokenEnum.BRACECLOSE)
            _action.Add(new Statement(local, lexer).Get());
          lexer.Next(); //BRACECLOSE
        }
        else
        {
          _action.Add(new Statement(local, lexer).Get());
        }
      }
      else
      {
        _action.Add(new ForStatement(local, lexer).Get());
      }
    }

    public override CStatement Get()
    {
      if (_condition == null)
        return _action[0];
      return this;
    }

    public override void Evaluate(Script local, AContext context)
    {
      while (_condition.Evaluate(local, context).IsTrue)
      {
        foreach (CStatement s in _action)
          s.Evaluate(local, context);
      }
    }
  }
}