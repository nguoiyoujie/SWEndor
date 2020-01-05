using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.ValueTypes;
using System.Collections.Generic;
using Primrose.Expressions.UnitTests.Scripting;

namespace Primrose.Expressions.UnitTests
{
  [TestClass]
  public class Operations_Test
  {
    [TestMethod]
    public void Operation_Math()
    {
      ScriptExpression expr;

      Pair<string, Val>[] test = new Pair<string, Val>[]
      {
        new Pair<string, Val>("1 + 1", new Val(1 + 1)),
        new Pair<string, Val>("2 - 3", new Val(2 - 3)),
        new Pair<string, Val>("4 * 5.2", new Val(4 * 5.2f)),
        new Pair<string, Val>("10 / 0.25", new Val(10 / 0.25f)),
        new Pair<string, Val>("10 % 3", new Val(10 % 3)),
        new Pair<string, Val>("2 * (4 + 5)", new Val(2 * (4 + 5))),
        new Pair<string, Val>("3 - (6 - 2 / 2)", new Val(3 - (6 - 2 / 2))),
        new Pair<string, Val>("((2 + 2) * (1.0 / 2)) - ((2 + 2) / (2 / 2))", new Val(((2 + 2) * (1.0f / 2)) - ((2 + 2) / (2 / 2)))),

        // int division 1 / 2 -> 0
        new Pair<string, Val>("((2 + 2) * (1 / 2)) - ((2 + 2) / (2 / 2))", new Val(((2 + 2) * (1 / 2)) - ((2 + 2) / (2 / 2)))),
      };

      foreach (Pair<string, Val> p in test)
      {
        expr = new ScriptExpression(p.t);
        Console.Write("{0} = {1}".F(p.t, p.u.Value));
        Assert.AreEqual(p.u, expr.Evaluate(null));
        Console.WriteLine(" ... OK!");
      }
    }

    [TestMethod]
    public void Operation_Concat()
    {
      ScriptExpression expr;

      Pair<string, Val>[] test = new Pair<string, Val>[]
      {
        new Pair<string, Val>("\"str\" + 1", new Val("str" + 1)),
        new Pair<string, Val>("0 + \"str\"", new Val(0 + "str")),
        new Pair<string, Val>("\"first\" + \"next\"", new Val("first" + "next")),
      };

      foreach (Pair<string, Val> p in test)
      {
        expr = new ScriptExpression(p.t);
        Console.Write("{0} = {1}".F(p.t, p.u.Value));
        Assert.AreEqual(p.u, expr.Evaluate(null));
        Console.WriteLine(" ... OK!");
      }
    }

    [TestMethod]
    public void Operation_Compare()
    {
      ScriptExpression expr;
      Random r = new Random();

      List<Pair<string, Val>> test = new List<Pair<string, Val>>();

      for (int i = 0; i < 8; i++)
      {
        int i0 = r.Next(-10000, 10000);
        int i1 = r.Next(-10000, 10000);
        float f0 = ((float)r.NextDouble() - 0.5f) * 20000;
        float f1 = ((float)r.NextDouble() - 0.5f) * 20000;

        test.Add(new Pair<string, Val>("{0} == {1}".F(i0, i0), new Val(i0 == i0)));
        test.Add(new Pair<string, Val>("{0} == {1}".F(i0, i1), new Val(i0 == i1)));
        test.Add(new Pair<string, Val>("{0} != {1}".F(i1, i1), new Val(i1 != i1)));
        test.Add(new Pair<string, Val>("{0} != {1}".F(i0, i1), new Val(i0 != i1)));

        test.Add(new Pair<string, Val>("{0} > {1}".F(i0, i1), new Val(i0 > i1)));
        test.Add(new Pair<string, Val>("{0} < {1}".F(i0, i1), new Val(i0 < i1)));
        test.Add(new Pair<string, Val>("{0} >= {1}".F(i0, i1), new Val(i0 >= i1)));
        test.Add(new Pair<string, Val>("{0} <= {1}".F(i0, i1), new Val(i0 <= i1)));
        test.Add(new Pair<string, Val>("{0} >= {1}".F(i0, i0), new Val(i0 >= i0)));
        test.Add(new Pair<string, Val>("{0} <= {1}".F(i1, i1), new Val(i1 <= i1)));

        test.Add(new Pair<string, Val>("{0} == {1}".F(f0, f0), new Val(f0 == f0)));
        test.Add(new Pair<string, Val>("{0} == {1}".F(f0, f1), new Val(f0 == f1)));
        test.Add(new Pair<string, Val>("{0} != {1}".F(f1, f1), new Val(f1 != f1)));
        test.Add(new Pair<string, Val>("{0} != {1}".F(f0, f1), new Val(f0 != f1)));

        test.Add(new Pair<string, Val>("{0} > {1}".F(f0, f1), new Val(f0 > f1)));
        test.Add(new Pair<string, Val>("{0} < {1}".F(f0, f1), new Val(f0 < f1)));
        test.Add(new Pair<string, Val>("{0} >= {1}".F(f0, f1), new Val(f0 >= f1)));
        test.Add(new Pair<string, Val>("{0} <= {1}".F(f0, f1), new Val(f0 <= f1)));
        test.Add(new Pair<string, Val>("{0} >= {1}".F(f0, f0), new Val(f0 >= f0)));
        test.Add(new Pair<string, Val>("{0} <= {1}".F(f1, f1), new Val(f1 <= f1)));
      }

      foreach (Pair<string, Val> p in test)
      {
        expr = new ScriptExpression(p.t);
        Console.Write("{0} = {1}".F(p.t, p.u.Value));
        Assert.AreEqual(p.u, expr.Evaluate(null));
        Console.WriteLine(" ... OK!");
      }
    }

    [TestMethod]
    public void Operation_Branch()
    {
      Script script;
      ScriptExpression expr;
      Random r = new Random();

      List<Pair<string, Val>> test = new List<Pair<string, Val>>();

      for (int i = 0; i < 8; i++)
      {
        bool b1 = i % 2 == 1;
        bool b2 = i % 4 >= 2;
        bool b3 = i >= 4;

        int i1 = r.Next(-10000, 10000);
        int i2 = r.Next(-10000, 10000);

        test.Add(new Pair<string, Val>("x = {2}; if ({0}) x = {1};".F(b1, i1, i2), new Val(b1 ? i1 : i2)));
        test.Add(new Pair<string, Val>("x = {2}; if ({0}) {{ x = {1}; }}".F(b1, i1, i2), new Val(b1 ? i1 : i2)));
        test.Add(new Pair<string, Val>("if ({0}) x = {1}; else x = {2};".F(b1, i1, i2), new Val(b1 ? i1 : i2)));
        test.Add(new Pair<string, Val>("if ({0}) {{ x = {1}; }} else {{ x = {2};}}".F(b1, i1, i2), new Val(b1 ? i1 : i2)));

        test.Add(new Pair<string, Val>("if ({0}) if ({1}) x = {2}; else x = -({2}); else x = {3};".F(b1, b2, i1, i2), new Val(b1 ? (b2 ? i1 : -i1) : i2)));
      }

      foreach (Pair<string, Val> p in test)
      {
        int n = 0;
        script = new Script("test");
        Context c = new Context();
        script.AddStatements("int x;", ref n);
        script.AddStatements(p.t, ref n);
        Console.Write("{0}".F(p.t));

        script.Run(c);
        expr = new ScriptExpression("x");
        Val res = script.Scope.GetVar(null, "x");

        Console.Write(" [Expect x = {0}, Actual: {1}]".F(p.u.Value, res.Value));
        Assert.AreEqual(p.u.Value, res.Value);

        Console.WriteLine(" ... OK!");
      }
    }
  }
}
