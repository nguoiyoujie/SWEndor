using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class SceneManagement
  {
    public static object SetMaxBounds(Context context, params object[] ps)
    {
      context.Engine.GameScenarioManager.MaxBounds = new TV_3DVECTOR(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }

    public static object SetMinBounds(Context context, params object[] ps)
    {
      context.Engine.GameScenarioManager.MinBounds = new TV_3DVECTOR(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }

    public static object SetMaxAIBounds(Context context, params object[] ps)
    {
      context.Engine.GameScenarioManager.MaxAIBounds = new TV_3DVECTOR(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }

    public static object SetMinAIBounds(Context context, params object[] ps)
    {
      context.Engine.GameScenarioManager.MinAIBounds = new TV_3DVECTOR(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }

    public static object FadeOut(Context context, params object[] ps)
    {
      if (context.Engine.GameScenarioManager.Scenario == null)
        return false;

      context.Engine.GameScenarioManager.Scenario.FadeOut();
      return true;
    }

  }
}
