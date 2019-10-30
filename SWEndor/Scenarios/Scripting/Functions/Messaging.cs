using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class Messaging
  {
    public static Val MessageText(Context context, params Val[] ps)
    {
      string text = ps[0].vS;
      float expiretime = ps[1].vI;
      COLOR color = new COLOR(ps[2].vF, ps[3].vF, ps[4].vF, ps[5].vF);

      if (ps.Length <= 6)
        context.Engine.Screen2D.MessageText(text, expiretime, color);
      else
        context.Engine.Screen2D.MessageText(text, expiretime, color, ps[6].vI);
      return Val.TRUE;
    }
  }
}
