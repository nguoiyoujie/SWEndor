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
        return Globals.Engine.GameScenarioManager.GetGameStateB(ps[0].ToString());
      else
        return Globals.Engine.GameScenarioManager.GetGameStateB(ps[0].ToString(), Convert.ToBoolean(ps[1].ToString()));
    }

    public static object SetGameStateB(Context context, object[] ps)
    {
      Globals.Engine.GameScenarioManager.SetGameStateB(ps[0].ToString(), Convert.ToBoolean(ps[1].ToString()));
      return true;
    }

    public static object GetGameStateF(Context context, object[] ps)
    {
      if (ps.Length == 1)
        return Globals.Engine.GameScenarioManager.GetGameStateF(ps[0].ToString());
      else
        return Globals.Engine.GameScenarioManager.GetGameStateF(ps[0].ToString(), Convert.ToSingle(ps[1].ToString()));
    }

    public static object SetGameStateF(Context context, object[] ps)
    {
      Globals.Engine.GameScenarioManager.SetGameStateF(ps[0].ToString(), Convert.ToSingle(ps[1].ToString()));
      return true;
    }

    public static object GetGameStateS(Context context, object[] ps)
    {
      if (ps.Length == 1)
        return Globals.Engine.GameScenarioManager.GetGameStateS(ps[0].ToString());
      else
        return Globals.Engine.GameScenarioManager.GetGameStateS(ps[0].ToString(), ps[1].ToString());
    }

    public static object SetGameStateS(Context context, object[] ps)
    {
      Globals.Engine.GameScenarioManager.SetGameStateS(ps[0].ToString(), ps[1].ToString());
      return true;
    }

    public static object GetGameTime(Context context, object[] ps)
    {
      return Globals.Engine.Game.GameTime;
    }

    public static object GetLastFrameTime(Context context, object[] ps)
    {
      return Globals.Engine.Game.TimeSinceRender;
    }

    public static object GetDifficulty(Context context, object[] ps)
    {
      if (Globals.Engine.GameScenarioManager.Scenario == null)
        return 0;

      return Globals.Engine.GameScenarioManager.Scenario.Difficulty;
    }

    public static object GetPlayerActorType(Context context, object[] ps)
    {
      return Globals.Engine.PlayerInfo.ActorType.Name;
    }

    public static object GetStageNumber(Context context, object[] ps)
    {
      if (Globals.Engine.GameScenarioManager.Scenario == null)
        return 0;

      return Globals.Engine.GameScenarioManager.Scenario.StageNumber;
    }

    public static object SetStageNumber(Context context, object[] ps)
    {
      if (Globals.Engine.GameScenarioManager.Scenario == null)
        return false;

      Globals.Engine.GameScenarioManager.Scenario.StageNumber = Convert.ToInt32(ps[0].ToString());
      return true;
    }

    public static object GetRegisterCount(Context context, object[] ps)
    {
      if (Globals.Engine.GameScenarioManager.Scenario == null)
        return 0;

      Dictionary<string, ActorInfo> reg = Globals.Engine.GameScenarioManager.Scenario.GetRegister(ps[0].ToString());
      if (reg == null)
        return 0;
      return reg.Count;
    }

    public static object GetTimeSinceLostWing(Context context, object[] ps)
    {
      if (Globals.Engine.GameScenarioManager.Scenario == null)
        return 0;

      return Globals.Engine.GameScenarioManager.Scenario.TimeSinceLostWing;
    }

    public static object GetTimeSinceLostShip(Context context, object[] ps)
    {
      if (Globals.Engine.GameScenarioManager.Scenario == null)
        return 0;

      return Globals.Engine.GameScenarioManager.Scenario.TimeSinceLostShip;
    }

    public static object AddEvent(Context context, object[] ps)
    {
      Script s = Script.Registry.Get(ps[1].ToString());
      if (s != null)
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + Convert.ToSingle(ps[0].ToString())
        , (_) => s.Run(context));
      else
      {
        // core events // implement this elsewhere
        GameEvent g = null;
        switch (ps[1].ToString().ToLower())
        {
          case "common.fadein":
            g = Globals.Engine.GameScenarioManager.Scenario.FadeIn;
            break;

          case "common.fadeout":
            g = Globals.Engine.GameScenarioManager.Scenario.FadeOut;
            break;
        }
        if (g != null)
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + Convert.ToInt32(ps[0].ToString()), g);
        else
          throw new InvalidOperationException(string.Format("Script event '{0}' does not exist!", ps[1].ToString()));
      }

      return true;
    }
  }
}
