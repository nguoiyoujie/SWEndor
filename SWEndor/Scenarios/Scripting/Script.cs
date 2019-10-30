using System.IO;
using System.Collections.Generic;
using SWEndor.Scenarios.Scripting.Expressions;
using SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Statements;
using SWEndor.Primitives.Extensions;

namespace SWEndor.Scenarios.Scripting
{
  public partial class Script
  {
    private List<RootStatement> m_statements = new List<RootStatement>();

    public readonly string Name;
    internal Script() { }

    public Script(string scriptname)
    {
      Globals.Engine.Screen2D.LoadingTextLines.Add("loading script:".C(scriptname));
      Globals.Engine.Screen2D.LoadingTextLines.RemoveAt(0);
      Name = scriptname;
      Registry.Add(scriptname, this);
    }

    public void AddExpression(string line, ref int linenumber)
    {
      RootStatement statement;
      Parser.Parse(line, out statement, ref linenumber);
      m_statements.Add(statement);
    }

    public void Run(Context context)
    {
      foreach (RootStatement statement in m_statements)
        statement.Evaluate(context);
    }
  }
}
