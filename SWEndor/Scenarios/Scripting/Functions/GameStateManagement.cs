using MTV3D65;
using SWEndor.Actors;
using SWEndor.Player;
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
        return GameScenarioManager.Instance().GetGameStateB(ps[0].ToString());
      else
        return GameScenarioManager.Instance().GetGameStateB(ps[0].ToString(), Convert.ToBoolean(ps[1].ToString()));
    }

    public static object SetGameStateB(Context context, object[] ps)
    {
      GameScenarioManager.Instance().SetGameStateB(ps[0].ToString(), Convert.ToBoolean(ps[1].ToString()));
      return true;
    }

    public static object GetGameStateF(Context context, object[] ps)
    {
      if (ps.Length == 1)
        return GameScenarioManager.Instance().GetGameStateF(ps[0].ToString());
      else
        return GameScenarioManager.Instance().GetGameStateF(ps[0].ToString(), Convert.ToSingle(ps[1].ToString()));
    }

    public static object SetGameStateF(Context context, object[] ps)
    {
      GameScenarioManager.Instance().SetGameStateF(ps[0].ToString(), Convert.ToSingle(ps[1].ToString()));
      return true;
    }

    public static object GetGameStateS(Context context, object[] ps)
    {
      if (ps.Length == 1)
        return GameScenarioManager.Instance().GetGameStateS(ps[0].ToString());
      else
        return GameScenarioManager.Instance().GetGameStateS(ps[0].ToString(), ps[1].ToString());
    }

    public static object SetGameStateS(Context context, object[] ps)
    {
      GameScenarioManager.Instance().SetGameStateS(ps[0].ToString(), ps[1].ToString());
      return true;
    }

    public static object GetGameTime(Context context, object[] ps)
    {
      return Game.Instance().GameTime;
    }

    public static object GetLastFrameTime(Context context, object[] ps)
    {
      return Game.Instance().TimeSinceRender;
    }

    public static object GetDifficulty(Context context, object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null)
        return 0;

      return GameScenarioManager.Instance().Scenario.Difficulty;
    }

    public static object GetPlayerActorType(Context context, object[] ps)
    {
      return PlayerInfo.Instance().ActorType.Name;
    }

    public static object GetStageNumber(Context context, object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null)
        return 0;

      return GameScenarioManager.Instance().Scenario.StageNumber;
    }

    public static object SetStageNumber(Context context, object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null)
        return false;

      GameScenarioManager.Instance().Scenario.StageNumber = Convert.ToInt32(ps[0].ToString());
      return true;
    }

    public static object GetRegisterCount(Context context, object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null)
        return 0;

      Dictionary<string, ActorInfo> reg = GameScenarioManager.Instance().Scenario.GetRegister(ps[0].ToString());
      if (reg == null)
        return 0;
      return reg.Count;
    }

    public static object GetTimeSinceLostWing(Context context, object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null)
        return 0;

      return GameScenarioManager.Instance().Scenario.TimeSinceLostWing;
    }

    public static object GetTimeSinceLostShip(Context context, object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null)
        return 0;

      return GameScenarioManager.Instance().Scenario.TimeSinceLostShip;
    }

    public static object AddEvent(Context context, object[] ps)
    {
      Script s = Script.Registry.Get(ps[1].ToString());
      if (s != null)
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + Convert.ToSingle(ps[0].ToString())
        , (_) => s.Run());
      else
      {
        // core events // implement this elsewhere
        GameEvent g = null;
        switch (ps[1].ToString().ToLower())
        {
          case "common.fadein":
            g = GameScenarioManager.Instance().Scenario.FadeIn;
            break;

          case "common.fadeout":
            g = GameScenarioManager.Instance().Scenario.FadeOut;
            break;
        }
        if (g != null)
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + Convert.ToInt32(ps[0].ToString()), g);
        else
          throw new InvalidOperationException(string.Format("Script event '{0}' does not exist!", ps[1].ToString()));
      }

      return true;
    }
  }
}
