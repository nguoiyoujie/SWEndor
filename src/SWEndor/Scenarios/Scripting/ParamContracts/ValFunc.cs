using Primrose.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting
{
  public class ValFunc : IValFunc
  {
    Func<Context, Val> F;
    public ValFunc(Func<Context, Val> fn) { F = fn; }

    public Val Execute(ITracker caller, string _funcName, Context c, Val[] p)
    {
      return F(c);
    }
  }
}
