using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class SceneCameraManagement
  {
    public static Val SetSceneCameraAsActive(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.MaxBounds = new TV_3DVECTOR(ps[0].ValueF, ps[1].ValueF, ps[2].ValueF);
      return Val.TRUE;
    }
  }
}
