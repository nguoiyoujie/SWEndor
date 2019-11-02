using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class SceneManagement
  {
    public static Val SetMood(Context context, params Val[] ps)
    {
      if (context.Engine.GameScenarioManager.Scenario == null)
        return Val.FALSE;

      context.Engine.GameScenarioManager.Scenario.Mood = (MoodStates)(int)ps[0];
      return Val.TRUE;
    }

    public static Val SetMaxBounds(Context context, params Val[] ps)
    {
      context.Engine.GameScenarioManager.MaxBounds = new TV_3DVECTOR((float)ps[0], (float)ps[1], (float)ps[2]);
      return Val.TRUE;
    }

    public static Val SetMinBounds(Context context, params Val[] ps)
    {
      context.Engine.GameScenarioManager.MinBounds = new TV_3DVECTOR((float)ps[0], (float)ps[1], (float)ps[2]);
      return Val.TRUE;
    }

    public static Val SetMaxAIBounds(Context context, params Val[] ps)
    {
      context.Engine.GameScenarioManager.MaxAIBounds = new TV_3DVECTOR((float)ps[0], (float)ps[1], (float)ps[2]);
      return Val.TRUE;
    }

    public static Val SetMinAIBounds(Context context, params Val[] ps)
    {
      context.Engine.GameScenarioManager.MinAIBounds = new TV_3DVECTOR((float)ps[0], (float)ps[1], (float)ps[2]);
      return Val.TRUE;
    }

    public static Val FadeOut(Context context, params Val[] ps)
    {
      if (context.Engine.GameScenarioManager.Scenario == null)
        return Val.FALSE;

      context.Engine.GameScenarioManager.Scenario.FadeOut();
      return Val.TRUE;
    }
  }
}
