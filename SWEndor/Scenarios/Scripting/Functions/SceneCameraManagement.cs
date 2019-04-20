using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class SceneCameraManagement
  {
    public static object SetSceneCameraAsActive(Context context, object[] ps)
    {
      GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }
  }
}
