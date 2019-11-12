using System.Collections.Generic;
using Primrose.Primitives.Extensions;
using System;
using Primrose.Primitives.ValueTypes;
using Primrose.Expressions.Tree.Statements;

namespace Primrose.Expressions
{
  /// <summary>
  /// Represents a set of parsable statements
  /// </summary>
  public partial class Script
  {
    private List<RootStatement> m_statements = new List<RootStatement>();
    private Dictionary<string, Val> m_variables = new Dictionary<string, Val>();

    /// <summary>The name of the script</summary>
    public readonly string Name;
    internal Script() { }

    /// <summary>Creates a script</summary>
    /// <param name="scriptname">The name of the script</param>
    public Script(string scriptname)
    {
      Name = scriptname;
      Registry.Add(scriptname, this);
    }

    /// <summary>Adds one or more statements to the script.</summary>
    /// <param name="text">The string text to be parsed</param>
    /// <param name="linenumber">The current line number</param>
    public void AddStatements(string text, ref int linenumber)
    {
      RootStatement statement;
      Parser.Parse(this, text, out statement, ref linenumber);
      m_statements.Add(statement);
    }

    /// <summary>Clears the script of information</summary>
    public void Clear()
    {
      m_statements.Clear();
      m_variables.Clear();
    }

    /// <summary>Evaluates the script</summary>
    /// <param name="context">The script context</param>
    public void Run(AContext context)
    {
      foreach (RootStatement statement in m_statements)
        statement.Evaluate(this, context);
    }

    /// <summary>Declares a variable</summary>
    /// <param name="name">The variable name</param>
    /// <param name="type">The variable type</param>
    /// <exception cref="InvalidOperationException">Duplicate declaration of a variable in the same scope</exception>
    internal void DeclVar(string name, ValType type)
    {
      if (m_variables.ContainsKey(name))
        throw new InvalidOperationException("Duplicate declaration of variable '{0}' in the same scope".F(name));

      m_variables.Add(name, new Val(type));
    }

    /// <summary>Retrieves the value of a variable</summary>
    /// <param name="name">The variable name</param>
    /// <returns>The Val object containing the value of the variable</returns>
    /// <exception cref="InvalidOperationException">Attempted to get the value from an undeclared variable</exception>
    internal Val GetVar(string name)
    {
      Val ret;
      if (!m_variables.TryGetValue(name, out ret))
        if (this == Registry.Global || !Registry.Global.m_variables.TryGetValue(name, out ret))
          throw new InvalidOperationException("Attempted to get undeclared variable '{0}'".F(name));
      return ret;
    }

    /// <summary>Set the value of a variable</summary>
    /// <param name="name">The variable name</param>
    /// <param name="val">The Val object containing the value of the variable</param>
    /// <exception cref="InvalidOperationException">Attempted to set the value of an undeclared variable</exception>
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

      // float2/3/4 can accept float_array/int_array of same or smaller length
      else if (t == ValType.FLOAT2 && val.Type == ValType.FLOAT_ARRAY)
      {
        float[] fv = (float[])val;
        int len = fv.Length;
        if (len == 2)
          m_variables[name] = new Val(float2.FromArray(fv));
        else
          throw new InvalidOperationException("Attempted assignment of an array of length {0} to variable '{1} {2}'".F(len, t, name));
      }

      else if (t == ValType.FLOAT3 && val.Type == ValType.FLOAT_ARRAY)
      {
        float[] fv = (float[])val;
        int len = fv.Length;
        if (len == 3)
          m_variables[name] = new Val(float3.FromArray(fv));
        else
          throw new InvalidOperationException("Attempted assignment of an array of length {0} to variable '{1} {2}'".F(len, t, name));
      }

      else if (t == ValType.FLOAT4 && val.Type == ValType.FLOAT_ARRAY)
      {
        float[] fv = (float[])val;
        int len = fv.Length;
        if (len == 4)
          m_variables[name] = new Val(float4.FromArray(fv));
        else
          throw new InvalidOperationException("Attempted assignment of an array of length {0} to variable '{1} {2}'".F(len, t, name));
      }

      else if (t == ValType.FLOAT2 && val.Type == ValType.INT_ARRAY)
      {
        int[] fv = (int[])val;
        int len = fv.Length;
        if (len == 2)
          m_variables[name] = new Val(float2.FromArray(fv));
        else
          throw new InvalidOperationException("Attempted assignment of an array of length {0} to variable '{1} {2}'".F(len, t, name));
      }

      else if (t == ValType.FLOAT3 && val.Type == ValType.INT_ARRAY)
      {
        int[] fv = (int[])val;
        int len = fv.Length;
        if (len == 3)
          m_variables[name] = new Val(float3.FromArray(fv));
        else
          throw new InvalidOperationException("Attempted assignment of an array of length {0} to variable '{1} {2}'".F(len, t, name));
      }

      else if (t == ValType.FLOAT4 && val.Type == ValType.INT_ARRAY)
      {
        int[] fv = (int[])val;
        int len = fv.Length;
        if (len == 4)
          m_variables[name] = new Val(float4.FromArray(fv));
        else
          throw new InvalidOperationException("Attempted assignment of an array of length {0} to variable '{1} {2}'".F(len, t, name));
      }

      else
        throw new InvalidOperationException("Attempted assignment of value of type '{0}' to variable '{1} {2}'".F(val.Type, name, t));
    }
  }
}
