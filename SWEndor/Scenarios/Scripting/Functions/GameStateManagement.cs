using SWEndor.Actors;
using SWEndor.Scenarios.Scripting.Expressions;
using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class GameStateManagement
  {

    public static object GetGameStateB(Context context, object[] ps)
    {
      if (ps.Length == 1)
        return context.Engine.GameScenarioManager.GetGameStateB(ps[0].ToString());
      else
        return context.Engine.GameScenarioManager.GetGameStateB(ps[0].ToString(), Convert.ToBoolean(ps[1].ToString()));
    }

    public static object SetGameStateB(Context context, object[] ps)
    {
      context.Engine.GameScenarioManager.SetGameStateB(ps[0].ToString(), Convert.ToBoolean(ps[1].ToString()));
      return true;
    }

    public static object GetGameStateF(Context context, object[] ps)
    {
      if (ps.Length == 1)
        return context.Engine.GameScenarioManager.GetGameStateF(ps[0].ToString());
      else
        return context.Engine.GameScenarioManager.GetGameStateF(ps[0].ToString(), Convert.ToSingle(ps[1].ToString()));
    }

    public static object SetGameStateF(Context context, object[] ps)
    {
      context.Engine.GameScenarioManager.SetGameStateF(ps[0].ToString(), Convert.ToSingle(ps[1].ToString()));
      return true;
    }

    public static object GetGameStateS(Context context, object[] ps)
    {
      if (ps.Length == 1)
        return context.Engine.GameScenarioManager.GetGameStateS(ps[0].ToString());
      else
        return context.Engine.GameScenarioManager.GetGameStateS(ps[0].ToString(), ps[1].ToString());
    }

    public static object SetGameStateS(Context context, object[] ps)
    {
      context.Engine.GameScenarioManager.SetGameStateS(ps[0].ToString(), ps[1].ToString());
      return true;
    }

    public static object GetGameTime(Context context, object[] ps)
    {
      return context.Engine.Game.GameTime;
    }

    public static object GetLastFrameTime(Context context, object[] ps)
    {
      return context.Engine.Game.TimeSinceRender;
    }

    public static object GetDifficulty(Context context, object[] ps)
    {
      if (context.Engine.GameScenarioManager.Scenario == null)
        return 0;

      return context.Engine.GameScenarioManager.Scenario.Difficulty;
    }

    public static object GetPlayerActorType(Context context, object[] ps)
    {
      return context.Engine.PlayerInfo.ActorType.Name;
    }

    public static object GetStageNumber(Context context, object[] ps)
    {
      if (context.Engine.GameScenarioManager.Scenario == null)
        return 0;

      return context.Engine.GameScenarioManager.Scenario.StageNumber;
    }

    public static object SetStageNumber(Context context, object[] ps)
    {
      if (context.Engine.GameScenarioManager.Scenario == null)
        return false;

      context.Engine.GameScenarioManager.Scenario.StageNumber = Convert.ToInt32(ps[0].ToString());
      return true;
    }

    public static object GetRegisterCount(Context context, object[] ps)
    {
      if (context.Engine.GameScenarioManager.Scenario == null)
        return 0;

      HashSet<ActorInfo> reg = context.Engine.GameScenarioManager.Scenario.GetRegister(ps[0].ToString());
      if (reg == null)
        return 0;
      return reg.Count;
    }

    public static object GetTimeSinceLostWing(Context context, object[] ps)
    {
      if (context.Engine.GameScenarioManager.Scenario == null)
        return 0;

      return context.Engine.GameScenarioManager.Scenario.TimeSinceLostWing;
    }

    public static object GetTimeSinceLostShip(Context context, object[] ps)
    {
      if (context.Engine.GameScenarioManager.Scenario == null)
        return 0;

      return context.Engine.GameScenarioManager.Scenario.TimeSinceLostShip;
    }

    public static object AddEvent(Context context, object[] ps)
    {
      Script s = Script.Registry.Get(ps[1].ToString());
      if (s != null)
        context.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + Convert.ToSingle(ps[0].ToString())
        , (_) => s.Run(context));
      else
      {
        // core events // implement this elsewhere
        GameEvent g = null;
        switch (ps[1].ToString().ToLower())
        {
          case "common.fadein":
            g = context.Engine.GameScenarioManager.Scenario.FadeIn;
            break;

          case "common.fadeout":
            g = context.Engine.GameScenarioManager.Scenario.FadeOut;
            break;
        }
        if (g != null)
          context.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + Convert.ToInt32(ps[0].ToString()), g);
        else
          throw new InvalidOperationException(string.Format("Script event '{0}' does not exist!", ps[1].ToString()));
      }

      return true;
    }
  }
}
