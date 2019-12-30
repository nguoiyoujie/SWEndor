using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.ValueTypes;
using System.Collections.Generic;
using Primrose.Expressions.UnitTests.Scripting;
using System.IO;

namespace Primrose.Expressions.UnitTests
{
  [TestClass]
  public class Script_Test
  {
    [TestMethod]
    public void Script_Simulation()
    {
      Context c = new Context();
      string test_dir = "./Scripts";

      foreach (string sfile in Directory.GetFiles(test_dir))
      {
        Console.WriteLine("-----------------------------------------------");
        Console.WriteLine("loading script file:".C(Path.GetFileName(sfile)));

        c.Reset();
        Script.Registry.Clear();

        ScriptFile f = new ScriptFile(sfile);
        f.ScriptReadDelegate = ReadScript;
        f.ReadFile();

        Script.Registry.Global.Run(c);

        foreach (Script s in Script.Registry.GetAll())
        {
          Console.WriteLine("running script:".C(s.Name));
          s.Run(c);
          Console.WriteLine("script ".C(s.Name, "... OK!"));
          Console.WriteLine();
        }
        Console.WriteLine("script file complete!");
        Console.WriteLine();
      }
    }

    public void ReadScript(string name)
    {
      Console.Write("loading script:".C(name));
    }
  }
}
