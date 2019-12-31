using System.Collections.Generic;
using Primrose.Expressions.Tree.Statements;

namespace Primrose.Expressions
{
  /// <summary>
  /// Represents a set of parsable statements
  /// </summary>
  public partial class Script
  {
    private List<RootStatement> m_statements = new List<RootStatement>();

    /// <summary>The base context scope of the script</summary>
    public ContextScope Scope;

    /// <summary>The name of the script</summary>
    public readonly string Name;
    internal Script() { Scope = new ContextScope(); }

    /// <summary>Creates a script</summary>
    /// <param name="scriptname">The name of the script</param>
    public Script(string scriptname)
    {
      Name = scriptname;
      Registry.Add(scriptname, this);
      Scope = Registry.Global.Scope.Next;
    }

    /// <summary>Adds one or more statements to the script.</summary>
    /// <param name="text">The string text to be parsed</param>
    /// <param name="linenumber">The current line number</param>
    public void AddStatements(string text, ref int linenumber)
    {
      RootStatement statement;
      Parser.Parse(Scope, text, out statement, Name, ref linenumber);
      m_statements.Add(statement);
    }

    /// <summary>Clears the script of information</summary>
    public void Clear()
    {
      m_statements.Clear();
      Scope.Clear();
    }

    /// <summary>Evaluates the script</summary>
    /// <param name="context">The script context</param>
    public void Run(AContext context)
    {
      foreach (RootStatement statement in m_statements)
        statement.Evaluate(context);
    }

  }
}
