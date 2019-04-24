using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Scenarios;
using System;
using System.Collections.Generic;

namespace SWEndor.Terminal.Commands.Actor
{
  public class Spawn : TCommandBase
  {
    public Spawn()
    {
      MinParameters = 12;
      MaxParameters = int.MaxValue;
      Name = "actor.spawn";
      Desc = "Spawns an actor into the battlefield";

      ParameterNames.Add("actortype");
      ParameterNames.Add("unit_name");
      ParameterNames.Add("registry_name");
      ParameterNames.Add("sidebar_name");
      ParameterNames.Add("spawn_time");
      ParameterNames.Add("faction_name");
      ParameterNames.Add("position_x");
      ParameterNames.Add("position_y");
      ParameterNames.Add("position_z");
      ParameterNames.Add("rotation_x");
      ParameterNames.Add("rotation_y");
      ParameterNames.Add("rotation_z");
      ParameterNames.Add("[registries]"); // optional
    }

    protected override TCommandFeedback Evaluate(string[] param)
    {
      GameScenarioBase gscenario = Globals.Engine.GameScenarioManager.Scenario;
      if (gscenario == null)
        return new TCommandFeedback(TCommandFeedbackType.ERROR, "GameScenario is null!");

      ActorTypeInfo atype = ActorTypeInfo.Factory.Get(param[0].ToString());
      string unitname = param[1];
      string regname = param[2];
      string sidebarname = param[3];
      float spawntime = Convert.ToSingle(param[4]);
      FactionInfo faction = FactionInfo.Factory.Get(param[5]);
      TV_3DVECTOR position = new TV_3DVECTOR(Convert.ToSingle(param[6]), Convert.ToSingle(param[7]), Convert.ToSingle(param[8]));
      TV_3DVECTOR rotation = new TV_3DVECTOR(Convert.ToSingle(param[9]), Convert.ToSingle(param[10]), Convert.ToSingle(param[11]));
      List<string> registries = new List<string>();

      for (int i = 12; i < param.Length; i++)
      {
        registries.Add(param[i].ToString());
      }

      ActorInfo res = new ActorSpawnInfo
      {
        Type = atype,
        Name = unitname,
        RegisterName = regname,
        SidebarName = sidebarname,
        SpawnTime = spawntime,
        Faction = faction,
        Position = position,
        Rotation = rotation,
        Actions = null,
        Registries = registries.ToArray()
      }.Spawn(gscenario);

      if (res == null)
        return new TCommandFeedback(TCommandFeedbackType.ERROR, "SpawnActor failed!");

      return new TCommandFeedback(TCommandFeedbackType.NORMAL, res.ID);
    }
  }
}
