using Primrose.Primitives.Extensions;
using Primrose.Primitives.Factories;
using Primrose.Primitives.ValueTypes;
using System.Collections.Generic;

namespace Primrose.Expressions.UnitTests.Scripting
{
  public delegate Val FunctionDelegate(Context context, Val[] param);

  public class Context : AContext
  {
    public readonly Registry<FunctionDelegate> Functions = new Registry<FunctionDelegate>();

    public readonly Registry<Pair<string, int>, IValFunc> ValFuncs = new Registry<Pair<string, int>, IValFunc>();
    public readonly List<string> ValFuncRef = new List<string>();

    internal Context()
    {
      DefineFunc();
    }

    public override Val RunFunction(ITracker caller, string _funcName, Val[] param)
    {
      FunctionDelegate fd = Functions.Get(_funcName);
      if (fd == null)
      {
        IValFunc vfs = ValFuncs.Get(new Pair<string, int>(_funcName, param.Length));
        if (vfs == null)
          if (ValFuncRef.Contains(_funcName))
            throw new EvalException(caller, "Incorrect number/type of parameters supplied to function '{0}'!".F(_funcName));
          else
            throw new EvalException(caller, "The function '{0}' does not exist!".F(_funcName));

        return vfs.Execute(caller, _funcName, this, param);
      }
      return fd.Invoke(this, param);
    }

    internal void Reset() { }

    private void AddFunc(string name, ValFunc fn) { ValFuncs.Add(new Pair<string, int>(name, 0), fn); if (!ValFuncRef.Contains(name)) ValFuncRef.Add(name); }
    private void AddFunc<T1>(string name, ValFunc<T1> fn) { ValFuncs.Add(new Pair<string, int>(name, 1), fn); if (!ValFuncRef.Contains(name)) ValFuncRef.Add(name); }
    private void AddFunc<T1, T2>(string name, ValFunc<T1, T2> fn) { ValFuncs.Add(new Pair<string, int>(name, 2), fn); if (!ValFuncRef.Contains(name)) ValFuncRef.Add(name); }
    private void AddFunc<T1, T2, T3>(string name, ValFunc<T1, T2, T3> fn) { ValFuncs.Add(new Pair<string, int>(name, 3), fn); if (!ValFuncRef.Contains(name)) ValFuncRef.Add(name); }
    private void AddFunc<T1, T2, T3, T4>(string name, ValFunc<T1, T2, T3, T4> fn) { ValFuncs.Add(new Pair<string, int>(name, 4), fn); if (!ValFuncRef.Contains(name)) ValFuncRef.Add(name); }
    private void AddFunc<T1, T2, T3, T4, T5>(string name, ValFunc<T1, T2, T3, T4, T5> fn) { ValFuncs.Add(new Pair<string, int>(name, 5), fn); if (!ValFuncRef.Contains(name)) ValFuncRef.Add(name); }
    private void AddFunc<T1, T2, T3, T4, T5, T6>(string name, ValFunc<T1, T2, T3, T4, T5, T6> fn) { ValFuncs.Add(new Pair<string, int>(name, 6), fn); if (!ValFuncRef.Contains(name)) ValFuncRef.Add(name); }
    private void AddFunc<T1, T2, T3, T4, T5, T6, T7>(string name, ValFunc<T1, T2, T3, T4, T5, T6, T7> fn) { ValFuncs.Add(new Pair<string, int>(name, 7), fn); if (!ValFuncRef.Contains(name)) ValFuncRef.Add(name); }
    private void AddFunc<T1, T2, T3, T4, T5, T6, T7, T8>(string name, ValFunc<T1, T2, T3, T4, T5, T6, T7, T8> fn) { ValFuncs.Add(new Pair<string, int>(name, 8), fn); if (!ValFuncRef.Contains(name)) ValFuncRef.Add(name); }

    internal void DefineFunc()
    {
      ValFuncs.Clear();
      Functions.Clear();
      ValFuncRef.Clear();

      // Assert
      AddFunc("Assert.AreEqual", new ValFunc<Val, Val>(AssertFns.AreEqual));
      AddFunc("Assert.AreNotEqual", new ValFunc<Val, Val>(AssertFns.AreNotEqual));

      // Console
      AddFunc("Console.Write", new ValFunc<Val>(ConsoleFns.Write));
      AddFunc("Console.WriteLine", new ValFunc<Val>(ConsoleFns.WriteLine));

      // Math
      AddFunc("Math.Int", new ValFunc<float>(MathFns.Int));
      AddFunc("Math.Max", new ValFunc<float, float>(MathFns.Max));
      AddFunc("Math.Min", new ValFunc<float, float>(MathFns.Min));

      // Misc
      AddFunc("IsNull", new ValFunc<Val>(MiscFns.IsNull));
      AddFunc("Random", new ValFunc(MiscFns.Random));
      AddFunc("Random", new ValFunc<int>(MiscFns.Random));
      AddFunc("Random", new ValFunc<int, int>(MiscFns.Random));
      AddFunc("GetArrayElement", new ValFunc<Val, int>(MiscFns.GetArrayElement));
    }
  }
}
