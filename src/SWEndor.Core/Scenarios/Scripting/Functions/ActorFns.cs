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

      return new Val(actor.TypeInfo.ID);
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

    public static Val GetFaction(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(FactionInfo.Neutral.Name);

      return new Val(actor.Faction.Name);
    }

    public static Val SetFaction(Context context, int actorID, string name)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.NULL;

      actor.Faction = FactionInfo.Factory.Get(name);
      return Val.NULL;
    }

    public static Val AddToRegister(Context context, int actorID, string register)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.NULL;

      HashSet<ActorInfo> reg = context.Engine.GameScenarioManager.Scenario.GetRegister(register);
      reg?.Add(actor);

      return Val.NULL;
    }

    public static Val RemoveFromRegister(Context context, int actorID, string register)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.NULL;

      HashSet<ActorInfo> reg = context.Engine.GameScenarioManager.Scenario.GetRegister(register);
      reg?.Remove(actor);

      return Val.NULL;
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
        return Val.NULL;

      actor.Position = point.ToVec3();
      return Val.NULL;
    }

    public static Val SetLocalRotation(Context context, int actorID, float3 point)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.NULL;

      actor.Rotation = point.ToVec3();
      return Val.NULL;
    }

    public static Val SetLocalDirection(Context context, int actorID, float3 point)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.NULL;

      actor.Direction = point.ToVec3();
      return Val.NULL;
    }
    
    public static Val LookAtPoint(Context context, int actorID, float3 point)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.NULL;

      TV_3DVECTOR vec = point.ToVec3();
      actor.LookAt(vec);

      return Val.NULL;
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

    public static Val GetChildrenByType(Context context, int actorID, string actorType)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(new int[0]);

      List<int> ret = new List<int>();
      foreach (ActorInfo a in actor.Children)
        if (a.TypeInfo.ID.Equals(actorType, StringComparison.InvariantCultureIgnoreCase))
          ret.Add(a.ID);

      return new Val(ret.ToArray());
    }

    public static Val GetHP(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(0);

      return new Val(actor.HP);
    }

    public static Val GetShd(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(0);

      return new Val(actor.Shd);
    }

    public static Val GetHull(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(0);

      return new Val(actor.Hull);
    }

    public static Val GetMaxHP(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(0);

      return new Val(actor.MaxHP);
    }

    public static Val GetMaxShd(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(0);

      return new Val(actor.MaxShd);
    }

    public static Val GetMaxHull(Context context, int actorID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return new Val(0);

      return new Val(actor.MaxHull);
    }

    public static Val SetHP(Context context, int actorID, float value)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.NULL;

      actor.HP = value;
      return Val.NULL;
    }

    public static Val SetShd(Context context, int actorID, float value)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.NULL;

      actor.Shd = value;
      return Val.NULL;
    }

    public static Val SetHull(Context context, int actorID, float value)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.NULL;

      actor.Hull = value;
      return Val.NULL;
    }

    public static Val SetMaxHP(Context context, int actorID, float value)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.NULL;

      actor.MaxHP = value;
      return Val.NULL;
    }

    public static Val SetMaxShd(Context context, int actorID, float value)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.NULL;

      actor.MaxShd = value;
      return Val.NULL;
    }

    public static Val SetMaxHull(Context context, int actorID, float value)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.NULL;

      actor.MaxHull = value;
      return Val.NULL;
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
        return new Val(0);

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
        // Regeneration
        case "Regen.NoRegen":
          if (setValue)
            actor.NoRegen = (bool)newValue;
          else
            newValue = new Val(actor.NoRegen);
          return;
        case "Regen.Self":
          if (setValue)
            actor.SelfRegenRate = (float)newValue;
          else
            newValue = new Val(actor.SelfRegenRate);
          return;
        case "Regen.Child":
          if (setValue)
            actor.ChildRegenRate = (float)newValue;
          else
            newValue = new Val(actor.ChildRegenRate);
          return;
        case "Regen.Parent":
          if (setValue)
            actor.ParentRegenRate = (float)newValue;
          else
            newValue = new Val(actor.ParentRegenRate);
          return;
        case "Regen.Sibling":
          if (setValue)
            actor.SiblingRegenRate = (float)newValue;
          else
            newValue = new Val(actor.SiblingRegenRate);
          return;

        // AI
        case "AI.CanEvade":
          if (setValue)
            actor.AI.CanEvade = (bool)newValue;
          else
            newValue = new Val(actor.AI.CanEvade);
          return;
        case "AI.CanRetaliate":
          if (setValue)
            actor.AI.CanRetaliate = (bool)newValue;
          else
            newValue = new Val(actor.AI.CanRetaliate);
          return;
        case "AI.HuntWeight":
          if (setValue)
            actor.AI.HuntWeight = (int)newValue;
          else
            newValue = new Val(actor.AI.HuntWeight);
          return;

        // Movement
        case "Movement.ApplyZBalance":
          if (setValue)
            actor.MoveData.ApplyZBalance = (bool)newValue;
          else
            newValue = new Val(actor.MoveData.ApplyZBalance);
          return;
        case "Movement.MinSpeed":
          if (setValue)
            actor.MoveData.MinSpeed = (float)newValue;
          else
            newValue = new Val(actor.MoveData.MinSpeed);
          return;
        case "Movement.MaxSpeed":
          if (setValue)
            actor.MoveData.MaxSpeed = (float)newValue;
          else
            newValue = new Val(actor.MoveData.MaxSpeed);
          return;
        case "Movement.Speed":
          if (setValue)
            actor.MoveData.Speed = (float)newValue;
          else
            newValue = new Val(actor.MoveData.Speed);
          return;
        case "Movement.MaxSpeedChangeRate":
          if (setValue)
            actor.MoveData.MaxSpeedChangeRate = (float)newValue;
          else
            newValue = new Val(actor.MoveData.MaxSpeedChangeRate);
          return;
        case "Movement.MaxTurnRate":
          if (setValue)
            actor.MoveData.MaxTurnRate = (float)newValue;
          else
            newValue = new Val(actor.MoveData.MaxTurnRate);
          return;

        // Health
        case "Health.HP":
          if (setValue)
            actor.HP = (float)newValue;
          else
            newValue = new Val(actor.HP);
          return;
        case "Health.Shd":
          if (setValue)
            actor.Shd = (float)newValue;
          else
            newValue = new Val(actor.Shd);
          return;
        case "Health.Hull":
          if (setValue)
            actor.Hull = (float)newValue;
          else
            newValue = new Val(actor.Hull);
          return;
        case "Health.MaxHP":
          if (setValue)
            actor.MaxHP = (float)newValue;
          else
            newValue = new Val(actor.MaxHP);
          return;
        case "Health.MaxShd":
          if (setValue)
            actor.MaxShd = (float)newValue;
          else
            newValue = new Val(actor.MaxShd);
          return;
        case "Health.MaxHull":
          if (setValue)
            actor.MaxHull = (float)newValue;
          else
            newValue = new Val(actor.MaxHull);
          return;

        // Spawner
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
        case "Spawner.SpawnsRemaining":
          if (setValue)
            actor.SpawnerInfo.SpawnsRemaining = (int)newValue;
          else
            newValue = new Val(actor.SpawnerInfo.SpawnsRemaining);
          return;
          

        // Transform
        case "Transform.Scale":
          if (setValue)
            actor.Scale = (float)newValue;
          else
            newValue = new Val(actor.Scale);
          return;
        case "Transform.Position":
          if (setValue)
            actor.Position = ((float3)newValue).ToVec3();
          else
            newValue = new Val(actor.Position.ToFloat3());
          return;
        case "Transform.Rotation":
          if (setValue)
            actor.Rotation = ((float3)newValue).ToVec3();
          else
            newValue = new Val(actor.Rotation.ToFloat3());
          return;
        case "Transform.Direction":
          if (setValue)
            actor.Direction = ((float3)newValue).ToVec3();
          else
            newValue = new Val(actor.Direction.ToFloat3());
          return;

        // Misc
        case "InCombat":
          if (setValue)
            actor.InCombat = (bool)newValue;
          else
            newValue = new Val(actor.InCombat);
          return;
        case "Name":
          if (setValue)
            actor.Name = (string)newValue;
          else
            newValue = new Val(actor.Name);
          return;
        case "SideBarName":
          if (setValue)
            actor.SideBarName = (string)newValue;
          else
            newValue = new Val(actor.SideBarName);
          return;
      }
    }

  }
}
