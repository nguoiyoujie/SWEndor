using SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Statements;
using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions
{
  public class ForEachStatement : CStatement
  {
    private CExpression _enumerable;
    private Variable _var;
    private List<CStatement> _actions = new List<CStatement>();

    internal ForEachStatement(Lexer lexer) : base(lexer)
    {
      // FOREACH ( VAR IN EXPR ) STATEMENT 
      // FOREACH ( VAR IN EXPR ) { STATEMENT STATEMENT STATEMENT ... } 
      // or
      // ASSIGNMENTEXPR

      if (lexer.TokenType == TokenEnum.FOREACH)
      {
        lexer.Next(); //FOREACH
        if (lexer.TokenType != TokenEnum.BRACKETOPEN)
          throw new ParseException(lexer, TokenEnum.BRACKETOPEN);
        lexer.Next(); //BRACKETOPEN

        _var = (Variable)(new Variable(lexer).Get());

        if (lexer.TokenType != TokenEnum.IN)
          throw new ParseException(lexer, TokenEnum.IN);
        lexer.Next(); //IN

        _enumerable = new Expression(lexer).Get();

        if (lexer.TokenType != TokenEnum.BRACKETCLOSE)
          throw new ParseException(lexer, TokenEnum.BRACKETCLOSE);
        lexer.Next(); //BRACKETCLOSE

        if (lexer.TokenType == TokenEnum.BRACEOPEN)
        {
          lexer.Next(); //BRACEOPEN
          while (lexer.TokenType != TokenEnum.BRACECLOSE)
            _actions.Add(new Statement(lexer).Get());
          lexer.Next(); //BRACECLOSE
        }
        else
        {
          _actions.Add(new Statement(lexer).Get());
        }
      }
      else
      {
        _actions.Add(new IfThenElseStatement(lexer).Get());
      }
    }

    public override CStatement Get()
    {
      if (_enumerable == null)
        return _actions[0];
      return this;
    }

    public override void Evaluate(Context context)
    {
      if (_enumerable != null)
      {
        Val array = _enumerable.Evaluate(context);
        if (array.Type == ValType.INT_ARRAY)
        {
          foreach (int v in array.aI)
          {
            context.Variables.Put(_var.varName, new Context.ContextVariable(_var.varName, new Val(v))); 
            foreach (CStatement s in _actions)
              s.Evaluate(context);
          }
          return;
        }
        else if (array.Type == ValType.INT_ARRAY)
        {
          foreach (float v in array.aF)
          {
            context.Variables.Put(_var.varName, new Context.ContextVariable(_var.varName, new Val(v)));
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