using Primrose.Primitives.Extensions;
using Primrose.Primitives.ValueTypes;
using System;
using System.Collections.Generic;

namespace Primrose.Expressions
{
  /// <summary>
  /// Provides a context scope for the script
  /// </summary>
  public class ContextScope
  {
    private Dictionary<string, Val> m_variables = new Dictionary<string, Val>();

    /// <summary>The parent of this scope. Evaluates to null if this is the global scope</summary>
    public ContextScope Parent { get; private set; }

    /// <summary>Returns a child of this scope.</summary>
    public ContextScope Next { get { return new ContextScope() { Parent = this }; } }

    /// <summary>Clears the context of information</summary>
    public void Clear()
    {
      m_variables.Clear();
    }

    /// <summary>Declares a variable</summary>
    /// <param name="name">The variable name</param>
    /// <param name="type">The variable type</param>
    /// <param name="lexer">The lexer</param>
    /// <exception cref="InvalidOperationException">Duplicate declaration of a variable in the same scope</exception>
    internal void DeclVar(string name, ValType type, Lexer lexer)
    {
      if (m_variables.ContainsKey(name))
        throw new ParseException(lexer, "Duplicate declaration of variable '{0}' in the same scope".F(name));

      m_variables.Add(name, new Val(type));
    }

    /// <summary>Retrieves the value of a variable</summary>
    /// <param name="eval">The expression object being evaluated</param>
    /// <param name="name">The variable name</param>
    /// <returns>The Val object containing the value of the variable</returns>
    /// <exception cref="InvalidOperationException">Attempted to get the value from an undeclared variable</exception>
    public Val GetVar(ITracker eval, string name)
    {
      Val ret;
      if (!m_variables.TryGetValue(name, out ret))
        if (Parent != null)
          return Parent.GetVar(eval, name);
        else
          throw new EvalException(eval, "Attempted to get undeclared variable '{0}'".F(name));
      return ret;
    }

    /// <summary>Set the value of a variable</summary>
    /// <param name="eval">The expression object being evaluated</param>
    /// <param name="name">The variable name</param>
    /// <param name="val">The Val object containing the value of the variable</param>
    /// <exception cref="InvalidOperationException">Attempted to set the value of an undeclared variable</exception>
    public void SetVar(ITracker eval, string name, Val val)
    {
      if (m_variables.ContainsKey(name))
      {
        WriteVar(name, val);
      }
      else
      {
        if (Parent != null)
          Parent.SetVar(eval, name, val);
        else
          throw new EvalException(eval, "Attempted to set undeclared variable '{0}'".F(name));
      }
    }

    private void WriteVar(string name, Val val)
    {
      ValType t = m_variables[name].Type;
      try
      {
        m_variables[name] = Ops.Coerce(t, val);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException("Illegal assignment to '{0} {1}': {2}".F(t, name, ex.Message));
      }

      /*
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
      */
    }
  }
}
