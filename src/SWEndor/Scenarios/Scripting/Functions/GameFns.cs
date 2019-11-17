using Primrose.Expressions;
using SWEndor.Actors;
using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class GameFns
  {
    public static Val GetGameStateB(Context context, string name) { return new Val(context.Engine.GameScenarioManager.GetGameStateB(name)); }
    public static Val GetGameStateB(Context context, string name, bool defaultVal) { return new Val(context.Engine.GameScenarioManager.GetGameStateB(name, defaultVal)); }
    public static Val SetGameStateB(Context context, string name, bool value) { context.Engine.GameScenarioManager.SetGameStateB(name, value);  return Val.NULL; }

    public static Val GetGameStateF(Context context, string name) { return new Val(context.Engine.GameScenarioManager.GetGameStateF(name)); }
    public static Val GetGameStateF(Context context, string name, float defaultVal) { return new Val(context.Engine.GameScenarioManager.GetGameStateF(name, defaultVal)); }
    public static Val SetGameStateF(Context context, string name, float value) { context.Engine.GameScenarioManager.SetGameStateF(name, value); return Val.NULL; }

    public static Val GetGameStateS(Context context, string name) { return new Val(context.Engine.GameScenarioManager.GetGameStateS(name)); }
    public static Val GetGameStateS(Context context, string name, string defaultVal) { return new Val(context.Engine.GameScenarioManager.GetGameStateS(name, defaultVal)); }
    public static Val SetGameStateS(Context context, string name, string value) { context.Engine.GameScenarioManager.SetGameStateS(name, value); return Val.NULL; }

    public static Val GetGameTime(Context context) { return new Val(context.Engine.Game.GameTime); }
    public static Val GetLastFrameTime(Context context) { return new Val(context.Engine.Game.TimeSinceRender); }
    public static Val GetDifficulty(Context context) { return new Val(context.Engine.GameScenarioManager.Scenario.Difficulty); }

    public static Val GetPlayerActorType(Context context) { return new Val(context.Engine.PlayerInfo.ActorType.ID); }
    public static Val GetPlayerName(Context context) { return new Val(context.Engine.PlayerInfo.Name); }

    public static Val GetStageNumber(Context context) { return new Val(context.Engine.GameScenarioManager.Scenario.StageNumber); }
    public static Val SetStageNumber(Context context, int value) { context.Engine.GameScenarioManager.Scenario.StageNumber = value; return Val.NULL; }

    public static Val GetRegisterCount(Context context, string register)
    {
      HashSet<ActorInfo> reg = context.Engine.GameScenarioManager.Scenario.GetRegister((string)register);
      if (reg == null)
        return new Val(0);
      return new Val(reg.Count);
    }

    public static Val GetTimeSinceLostWing(Context context) { return new Val(context.Engine.GameScenarioManager.Scenario.TimeSinceLostWing); }
    public static Val GetTimeSinceLostShip(Context context) { return new Val(context.Engine.GameScenarioManager.Scenario.TimeSinceLostShip); }
    public static Val GetTimeSinceLostStructure(Context context) { return new Val(context.Engine.GameScenarioManager.Scenario.TimeSinceLostStructure); }

    public static Val AddEvent(Context context, float time, string eventName)
    {
      Script script = Script.Registry.Get(eventName);
      if (script != null)
        context.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + time
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
          context.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + time, g);
        else
          throw new InvalidOperationException(string.Format("Script event '{0}' does not exist!", eventName));
      }

      return Val.TRUE;
    }
  }
}
