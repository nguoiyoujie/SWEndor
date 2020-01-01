using Primrose.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting
{
  public class ValFunc<T> : IValFunc
  {
    Func<Context, T, Val> F;
    public ValFunc(Func<Context, T, Val> fn) { F = fn; }

    public Val Execute(ITracker caller, string _funcName, Context c, Val[] p)
    {
      return F(c, ValParamContract.Convert<T>(caller, _funcName, 1, p[0]));
    }
  }
}
