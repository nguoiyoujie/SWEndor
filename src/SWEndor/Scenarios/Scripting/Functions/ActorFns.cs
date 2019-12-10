using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Models;
using Primrose.Primitives.ValueTypes;
using System.Collections.Generic;
using SWEndor.Primitives.Extensions;
using Primrose.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class ActorFns
  {
    public static Val GetActorType(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val();

      return new Val(actor.TypeInfo.Name);
    }

    public static Val IsFighter(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      return new Val(actor.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER));
    }

    public static Val IsLargeShip(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      return new Val(actor.TypeInfo.AIData.TargetType.Has(TargetType.SHIP));
    }

    public static Val IsAlive(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      return new Val(!actor.IsDead);
    }

    public static Val GetLocalPosition(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(new float3(0, 0, 0));

      TV_3DVECTOR vec = actor.Position;
      return new Val(new float3(vec.x, vec.y, vec.z));
    }

    public static Val GetLocalRotation(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(new float3( 0, 0, 0 ));

      TV_3DVECTOR vec = actor.Rotation;
      return new Val(new float3(vec.x, vec.y, vec.z));
    }

    public static Val GetLocalDirection(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(new float3(0, 0, 0));

      TV_3DVECTOR vec = actor.Direction;
      return new Val(new float3(vec.x, vec.y, vec.z));
    }

    public static Val GetGlobalPosition(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(new float3(0, 0, 0));

      TV_3DVECTOR vec = actor.GetGlobalPosition();
      return new Val(new float3(vec.x, vec.y, vec.z));
    }

    public static Val GetGlobalRotation(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(new float3(0, 0, 0));

      TV_3DVECTOR vec = actor.GetGlobalRotation();
      return new Val(new float3(vec.x, vec.y, vec.z));
    }

    public static Val GetGlobalDirection(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(new float3(0, 0, 0));

      TV_3DVECTOR vec = actor.GetGlobalDirection();
      return new Val(new float3(vec.x, vec.y, vec.z));
    }

    public static Val SetLocalPosition(Context context, int actorID, float3 point)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      actor.Position = point.ToVec3();
      return Val.TRUE;
    }

    public static Val SetLocalRotation(Context context, int actorID, float3 point)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      actor.Rotation = point.ToVec3();
      return Val.TRUE;
    }

    public static Val SetLocalDirection(Context context, int actorID, float3 point)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      actor.Direction = point.ToVec3();
      return Val.TRUE;
    }
    
    public static Val LookAtPoint(Context context, int actorID, float3 point)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      TV_3DVECTOR vec = point.ToVec3();
      actor.LookAt(vec);

      return Val.TRUE;
    }

    public static Val GetChildren(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(new int[0]);

      List<int> ret = new List<int>();
      foreach (ActorInfo a in actor.Children)
        ret.Add(a.ID);

      return new Val(ret.ToArray());
    }

    public static Val GetProperty(Context context, int actorID, string property)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val();

      Val result = new Val();
      ConfigureActorProperty(context.Engine, actor, property, false, ref result);
      return result;
    }

    public static Val GetArmor(Context context, int actorID, string sdmgtype)
    {
      DamageType dmgtype = (DamageType)Enum.Parse(typeof(DamageType), sdmgtype);
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.NULL;

      return new Val(actor.GetArmor(dmgtype));
    }

    public static Val SetArmor(Context context, int actorID, string sdmgtype, float value)
    {
      DamageType dmgtype = (DamageType)Enum.Parse(typeof(DamageType), sdmgtype);
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.NULL;

      actor.SetArmor(dmgtype, value);
      return Val.NULL;
    }

    public static Val SetArmorAll(Context context, int actorID, float value)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.NULL;

      actor.SetArmorAll(value);
      return Val.NULL;
    }

    public static Val RestoreArmor(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.NULL;

      actor.RestoreArmor();
      return Val.NULL;
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
        case "CanEvade":
          if (setValue)
            actor.AI.CanEvade = (bool)newValue;
          else
            newValue = new Val(actor.AI.CanEvade);
          return;
        case "CanRetaliate":
          if (setValue)
            actor.AI.CanRetaliate = (bool)newValue;
          else
            newValue = new Val(actor.AI.CanRetaliate);
          return;
        //case "ChildRegenRate":
        //  if (setValue)
        //    engine.ActorDataSet.RegenData[actor.dataID].ChildRegenRate = newValue.ValueF;
        //  else
        //    newValue = engine.ActorDataSet.RegenData[actor.dataID].ChildRegenRate;
        //  return;
        case "DamageModifier":
          if (setValue)
            actor.SetArmor(DamageType.LASER, (float)newValue);
          else
            newValue = new Val(actor.GetArmor(DamageType.LASER));
          return;
        case "HuntWeight":
          if (setValue)
            actor.AI.HuntWeight = (int)newValue;
          else
            newValue = new Val(actor.AI.HuntWeight);
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
        case "Spawner.Enabled":
          if (setValue)
            actor.SpawnerInfo.Enabled = (bool)newValue;
          else
            newValue = new Val(actor.SpawnerInfo.Enabled);
          return;
        case "Spawner.SpawnTypes":
          if (setValue)
            actor.SpawnerInfo.SpawnTypes = (string[])newValue;
          else
            newValue = new Val(actor.SpawnerInfo.SpawnTypes);
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
