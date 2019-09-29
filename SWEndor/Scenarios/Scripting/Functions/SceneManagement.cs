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

      context.Engine.GameScenarioManager.Scenario.Mood = (MoodStates)ps[0].ValueI;
      return Val.TRUE;
    }

    public static Val SetMaxBounds(Context context, params Val[] ps)
    {
      context.Engine.GameScenarioManager.MaxBounds = new TV_3DVECTOR(ps[0].ValueF, ps[1].ValueF, ps[2].ValueF);
      return Val.TRUE;
    }

    public static Val SetMinBounds(Context context, params Val[] ps)
    {
      context.Engine.GameScenarioManager.MinBounds = new TV_3DVECTOR(ps[0].ValueF, ps[1].ValueF, ps[2].ValueF);
      return Val.TRUE;
    }

    public static Val SetMaxAIBounds(Context context, params Val[] ps)
    {
      context.Engine.GameScenarioManager.MaxAIBounds = new TV_3DVECTOR(ps[0].ValueF, ps[1].ValueF, ps[2].ValueF);
      return Val.TRUE;
    }

    public static Val SetMinAIBounds(Context context, params Val[] ps)
    {
      context.Engine.GameScenarioManager.MinAIBounds = new TV_3DVECTOR(ps[0].ValueF, ps[1].ValueF, ps[2].ValueF);
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
