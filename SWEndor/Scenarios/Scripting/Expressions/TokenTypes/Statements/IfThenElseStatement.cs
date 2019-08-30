using SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Statements;
using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class IfThenElseStatement : CStatement
  {
    private CExpression _condition;
    private List<CStatement> _actionIfTrue = new List<CStatement>();
    private List<CStatement> _actionIfFalse = new List<CStatement>();

    internal IfThenElseStatement(Lexer lexer) : base(lexer)
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

        _condition = new Expression(lexer).Get();

        if (lexer.TokenType != TokenEnum.BRACKETCLOSE)
          throw new ParseException(lexer, TokenEnum.BRACKETCLOSE);
        lexer.Next(); //BRACKETCLOSE

        //if (lexer.TokenType != TokenEnum.THEN)
        //  throw new ParseException(lexer, TokenEnum.THEN);
        //lexer.Next(); //THEN

        if (lexer.TokenType == TokenEnum.BRACEOPEN)
        {
          lexer.Next(); //BRACEOPEN
          while (lexer.TokenType != TokenEnum.BRACECLOSE)
            _actionIfTrue.Add(new Statement(lexer).Get());
          lexer.Next(); //BRACECLOSE
        }
        else
        {
          _actionIfTrue.Add(new Statement(lexer).Get());
        }

        if (lexer.TokenType == TokenEnum.ELSE)
        {
          lexer.Next(); //ELSE
          if (lexer.TokenType == TokenEnum.BRACEOPEN)
          {
            lexer.Next(); //BRACEOPEN
            while (lexer.TokenType != TokenEnum.BRACECLOSE)
              _actionIfFalse.Add(new Statement(lexer).Get());
            lexer.Next(); //BRACECLOSE
          }
          else
          {
            _actionIfFalse.Add(new Statement(lexer).Get());
          }
        }
      }
      else
      {
        _actionIfTrue.Add(new AssignmentStatement(lexer).Get());
      }
    }

    public override CStatement Get()
    {
      if (_condition == null)
        return _actionIfTrue[0];
      return this;
    }

    public override void Evaluate(Context context)
    {
      if (_condition == null || (bool)(_condition.Evaluate(context)))
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