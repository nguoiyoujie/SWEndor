using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class SceneCameraManagement
  {
    public static Val SetSceneCameraAsActive(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.MaxBounds = new TV_3DVECTOR(ps[0].vF, ps[1].vF, ps[2].vF);
      return Val.TRUE;
    }
  }
}
