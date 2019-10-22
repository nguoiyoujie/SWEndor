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
      COLOR color = new COLOR(ps[2].ValueF, ps[3].ValueF, ps[4].ValueF, ps[5].ValueF);

      if (ps.Length <= 6)
        context.Engine.Screen2D.MessageText(text, expiretime, color);
      else
        context.Engine.Screen2D.MessageText(text, expiretime, color, ps[6].ValueI);
      return Val.TRUE;
    }
  }
}
