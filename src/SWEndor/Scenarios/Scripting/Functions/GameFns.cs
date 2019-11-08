﻿using Primrose.Expressions;
using SWEndor.Actors;
using SWEndor.Scenarios.Scripting.Expressions;
using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class GameFns
  {
    public static Val GetGameStateB(Context context, Val[] ps)
    {
      if (ps.Length == 1)
        return new Val(context.Engine.GameScenarioManager.GetGameStateB((string)ps[0]));
      else
        return new Val(context.Engine.GameScenarioManager.GetGameStateB((string)ps[0], (bool)ps[1]));
    }

    public static Val SetGameStateB(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.SetGameStateB((string)ps[0], (bool)ps[1]);
      return Val.TRUE;
    }

    public static Val GetGameStateF(Context context, Val[] ps)
    {
      if (ps.Length == 1)
        return new Val(context.Engine.GameScenarioManager.GetGameStateF((string)ps[0]));
      else
        return new Val(context.Engine.GameScenarioManager.GetGameStateF((string)ps[0], (float)ps[1]));
    }

    public static Val SetGameStateF(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.SetGameStateF((string)ps[0], (float)ps[1]);
      return Val.TRUE;
    }

    public static Val GetGameStateS(Context context, Val[] ps)
    {
      if (ps.Length == 1)
        return new Val(context.Engine.GameScenarioManager.GetGameStateS((string)ps[0]));
      else
        return new Val(context.Engine.GameScenarioManager.GetGameStateS((string)ps[0], (string)ps[1]));
    }

    public static Val SetGameStateS(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.SetGameStateS((string)ps[0], (string)ps[1]);
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

      context.Engine.GameScenarioManager.Scenario.StageNumber = (int)ps[0];
      return Val.TRUE;
    }

    public static Val GetRegisterCount(Context context, Val[] ps)
    {
      if (context.Engine.GameScenarioManager.Scenario == null)
        return new Val(0);

      HashSet<ActorInfo> reg = context.Engine.GameScenarioManager.Scenario.GetRegister((string)ps[0]);
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
      Script script = Script.Registry.Get((string)ps[1]);
      if (script != null)
        context.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + (float)ps[0]
        , (c, s) => s.Run(c), context, script);
      else
      {
        // core events // implement this elsewhere
        GameEvent g = null;
        switch (((string)ps[1]).ToLower())
        {
          case "common.fadein":
            g = context.Engine.GameScenarioManager.Scenario.FadeIn;
            break;

          case "common.fadeout":
            g = context.Engine.GameScenarioManager.Scenario.FadeOut;
            break;
        }
        if (g != null)
          context.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + (float)ps[0], g);
        else
          throw new InvalidOperationException(string.Format("Script event '{0}' does not exist!", (string)ps[1]));
      }

      return Val.TRUE;
    }
  }
}
