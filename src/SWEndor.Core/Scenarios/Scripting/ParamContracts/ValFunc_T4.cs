using Primrose.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting
{
  public class ValFunc<T1, T2, T3, T4> : IValFunc
  {
    Func<Context, T1, T2, T3, T4, Val> F;
    public ValFunc(Func<Context, T1, T2, T3, T4, Val> fn) { F = fn; }

    public Val Execute(ITracker caller, string _funcName, Context c, Val[] p)
    {
      return F(c
        , ValParamContract.Convert<T1>(caller, _funcName, 1, p[0])
        , ValParamContract.Convert<T2>(caller, _funcName, 2, p[1])
        , ValParamContract.Convert<T3>(caller, _funcName, 3, p[2])
        , ValParamContract.Convert<T4>(caller, _funcName, 4, p[3])
        );
    }
  }
}
