using Primrose.Expressions;
using Primrose.Primitives.Factories;
using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using System;
using System.Collections.Generic;

namespace SWEndor.Game.Scenarios.Scripting.Functions
{
  public static class GameFns
  {
    public static Val GetGameStateB(IContext context, string name) { return new Val(((Context)context).Engine.GameScenarioManager.Scenario.State.GetGameStateB(name)); }
    public static Val GetGameStateB(IContext context, string name, bool defaultVal) { return new Val(((Context)context).Engine.GameScenarioManager.Scenario.State.GetGameStateB(name, defaultVal)); }
    public static Val SetGameStateB(IContext context, string name, bool value) { ((Context)context).Engine.GameScenarioManager.Scenario.State.SetGameStateB(name, value);  return Val.NULL; }

    public static Val GetGameStateF(IContext context, string name) { return new Val(((Context)context).Engine.GameScenarioManager.Scenario.State.GetGameStateF(name)); }
    public static Val GetGameStateF(IContext context, string name, float defaultVal) { return new Val(((Context)context).Engine.GameScenarioManager.Scenario.State.GetGameStateF(name, defaultVal)); }
    public static Val SetGameStateF(IContext context, string name, float value) { ((Context)context).Engine.GameScenarioManager.Scenario.State.SetGameStateF(name, value); return Val.NULL; }

    public static Val GetGameStateS(IContext context, string name) { return new Val(((Context)context).Engine.GameScenarioManager.Scenario.State.GetGameStateS(name)); }
    public static Val GetGameStateS(IContext context, string name, string defaultVal) { return new Val(((Context)context).Engine.GameScenarioManager.Scenario.State.GetGameStateS(name, defaultVal)); }
    public static Val SetGameStateS(IContext context, string name, string value) { ((Context)context).Engine.GameScenarioManager.Scenario.State.SetGameStateS(name, value); return Val.NULL; }

    public static Val GetGameTime(IContext context) { return new Val(((Context)context).Engine.Game.GameTime); }
    public static Val GetLastFrameTime(IContext context) { return new Val(((Context)context).Engine.Game.TimeSinceRender); }
    public static Val GetDifficulty(IContext context) { return new Val(((Context)context).Engine.GameScenarioManager.Scenario.State.Difficulty); }

    public static Val GetPlayerActorType(IContext context) { return new Val(((Context)context).Engine.PlayerInfo.ActorType.ID); }
    public static Val GetPlayerName(IContext context) { return new Val(((Context)context).Engine.PlayerInfo.Name); }

    public static Val GetStageNumber(IContext context) { return new Val(((Context)context).Engine.GameScenarioManager.Scenario.State.StageNumber); }
    public static Val SetStageNumber(IContext context, int value) { ((Context)context).Engine.GameScenarioManager.Scenario.State.StageNumber = value; return Val.NULL; }

    public static Val GetRegisterCount(IContext context, string register)
    {
      Engine e = ((Context)context).Engine;
      HashSet<ActorInfo> reg = e.GameScenarioManager.Scenario.GetRegister(register);
      if (reg == null)
        return new Val(0);
      return new Val(reg.Count);
    }

    public static Val GetTimeSinceLostWing(IContext context) { return new Val(((Context)context).Engine.GameScenarioManager.Scenario.State.TimeSinceLostWing); }
    public static Val GetTimeSinceLostShip(IContext context) { return new Val(((Context)context).Engine.GameScenarioManager.Scenario.State.TimeSinceLostShip); }
    public static Val GetTimeSinceLostStructure(IContext context) { return new Val(((Context)context).Engine.GameScenarioManager.Scenario.State.TimeSinceLostStructure); }

    public static Val AddEvent(IContext context, float time, string eventName)
    {
      Engine e = ((Context)context).Engine;
      Script script = e.ScriptContext.Scripts.Get(eventName);
      if (script != null)
        e.GameScenarioManager.Scenario.EventQueue.Add(e.Game.GameTime + time
        , (a, s) => s.Run(a), context, script);
      else
      {
        // core events // implement this elsewhere
        GameEvent g = null;
        switch (eventName.ToLower())
        {
          case "common.fadein":
            g = e.GameScenarioManager.Scenario.FadeIn;
            break;

          case "common.fadeout":
            g = e.GameScenarioManager.Scenario.FadeOut;
            break;
        }
        if (g != null)
          e.GameScenarioManager.Scenario.EventQueue.Add(e.Game.GameTime + time, g);
        else
          throw new InvalidOperationException(string.Format("Script event '{0}' does not exist!", eventName));
      }

      return Val.TRUE;
    }

    public static Val AddEvent(IContext context, IList<Val> ps)
    {
      Engine e = ((Context)context).Engine;
      float time = ((float)ps[0]);
      string eventName = ((string)ps[1]);
      Script script = e.ScriptContext.Scripts.Get(eventName);
      List<Val> parameters = ObjectPool<List<Val>>.GetStaticPool().GetNew(); //new Val[ps.Length - 2];
      for (int i = 2; i < ps.Count; i++)
      {
        parameters.Add(ps[i]);
      }
      if (script != null)
      {
        e.GameScenarioManager.Scenario.EventQueue.Add(e.Game.GameTime + time
        , (a, s, p) => { s.Scope.SetParameters(p); ObjectPool<List<Val>>.GetStaticPool().Return(p); s.Run(a); } 
        , context, script, parameters);
      }
      else
      {
        GameEvent g = null;
        switch (eventName.ToLower())
        {
          case "common.fadein":
            g = e.GameScenarioManager.Scenario.FadeIn;
            break;

          case "common.fadeout":
            g = e.GameScenarioManager.Scenario.FadeOut;
            break;
        }
        if (g != null)
          e.GameScenarioManager.Scenario.EventQueue.Add(e.Game.GameTime + time, g);
        else
          throw new InvalidOperationException(string.Format("Script event '{0}' does not exist!", eventName));
      }

      return Val.TRUE;
    }
  }
}
