using SWEndor.Actors;
using SWEndor.Scenarios.Scripting.Expressions;
using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class GameStateManagement
  {
    public static Val GetGameStateB(Context context, Val[] ps)
    {
      if (ps.Length == 1)
        return new Val(context.Engine.GameScenarioManager.GetGameStateB(ps[0].vS));
      else
        return new Val(context.Engine.GameScenarioManager.GetGameStateB(ps[0].vS, ps[1].vB));
    }

    public static Val SetGameStateB(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.SetGameStateB(ps[0].vS, ps[1].vB);
      return Val.TRUE;
    }

    public static Val GetGameStateF(Context context, Val[] ps)
    {
      if (ps.Length == 1)
        return new Val(context.Engine.GameScenarioManager.GetGameStateF(ps[0].vS));
      else
        return new Val(context.Engine.GameScenarioManager.GetGameStateF(ps[0].vS, ps[1].vF));
    }

    public static Val SetGameStateF(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.SetGameStateF(ps[0].vS, ps[1].vF);
      return Val.TRUE;
    }

    public static Val GetGameStateS(Context context, Val[] ps)
    {
      if (ps.Length == 1)
        return new Val(context.Engine.GameScenarioManager.GetGameStateS(ps[0].vS));
      else
        return new Val(context.Engine.GameScenarioManager.GetGameStateS(ps[0].vS, ps[1].vS));
    }

    public static Val SetGameStateS(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.SetGameStateS(ps[0].vS, ps[1].vS);
      return Val.TRUE;
    }

    public static Val GetGameTime(Context context, Val[] ps)
    {
      return new Val(context.Engine.Game.GameTime);
    }

    public static Val GetLastFrameTime(Context context, Val[] ps)
    {
      return new Val(context.Engine.Game.TimeSinceRender);
    }

    public static Val GetDifficulty(Context context, Val[] ps)
    {
      if (context.Engine.GameScenarioManager.Scenario == null)
        return new Val(0);

      return new Val(context.Engine.GameScenarioManager.Scenario.Difficulty);
    }

    public static Val GetPlayerActorType(Context context, Val[] ps)
    {
      return new Val(context.Engine.PlayerInfo.ActorType.ID);
    }

    public static Val GetStageNumber(Context context, Val[] ps)
    {
      if (context.Engine.GameScenarioManager.Scenario == null)
        return new Val(0);

      return new Val(context.Engine.GameScenarioManager.Scenario.StageNumber);
    }

    public static Val SetStageNumber(Context context, Val[] ps)
    {
      if (context.Engine.GameScenarioManager.Scenario == null)
        return Val.FALSE;

      context.Engine.GameScenarioManager.Scenario.StageNumber = ps[0].vI;
      return Val.TRUE;
    }

    public static Val GetRegisterCount(Context context, Val[] ps)
    {
      if (context.Engine.GameScenarioManager.Scenario == null)
        return new Val(0);

      HashSet<ActorInfo> reg = context.Engine.GameScenarioManager.Scenario.GetRegister(ps[0].vS);
      if (reg == null)
        return new Val(0);
      return new Val(reg.Count);
    }

    public static Val GetTimeSinceLostWing(Context context, Val[] ps)
    {
      if (context.Engine.GameScenarioManager.Scenario == null)
        return new Val(0);

      return new Val(context.Engine.GameScenarioManager.Scenario.TimeSinceLostWing);
    }

    public static Val GetTimeSinceLostShip(Context context, Val[] ps)
    {
      if (context.Engine.GameScenarioManager.Scenario == null)
        return new Val(0);

      return new Val(context.Engine.GameScenarioManager.Scenario.TimeSinceLostShip);
    }

    public static Val AddEvent(Context context, Val[] ps)
    {
      Script s = Script.Registry.Get(ps[1].vS);
      if (s != null)
        context.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + ps[0].vF
        , () => s.Run(context));
      else
      {
        // core events // implement this elsewhere
        GameEvent g = null;
        switch (ps[1].vS.ToLower())
        {
          case "common.fadein":
            g = context.Engine.GameScenarioManager.Scenario.FadeIn;
            break;

          case "common.fadeout":
            g = context.Engine.GameScenarioManager.Scenario.FadeOut;
            break;
        }
        if (g != null)
          context.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + ps[0].vF, g);
        else
          throw new InvalidOperationException(string.Format("Script event '{0}' does not exist!", ps[1].vS));
      }

      return Val.TRUE;
    }
  }
}
