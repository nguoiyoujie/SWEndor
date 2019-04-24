using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class Messaging
  {
    public static object MessageText(Context context, params object[] ps)
    {
      string text = ps[0].ToString();
      float expiretime = Convert.ToSingle(ps[1].ToString());
      TV_COLOR color = new TV_COLOR(Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()), Convert.ToSingle(ps[4].ToString()), Convert.ToSingle(ps[5].ToString()));

      if (ps.Length <= 6)
        Globals.Engine.Screen2D.MessageText(text, expiretime, color);
      else
        Globals.Engine.Screen2D.MessageText(text, expiretime, color, Convert.ToInt32(ps[6].ToString()));
      return true;
    }
  }
}
