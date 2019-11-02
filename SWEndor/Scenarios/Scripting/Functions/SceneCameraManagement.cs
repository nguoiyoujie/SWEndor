using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class SceneCameraManagement
  {
    public static Val SetSceneCameraAsActive(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.MaxBounds = new TV_3DVECTOR((float)ps[0], (float)ps[1], (float)ps[2]);
      return Val.TRUE;
    }
  }
}
