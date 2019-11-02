using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class Messaging
  {
    public static Val MessageText(Context context, params Val[] ps)
    {
      string text = (string)ps[0];
      float expiretime = (int)ps[1];
      COLOR color = new COLOR((float)ps[2], (float)ps[3], (float)ps[4], (float)ps[5]);

      if (ps.Length <= 6)
        context.Engine.Screen2D.MessageText(text, expiretime, color);
      else
        context.Engine.Screen2D.MessageText(text, expiretime, color, (int)ps[6]);
      return Val.TRUE;
    }
  }
}
