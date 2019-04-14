using System.IO;
using System.Collections.Generic;
using SWEndor.Scenarios.Scripting.Expressions;
using SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Statements;

namespace SWEndor.Scenarios.Scripting
{
  public partial class Script
  {
    private List<RootStatement> m_statements = new List<RootStatement>();

    public readonly string Name;
    public Script(string filepath, string scriptname)
    {
      if (!File.Exists(filepath))
        throw new FileNotFoundException("Script file '" + filepath + "' is not found!");

      Screen2D.Instance().LoadingTextLines.Add("loading script:" + scriptname);
      Screen2D.Instance().LoadingTextLines.RemoveAt(0);
      Name = scriptname;
      Registry.Add(scriptname, this);
    }

    public void AddExpression(string line)
    {
      RootStatement statement;
      Parser.Parse(line, out statement);
      m_statements.Add(statement);
    }

    public void Run()
    {
      foreach (RootStatement statement in m_statements)
        statement.Evaluate(SWContext.Instance);
    }
  }
}
