using SWEndor.Scenarios.Scripting.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class ScriptManagement
  {
    public static Val CallScript(Context context, Val[] ps)
    {
      Script scr = Script.Registry.Get(ps[0].ValueS);
      if (scr != null)
      {
        scr.Run(context);
        return Val.TRUE;
      }
      return Val.FALSE;
    }

    public static Val GetArrayElement(Context context, Val[] ps)
    {
      if (ps[0].Type == ValType.INT_ARRAY)
        return new Val (ps[0].ArrayI[ps[1].ValueI]);

      if (ps[0].Type == ValType.FLOAT_ARRAY)
        return new Val(ps[0].ArrayF[ps[1].ValueI]);

      return Val.NULL;
    }
  }
}
