using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class UIManagement
  {
    public static Val SetUILine1Color(Context context, Val[] ps)
    {
      COLOR color = new COLOR((float)ps[0], (float)ps[1], (float)ps[2], 1);
      context.Engine.GameScenarioManager.Line1Color = color;
      return Val.TRUE;
    }

    public static Val SetUILine2Color(Context context, Val[] ps)
    {
      COLOR color = new COLOR((float)ps[0], (float)ps[1], (float)ps[2], 1);
      context.Engine.GameScenarioManager.Line2Color = color;
      return Val.TRUE;
    }

    public static Val SetUILine3Color(Context context, Val[] ps)
    {
      COLOR color = new COLOR((float)ps[0], (float)ps[1], (float)ps[2], 1);
      context.Engine.GameScenarioManager.Line3Color = color;
      return Val.TRUE;
    }

    public static Val SetUILine1Text(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.Line1Text = (string)ps[0];
      return Val.TRUE;
    }

    public static Val SetUILine2Text(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.Line2Text = (string)ps[0];
      return Val.TRUE;
    }

    public static Val SetUILine3Text(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.Line3Text = (string)ps[0];
      return Val.TRUE;
    }
  }
}
