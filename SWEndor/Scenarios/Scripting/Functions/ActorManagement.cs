using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.Scenarios.Scripting.Expressions;
using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class ActorManagement
  {
    public static Val Squadron_Spawn(Context context, params Val[] ps)
    {
      GameScenarioBase gscenario = context.Engine.GameScenarioManager.Scenario;

      string squadName = (string)ps[0];
      string actorType = (string)ps[1];
      string faction = (string)ps[2];
      int squadCount = (int)ps[3];
      float waitDelay = (float)ps[4];
      TargetType targetType = (TargetType)Enum.Parse(typeof(TargetType), (string)ps[5], true);
      bool hyperspaceIn = (bool)ps[6];
      GSFunctions.SquadFormation squadFormation = (GSFunctions.SquadFormation)Enum.Parse(typeof(GSFunctions.SquadFormation), (string)ps[7], true);
      TV_3DVECTOR rotation = new TV_3DVECTOR((float)ps[8], (float)ps[9], (float)ps[10]);
      float squadDistance = (float)ps[11]; // formation distance
      float spawntime = (float)ps[12];
      TV_3DVECTOR position = new TV_3DVECTOR((float)ps[13], (float)ps[14], (float)ps[15]);
      List<string> registries = new List<string>();
      for (int i = 16; i < ps.Length; i++)
        registries.Add((string)ps[i]);

      GSFunctions.SquadSpawnInfo sinfo = new GSFunctions.SquadSpawnInfo(
        squadName,
        context.Engine.ActorTypeFactory.Get(actorType),
        FactionInfo.Factory.Get(faction),
        squadCount,
        waitDelay,
        targetType,
        hyperspaceIn,
        squadFormation,
        rotation,
        squadDistance,
        registries.ToArray()
        );

      ActorInfo[] squad = GSFunctions.Squadron_Spawn(context.Engine, gscenario, position, spawntime, sinfo);
      int[] ret = new int[squad.Length];
      for (int i = 0; i < squad.Length; i++)
        ret[i] = squad[i].ID;

      return new Val(ret);
    }

    public static Val AddToSquad(Context context, params Val[] ps)
    {
      int id1 = (int)ps[0];
      int id2 = (int)ps[1];
      ActorInfo a1 = context.Engine.ActorFactory.Get(id1);
      ActorInfo a2 = context.Engine.ActorFactory.Get(id2);
      if (context.Engine.GameScenarioManager.Scenario == null || a1 == null || a2 == null)
        return Val.FALSE;

      a2.JoinSquad(a1);
      return Val.TRUE;
    }

    public static Val RemoveFromSquad(Context context, params Val[] ps)
    {
      int id1 = (int)ps[0];
      ActorInfo a1 = context.Engine.ActorFactory.Get(id1);
      if (context.Engine.GameScenarioManager.Scenario == null || a1 == null)
        return Val.FALSE;

      a1.Squad = null;

      return Val.TRUE;
    }

    public static Val MakeSquadLeader(Context context, params Val[] ps)
    {
      int id1 = (int)ps[0];
      ActorInfo a1 = context.Engine.ActorFactory.Get(id1);
      if (context.Engine.GameScenarioManager.Scenario == null || a1 == null)
        return Val.FALSE;

      a1.MakeSquadLeader();
      return Val.TRUE;
    }

    public static Val Spawn(Context context, params Val[] ps)
    {
      GameScenarioBase gscenario = context.Engine.GameScenarioManager.Scenario;
      if (gscenario == null)
        return new Val("-1");

      ActorTypeInfo atype = context.Engine.ActorTypeFactory.Get((string)ps[0]);
      string unitname = (string)ps[1];
      string regname = (string)ps[2]; //OBSOLETE, BUT NEED TO RE-WRITE SCRIPTS BEFORE IMPLEMENTING
      string sidebarname = (string)ps[3];
      float spawntime = (float)ps[4];
      FactionInfo faction = FactionInfo.Factory.Get((string)ps[5]);
      TV_3DVECTOR position = new TV_3DVECTOR((float)ps[6], (float)ps[7], (float)ps[8]);
      TV_3DVECTOR rotation = new TV_3DVECTOR((float)ps[9], (float)ps[10], (float)ps[11]);
      List<string> registries = new List<string>();

      for (int i = 12; i < ps.Length; i++)
        registries.Add((string)ps[i]);

      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = atype,
        Name = unitname,
        SidebarName = sidebarname,
        SpawnTime = spawntime,
        Faction = faction,
        Position = position,
        Rotation = rotation,
        Actions = null,
        Registries = registries.ToArray()
      };

      ActorInfo res = asi.Spawn(gscenario);
      if (res == null)
        return new Val("-1");
      return new Val(res.ID);
    }

    public static Val GetActorType(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val();

      return new Val(actor.TypeInfo.Name);
    }

    public static Val IsFighter(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      return new Val(actor.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER));
    }

    public static Val IsLargeShip(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      return new Val(actor.TypeInfo.AIData.TargetType.Has(TargetType.SHIP));
    }

    public static Val IsAlive(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      return new Val(!actor.IsDead);
    }

    // TO-DO: Remove function from scripts
    public static Val RegisterEvents(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      //context.Engine.GameScenarioManager.Scenario.RegisterEvents(actor);
      return Val.TRUE;
    }

    public static Val GetLocalPosition(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(new float[] { 0, 0, 0 });

      TV_3DVECTOR vec = actor.Position;
      return new Val(new float[] { vec.x, vec.y, vec.z });
    }

    public static Val GetLocalRotation(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(new float[] { 0, 0, 0 });

      TV_3DVECTOR vec = actor.Rotation;
      return new Val(new float[] { vec.x, vec.y, vec.z });
    }

    public static Val GetLocalDirection(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(new float[] { 0, 0, 0 });

      TV_3DVECTOR vec = actor.Direction;
      return new Val(new float[] { vec.x, vec.y, vec.z });
    }

    public static Val GetPosition(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(new float[] { 0, 0, 0 });

      TV_3DVECTOR vec = actor.GetGlobalPosition();
      return new Val(new float[] { vec.x, vec.y, vec.z });
    }

    public static Val GetRotation(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(new float[] { 0, 0, 0 });

      TV_3DVECTOR vec = actor.GetGlobalRotation();
      return new Val(new float[] { vec.x, vec.y, vec.z });
    }

    public static Val GetDirection(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(new float[] { 0, 0, 0 });

      TV_3DVECTOR vec = actor.GetGlobalDirection();
      return new Val(new float[] { vec.x, vec.y, vec.z });
    }

    public static Val SetLocalPosition(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      actor.Position= new TV_3DVECTOR((float)ps[1], (float)ps[2], (float)ps[3]);
      return Val.TRUE;
    }

    public static Val SetLocalRotation(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      actor.Rotation= new TV_3DVECTOR((float)ps[1], (float)ps[2], (float)ps[3]);
      return Val.TRUE;
    }

    public static Val SetLocalDirection(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      actor.Direction= new TV_3DVECTOR((float)ps[1], (float)ps[2], (float)ps[3]);
      return Val.TRUE;
    }
    
    public static Val LookAtPoint(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      TV_3DVECTOR vec= new TV_3DVECTOR((float)ps[1], (float)ps[2], (float)ps[3]);
      //if (ps.Length == 4)
        actor.LookAt(vec);
      //else
      //  actor.LookAt(vec, Convert.ToBoolean(ps[4].ToString()));

      return Val.TRUE;
    }

    public static Val GetChildren(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(new int[0]);

      List<int> ret = new List<int>();
      foreach (ActorInfo a in actor.Children)
        ret.Add(a.ID);

      return new Val(ret.ToArray());
    }

    public static Val GetProperty(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val();

      Val result = new Val();
      ConfigureActorProperty(context.Engine, actor, (string)ps[1], false, ref result);
      return result;
    }

    public static Val SetProperty(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val();

      ConfigureActorProperty(context.Engine, actor, (string)ps[1], true, ref ps[2]);
      return ps[2];
    }

    private static void ConfigureActorProperty(Engine engine, ActorInfo actor, string key, bool setValue, ref Val newValue)
    {
      switch (key)
      {
        //case "ActorState":
        //  if (setValue)
        //    actor.ActorState = (ActorState)Enum.Parse(typeof(ActorState), newValue.ToString());
        //  else
        //    newValue = actor.ActorState;
        //  return;
        //case "NoRegen":
        //  if (setValue)
        //    engine.ActorDataSet.RegenData[actor.dataID].NoRegen = newValue.ValueB;
        //  else
        //    newValue = engine.ActorDataSet.RegenData[actor.dataID].NoRegen;
        //  return;
        case "ApplyZBalance":
          if (setValue)
            actor.MoveData.ApplyZBalance = (bool)newValue;
          else
            newValue = new Val(actor.MoveData.ApplyZBalance);
          return;
          /*
        case "CamDeathCircleHeight":
          if (setValue)
            actor.TypeInfo.DeathCamera.Height = newValue.ValueF;
          else
            newValue = actor.TypeInfo.DeathCamera.Height;
          return;
        case "CamDeathCirclePeriod":
          if (setValue)
            actor.TypeInfo.DeathCamera.Period = newValue.ValueF;
          else
            newValue = actor.TypeInfo.DeathCamera.Period;
          return;
        case "CamDeathCircleRadius":
          if (setValue)
            actor.TypeInfo.DeathCamera.Radius = newValue.ValueF;
          else
            newValue = actor.TypeInfo.DeathCamera.Radius;
          return;
          */
        case "CanEvade":
          if (setValue)
            actor.CanEvade = (bool)newValue;
          else
            newValue = new Val(actor.CanEvade);
          return;
        case "CanRetaliate":
          if (setValue)
            actor.CanRetaliate = (bool)newValue;
          else
            newValue = new Val(actor.CanRetaliate);
          return;
        //case "ChildRegenRate":
        //  if (setValue)
        //    engine.ActorDataSet.RegenData[actor.dataID].ChildRegenRate = newValue.ValueF;
        //  else
        //    newValue = engine.ActorDataSet.RegenData[actor.dataID].ChildRegenRate;
        //  return;
        case "DamageModifier":
          if (setValue)
            actor.SetArmor(DamageType.NORMAL, (float)newValue);
          else
            newValue = new Val(actor.GetArmor(DamageType.NORMAL));
          return;
        case "HuntWeight":
          if (setValue)
            actor.HuntWeight = (int)newValue;
          else
            newValue = new Val(actor.HuntWeight);
          return;
        case "InCombat":
          if (setValue)
            actor.InCombat = (bool)newValue;
          else
            newValue = new Val(actor.InCombat);
          return;
        case "MaxSpeed":
          if (setValue)
            actor.MoveData.MaxSpeed = (float)newValue;
          else
            newValue = new Val(actor.MoveData.MaxSpeed);
          return;
        case "MaxSpeedChangeRate":
          if (setValue)
            actor.MoveData.MaxSpeedChangeRate = (float)newValue;
          else
            newValue = new Val(actor.MoveData.MaxSpeedChangeRate);
          return;
        case "MaxStrength":
          if (setValue)
            actor.MaxHP = (float)newValue;
          else
            newValue = new Val(actor.MaxHP);
          return;
        case "MaxTurnRate":
          if (setValue)
            actor.MoveData.MaxTurnRate = (float)newValue;
          else
            newValue = new Val(actor.MoveData.MaxTurnRate);
          return;
        case "MinSpeed":
          if (setValue)
            actor.MoveData.MinSpeed = (float)newValue;
          else
            newValue = new Val(actor.MoveData.MinSpeed);
          return;
        case "SideBarName":
          if (setValue)
            actor.SideBarName = (string)newValue;
          else
            newValue = new Val(actor.SideBarName);
          return;
        case "SetSpawnerEnable":
          if (setValue)
            actor.SetSpawnerEnable((bool)newValue);
          else
            newValue = new Val(actor.SpawnerInfo.Enabled);
          return;
        case "Strength":
          if (setValue)
            actor.HP = (float)newValue; 
          else
            newValue = new Val(actor.HP);
          return;
        case "Scale":
          if (setValue)
          {
            actor.Scale = (float)newValue;
          }
          else
            newValue = new Val(actor.Scale);
          return;
      }
    }

  }
}
