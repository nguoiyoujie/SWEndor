using System.IO;
using System.Collections.Generic;
using SWEndor.Scenarios.Scripting.Expressions;
using SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Statements;
using SWEndor.Primitives.Extensions;
using System;

namespace SWEndor.Scenarios.Scripting
{
  public partial class Script
  {
    private List<RootStatement> m_statements = new List<RootStatement>();
    private Dictionary<string, Val> m_variables = new Dictionary<string, Val>();

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
      Parser.Parse(this, line, out statement, ref linenumber);
      m_statements.Add(statement);
    }

    public void Clear()
    {
      m_statements.Clear();
      m_variables.Clear();
    }

    public void Run(Context context)
    {
      foreach (RootStatement statement in m_statements)
        statement.Evaluate(this, context);
    }

    public void DeclVar(string name, ValType type)
    {
      if (m_variables.ContainsKey(name))
        throw new InvalidOperationException("Duplicate declaration of variable '{0}' in same scope".F(name));

      m_variables.Add(name, new Val(type));
    }

    public Val GetVar(string name)
    {
      Val ret;
      if (!m_variables.TryGetValue(name, out ret))
        if (this == Registry.Global || !Registry.Global.m_variables.TryGetValue(name, out ret))
          throw new InvalidOperationException("Attempted to get undeclared variable '{0}'".F(name));
      return ret;
    }

    public void SetVar(string name, Val val)
    {
      if (m_variables.ContainsKey(name))
      {
        WriteVar(name, val);
      }
      else if (this != Registry.Global)
        Registry.Global.SetVar(name, val);
      else
        throw new InvalidOperationException("Attempted to set undeclared variable '{0}'".F(name));
    }

    private void WriteVar(string name, Val val)
    {
      ValType t = m_variables[name].Type;

      if (t == val.Type)
        m_variables[name] = val;

      // float can accept int
      else if (t == ValType.FLOAT && val.Type == ValType.INT)
        m_variables[name] = new Val((float)val);

      // int can accept float
      else if (t == ValType.INT && val.Type == ValType.FLOAT)
        m_variables[name] = new Val((int)val);

      else
        throw new InvalidOperationException("Attempted assignment of value of type '{0}' to variable '{1} {2}'".F(val.Type, name, t));
    }
  }
}
