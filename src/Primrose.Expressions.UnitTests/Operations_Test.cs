using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Primrose.Primitives.Extensions;
using System.Globalization;
using Primrose.Primitives.ValueTypes;
using System.Collections.Generic;

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

      // logical OR
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
  }
}
