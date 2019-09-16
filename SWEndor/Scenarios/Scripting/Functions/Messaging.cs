using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class Messaging
  {
    public static Val MessageText(Context context, params Val[] ps)
    {
      string text = ps[0].ValueS;
      float expiretime = ps[1].ValueI;
      TV_COLOR color = new TV_COLOR(ps[2].ValueF, ps[3].ValueF, ps[4].ValueF, ps[5].ValueF);

      if (ps.Length <= 6)
        Globals.Engine.Screen2D.MessageText(text, expiretime, color);
      else
        Globals.Engine.Screen2D.MessageText(text, expiretime, color, ps[6].ValueI);
      return Val.TRUE;
    }
  }
}
