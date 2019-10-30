﻿using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class UIManagement
  {
    public static Val SetUILine1Color(Context context, Val[] ps)
    {
      COLOR color = new COLOR(ps[0].vF, ps[1].vF, ps[2].vF, 1);
      context.Engine.GameScenarioManager.Line1Color = color;
      return Val.TRUE;
    }

    public static Val SetUILine2Color(Context context, Val[] ps)
    {
      COLOR color = new COLOR(ps[0].vF, ps[1].vF, ps[2].vF, 1);
      context.Engine.GameScenarioManager.Line2Color = color;
      return Val.TRUE;
    }

    public static Val SetUILine3Color(Context context, Val[] ps)
    {
      COLOR color = new COLOR(ps[0].vF, ps[1].vF, ps[2].vF, 1);
      context.Engine.GameScenarioManager.Line3Color = color;
      return Val.TRUE;
    }

    public static Val SetUILine1Text(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.Line1Text = ps[0].vS;
      return Val.TRUE;
    }

    public static Val SetUILine2Text(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.Line2Text = ps[0].vS;
      return Val.TRUE;
    }

    public static Val SetUILine3Text(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.Line3Text = ps[0].vS;
      return Val.TRUE;
    }
  }
}
