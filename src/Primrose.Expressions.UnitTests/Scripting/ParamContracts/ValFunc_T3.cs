using Primrose.Expressions;
using System;

namespace Primrose.Expressions.UnitTests.Scripting
{
  public class ValFunc<T1, T2, T3> : IValFunc
  {
    Func<Context, T1, T2, T3, Val> F;
    public ValFunc(Func<Context, T1, T2, T3, Val> fn) { F = fn; }

    public Val Execute(ITracker caller, string _funcName, Context c, Val[] p)
    {
      return F(c
        , ValParamContract.Convert<T1>(caller, _funcName, 1, p[0])
        , ValParamContract.Convert<T2>(caller, _funcName, 2, p[1])
        , ValParamContract.Convert<T3>(caller, _funcName, 3, p[2])
        );
    }
  }
}
