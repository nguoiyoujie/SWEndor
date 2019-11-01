﻿using SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Statements
{
  public class SingleStatement : CStatement
  {
    private CStatement _statement;

    internal SingleStatement(Script local, Lexer lexer) : base(local, lexer)
    {
      _statement = new AssignmentStatement(local, lexer).Get();

      if (lexer.TokenType == TokenEnum.SEMICOLON)
        lexer.Next(); // SEMICOLON
      else
        throw new ParseException(lexer, TokenEnum.SEMICOLON);
    }

    public override CStatement Get()
    {
      return _statement;
    }

    public override void Evaluate(Script local, Context context)
    {
      _statement.Evaluate(local, context);
    }
  }
}
