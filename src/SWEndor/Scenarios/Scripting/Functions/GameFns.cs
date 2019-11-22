﻿using Primrose.Expressions;
using SWEndor.Actors;
using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class GameFns
  {
    public static Val GetGameStateB(Context context, string name) { return new Val(context.Engine.GameScenarioManager.Scenario.State.GetGameStateB(name)); }
    public static Val GetGameStateB(Context context, string name, bool defaultVal) { return new Val(context.Engine.GameScenarioManager.Scenario.State.GetGameStateB(name, defaultVal)); }
    public static Val SetGameStateB(Context context, string name, bool value) { context.Engine.GameScenarioManager.Scenario.State.SetGameStateB(name, value);  return Val.NULL; }

    public static Val GetGameStateF(Context context, string name) { return new Val(context.Engine.GameScenarioManager.Scenario.State.GetGameStateF(name)); }
    public static Val GetGameStateF(Context context, string name, float defaultVal) { return new Val(context.Engine.GameScenarioManager.Scenario.State.GetGameStateF(name, defaultVal)); }
    public static Val SetGameStateF(Context context, string name, float value) { context.Engine.GameScenarioManager.Scenario.State.SetGameStateF(name, value); return Val.NULL; }

    public static Val GetGameStateS(Context context, string name) { return new Val(context.Engine.GameScenarioManager.Scenario.State.GetGameStateS(name)); }
    public static Val GetGameStateS(Context context, string name, string defaultVal) { return new Val(context.Engine.GameScenarioManager.Scenario.State.GetGameStateS(name, defaultVal)); }
    public static Val SetGameStateS(Context context, string name, string value) { context.Engine.GameScenarioManager.Scenario.State.SetGameStateS(name, value); return Val.NULL; }

    public static Val GetGameTime(Context context) { return new Val(context.Engine.Game.GameTime); }
    public static Val GetLastFrameTime(Context context) { return new Val(context.Engine.Game.TimeSinceRender); }
    public static Val GetDifficulty(Context context) { return new Val(context.Engine.GameScenarioManager.Scenario.State.Difficulty); }

    public static Val GetPlayerActorType(Context context) { return new Val(context.Engine.PlayerInfo.ActorType.ID); }
    public static Val GetPlayerName(Context context) { return new Val(context.Engine.PlayerInfo.Name); }

    public static Val GetStageNumber(Context context) { return new Val(context.Engine.GameScenarioManager.Scenario.State.StageNumber); }
    public static Val SetStageNumber(Context context, int value) { context.Engine.GameScenarioManager.Scenario.State.StageNumber = value; return Val.NULL; }

    public static Val GetRegisterCount(Context context, string register)
    {
      HashSet<ActorInfo> reg = context.Engine.GameScenarioManager.Scenario.GetRegister(register);
      if (reg == null)
        return new Val(0);
      return new Val(reg.Count);
    }

    public static Val GetTimeSinceLostWing(Context context) { return new Val(context.Engine.GameScenarioManager.Scenario.State.TimeSinceLostWing); }
    public static Val GetTimeSinceLostShip(Context context) { return new Val(context.Engine.GameScenarioManager.Scenario.State.TimeSinceLostShip); }
    public static Val GetTimeSinceLostStructure(Context context) { return new Val(context.Engine.GameScenarioManager.Scenario.State.TimeSinceLostStructure); }

    public static Val AddEvent(Context context, float time, string eventName)
    {
      Script script = Script.Registry.Get(eventName);
      if (script != null)
        context.Engine.GameScenarioManager.Scenario.EventQueue.Add(context.Engine.Game.GameTime + time
        , (c, s) => s.Run(c), context, script);
      else
      {
        // core events // implement this elsewhere
        GameEvent g = null;
        switch (eventName.ToLower())
        {
          case "common.fadein":
            g = context.Engine.GameScenarioManager.Scenario.FadeIn;
            break;

          case "common.fadeout":
            g = context.Engine.GameScenarioManager.Scenario.FadeOut;
            break;
        }
        if (g != null)
          context.Engine.GameScenarioManager.Scenario.EventQueue.Add(context.Engine.Game.GameTime + time, g);
        else
          throw new InvalidOperationException(string.Format("Script event '{0}' does not exist!", eventName));
      }

      return Val.TRUE;
    }
  }
}
