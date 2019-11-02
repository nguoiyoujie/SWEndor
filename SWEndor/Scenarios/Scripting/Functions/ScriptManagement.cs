using SWEndor.Scenarios.Scripting.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class ScriptManagement
  {
    public static Val CallScript(Context context, Val[] ps)
    {
      Script scr = Script.Registry.Get((string)ps[0]);
      if (scr != null)
      {
        scr.Run(context);
        return Val.TRUE;
      }
      return Val.FALSE;
    }

    public static Val GetArrayElement(Context context, Val[] ps)
    {
      if (ps[0].Type == ValType.BOOL_ARRAY)
        return new Val(((bool[])ps[0])[(int)ps[1]]);

      if (ps[0].Type == ValType.INT_ARRAY)
        return new Val (((int[])ps[0])[(int)ps[1]]);

      if (ps[0].Type == ValType.FLOAT_ARRAY)
        return new Val(((float[])ps[0])[(int)ps[1]]);

      return Val.NULL;
    }
  }
}
