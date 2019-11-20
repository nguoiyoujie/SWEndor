using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Primrose.Primitives.Extensions;
using System.Globalization;

namespace Primrose.Expressions.UnitTests
{
  [TestClass]
  public class Literals_Test
  {
    [TestMethod]
    public void Literal_Bool()
    {
      ScriptExpression expr;

      string[] test = { "true", "false" };
      foreach (string s in test)
      {
        bool i = Boolean.Parse(s);
        expr = new ScriptExpression(s);
        Console.Write("{0} = {1}".F(s, i));
        Assert.AreEqual(i, (bool)expr.Evaluate(null));
        Console.WriteLine(" ... OK!");
      }
    }

    [TestMethod]
    public void Literal_Int()
    {
      ScriptExpression expr;

      string[] test = {
        "0", // zero
        "1", "432", "57965", // natural
        "-2", "-93", "-24611", // negative
        "0xc", "0xff", "0xd045" // hex
      };
      foreach (string s in test)
      {
        int i = s.StartsWith("0x") 
          ? Int32.Parse(s.Substring(2), NumberStyles.HexNumber)
          : Int32.Parse(s, NumberStyles.Number);
        expr = new ScriptExpression(s);
        Console.Write("{0} = {1}".F(s, i));
        Assert.AreEqual(i, (int)expr.Evaluate(null));
        Console.WriteLine(" ... OK!");
      }
    }

    [TestMethod]
    public void Literal_Float()
    {
      ScriptExpression expr;

      string[] test = {
        "0.0", // zero
        "0.54", "0.231", "0.00001", // 0 < x < 1
        "20.4", "539022.2",
        "10", "24", // int as float
        "-6509.2", "-35.13896" // negative
      };

      foreach (string s in test)
      {
        float i = Single.Parse(s);
        expr = new ScriptExpression(s);
        Console.Write("{0} = {1}".F(s, i));
        Assert.AreEqual(i, (float)expr.Evaluate(null));
        Console.WriteLine(" ... OK!");
      }
    }

    [TestMethod]
    public void Literal_String()
    {
      ScriptExpression expr;

      string[] test = {
        "\"yes\"",
        "\"affirmative\"",
        "\"joy to the world\"",
        "\"RAGE_AGE_GE_E\"",
        "\"Weird.pun|ct&u@a$ti()o/n!\"",
        "\"[(brackets)]{}\"",
      };

      foreach (string s in test)
      {
        string i = s.Trim('\"');
        expr = new ScriptExpression(s);
        Console.Write(i);
        Assert.AreEqual(i, (string)expr.Evaluate(null));
        Console.WriteLine(" ... OK!");
      }
    }
  }
}
