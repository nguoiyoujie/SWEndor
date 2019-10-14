using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class UIManagement
  {
    public static Val SetUILine1Color(Context context, Val[] ps)
    {
      int color = new TV_COLOR(ps[0].ValueF, ps[1].ValueF, ps[2].ValueF, 1).GetIntColor();
      context.Engine.GameScenarioManager.Line1Color = color;
      return Val.TRUE;
    }

    public static Val SetUILine2Color(Context context, Val[] ps)
    {
      int color = new TV_COLOR(ps[0].ValueF, ps[1].ValueF, ps[2].ValueF, 1).GetIntColor();
      context.Engine.GameScenarioManager.Line2Color = color;
      return Val.TRUE;
    }

    public static Val SetUILine3Color(Context context, Val[] ps)
    {
      int color = new TV_COLOR(ps[0].ValueF, ps[1].ValueF, ps[2].ValueF, 1).GetIntColor();
      context.Engine.GameScenarioManager.Line3Color = color;
      return Val.TRUE;
    }

    public static Val SetUILine1Text(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.Line1Text = ps[0].ValueS;
      return Val.TRUE;
    }

    public static Val SetUILine2Text(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.Line2Text = ps[0].ValueS;
      return Val.TRUE;
    }

    public static Val SetUILine3Text(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.Line3Text = ps[0].ValueS;
      return Val.TRUE;
    }
  }
}
