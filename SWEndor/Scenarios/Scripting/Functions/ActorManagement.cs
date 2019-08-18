﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Traits;
using SWEndor.ActorTypes;
using SWEndor.Scenarios.Scripting.Expressions;
using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class ActorManagement
  {
    public static object Spawn(Context context, params object[] ps)
    {
      GameScenarioBase gscenario = context.Engine.GameScenarioManager.Scenario;
      if (gscenario == null)
        return "-1";

      ActorTypeInfo atype = context.Engine.ActorTypeFactory.Get(ps[0].ToString());
      string unitname = ps[1].ToString();
      string regname = ps[2].ToString();
      string sidebarname = ps[3].ToString();
      float spawntime = Convert.ToSingle(ps[4]);
      FactionInfo faction = FactionInfo.Factory.Get(ps[5].ToString());
      TV_3DVECTOR position = new TV_3DVECTOR(Convert.ToSingle(ps[6]), Convert.ToSingle(ps[7]), Convert.ToSingle(ps[8]));
      TV_3DVECTOR rotation = new TV_3DVECTOR(Convert.ToSingle(ps[9]), Convert.ToSingle(ps[10]), Convert.ToSingle(ps[11]));
      List<string> registries = new List<string>();

      for (int i = 12; i < ps.Length; i++)
        registries.Add(ps[i].ToString());

      ActorSpawnInfo asi = new ActorSpawnInfo
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
      };

      ActorInfo res = asi.Spawn(gscenario);
      if (res == null)
        return "-1";
      return res.ID;
    }

    /*
    public static object SetActive(Context context, params object[] ps)
    {
      if (GameScenarioManager.Scenario == null)
        return false;

      int id = Convert.ToInt32(ps[0].ToString());
      actor = ActorFactory.Get(id);
      return actor != null;
    }
    */

    public static object IsAlive(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      if (context.Engine.GameScenarioManager.Scenario == null)
        return false;

      using (var v = context.Engine.ActorFactory.Get(id))
      {
        if (v == null)
          return false;

        return !v.Value.StateModel.IsDead;
      }
    }

    public static object RegisterEvents(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      if (context.Engine.GameScenarioManager.Scenario == null)
        return false;

      using (var v = context.Engine.ActorFactory.Get(id))
      {
        if (v == null)
          return false;

        context.Engine.GameScenarioManager.Scenario.RegisterEvents(v.Value);
        return true;
      }
    }

    public static object GetLocalPosition(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      if (context.Engine.GameScenarioManager.Scenario == null)
        return new float[] { 0, 0, 0 };

      using (var v = context.Engine.ActorFactory.Get(id))
      {
        if (v == null)
          return new float[] { 0, 0, 0 };

        TV_3DVECTOR vec = v.Value.Transform.Position;
        return new float[] { vec.x, vec.y, vec.z };
      }
    }

    public static object GetLocalRotation(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      if (context.Engine.GameScenarioManager.Scenario == null)
        return new float[] { 0, 0, 0 };

      using (var v = context.Engine.ActorFactory.Get(id))
      {
        if (v == null)
          return new float[] { 0, 0, 0 };

        TV_3DVECTOR vec = v.Value.Transform.Rotation;
        return new float[] { vec.x, vec.y, vec.z };
      }
    }

    public static object GetLocalDirection(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      if (context.Engine.GameScenarioManager.Scenario == null)
        return new float[] { 0, 0, 0 };

      using (var v = context.Engine.ActorFactory.Get(id))
      {
        if (v == null)
          return new float[] { 0, 0, 0 };

        TV_3DVECTOR vec = v.Value.Transform.Direction;
        return new float[] { vec.x, vec.y, vec.z };
      }
    }

    public static object GetPosition(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      if (context.Engine.GameScenarioManager.Scenario == null)
        return new float[] { 0, 0, 0 };

      using (var v = context.Engine.ActorFactory.Get(id))
      {
        if (v == null)
          return new float[] { 0, 0, 0 };

        TV_3DVECTOR vec = v.Value.Transform.Position;
        return new float[] { vec.x, vec.y, vec.z };
      }
    }

    public static object GetRotation(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      if (context.Engine.GameScenarioManager.Scenario == null)
        return new float[] { 0, 0, 0 };

      using (var v = context.Engine.ActorFactory.Get(id))
      {
        if (v == null)
          return new float[] { 0, 0, 0 };

        ActorInfo actor = v.Value;
        TV_3DVECTOR vec = actor.Transform.GetGlobalRotation(actor);
        return new float[] { vec.x, vec.y, vec.z };
      }
    }

    public static object GetDirection(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      if (context.Engine.GameScenarioManager.Scenario == null)
        return new float[] { 0, 0, 0 };

      using (var v = context.Engine.ActorFactory.Get(id))
      {
        if (v == null)
          return new float[] { 0, 0, 0 };

        ActorInfo actor = v.Value;
        TV_3DVECTOR vec = actor.Transform.GetGlobalDirection(actor);
        return new float[] { vec.x, vec.y, vec.z };
      }
    }

    public static object SetLocalPosition(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      if (context.Engine.GameScenarioManager.Scenario == null)
        return false;

      using (var v = context.Engine.ActorFactory.Get(id))
      {
        if (v == null)
          return false;

        ActorInfo actor = v.Value;
        actor.Transform.Position = new TV_3DVECTOR(Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()));
        return true;
      }
    }

    public static object SetLocalRotation(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      if (context.Engine.GameScenarioManager.Scenario == null)
        return false;

      using (var v = context.Engine.ActorFactory.Get(id))
      {
        if (v == null)
          return false;

        ActorInfo actor = v.Value;
        actor.Transform.Rotation = new TV_3DVECTOR(Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()));
        return true;
      }
    }

    public static object SetLocalDirection(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      if (context.Engine.GameScenarioManager.Scenario == null)
        return false;

      using (var v = context.Engine.ActorFactory.Get(id))
      {
        if (v == null)
          return false;

        ActorInfo actor = v.Value;
        actor.Transform.Direction = new TV_3DVECTOR(Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()));
        return true;
      }
    }

    public static object SetRotation(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      if (context.Engine.GameScenarioManager.Scenario == null)
        return false;

      using (var v = context.Engine.ActorFactory.Get(id))
      {
        if (v == null)
          return false;

        ActorInfo actor = v.Value;
        actor.Transform.Rotation = new TV_3DVECTOR(Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()));
        return true;
      }
    }

    public static object SetDirection(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      if (context.Engine.GameScenarioManager.Scenario == null)
        return false;

      using (var v = context.Engine.ActorFactory.Get(id))
      {
        if (v == null)
          return false;

        ActorInfo actor = v.Value;
        actor.Transform.Direction = new TV_3DVECTOR(Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()));
        return true;
      }
    }

    public static object LookAtPoint(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      if (context.Engine.GameScenarioManager.Scenario == null)
        return false;

      using (var v = context.Engine.ActorFactory.Get(id))
      {
        if (v == null)
          return false;

        ActorInfo actor = v.Value;

        TV_3DVECTOR vec = new TV_3DVECTOR(Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()));
        if (ps.Length == 4)
          actor.Transform.LookAt(vec);
        else
          actor.Transform.LookAt(vec, Convert.ToBoolean(ps[4].ToString()));

        return true;
      }
    }

    public static object GetChildren(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      if (context.Engine.GameScenarioManager.Scenario == null)
        return new int[] { };

      using (var v = context.Engine.ActorFactory.Get(id))
      {
        if (v == null)
          return new int[] { };

        return v.Value.Children;
      }
    }

    public static object GetProperty(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      if (context.Engine.GameScenarioManager.Scenario == null)
        return new int[] { };

      using (var v = context.Engine.ActorFactory.Get(id))
      {
        if (v == null)
          return new int[] { };

        ActorInfo actor = v.Value;

        object result = null;
        ConfigureActorProperty(context.Engine, actor, ps[1].ToString(), false, ref result);
        return result;
      }
    }

    public static object SetProperty(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      if (context.Engine.GameScenarioManager.Scenario == null)
        return new int[] { };

      using (var v = context.Engine.ActorFactory.Get(id))
      {
        if (v == null)
          return new int[] { };

        ActorInfo actor = v.Value;

        ConfigureActorProperty(context.Engine, actor, ps[1].ToString(), true, ref ps[2]);
        return ps[2];
      }
    }

    private static void ConfigureActorProperty(Engine engine, ActorInfo actor, string key, bool setValue, ref object newValue)
    {
      switch (key)
      {
        //case "ActorState":
        //  if (setValue)
        //    actor.ActorState = (ActorState)Enum.Parse(typeof(ActorState), newValue.ToString());
        //  else
        //    newValue = actor.ActorState;
        //  return;
        case "NoRegen":
          if (setValue)
            actor.RegenData.NoRegen = Convert.ToBoolean(newValue);
          else
            newValue = actor.RegenData.NoRegen;
          return;
        case "ApplyZBalance":
          if (setValue)
            actor.MoveData.ApplyZBalance = Convert.ToBoolean(newValue);
          else
            newValue = actor.MoveData.ApplyZBalance;
          return;
          /*
        case "CamDeathCircleHeight":
          if (setValue)
            actor.TypeInfo.DeathCamera.Height = Convert.ToSingle(newValue);
          else
            newValue = actor.TypeInfo.DeathCamera.Height;
          return;
        case "CamDeathCirclePeriod":
          if (setValue)
            actor.TypeInfo.DeathCamera.Period = Convert.ToSingle(newValue);
          else
            newValue = actor.TypeInfo.DeathCamera.Period;
          return;
        case "CamDeathCircleRadius":
          if (setValue)
            actor.TypeInfo.DeathCamera.Radius = Convert.ToSingle(newValue);
          else
            newValue = actor.TypeInfo.DeathCamera.Radius;
          return;
          */
        case "CanEvade":
          if (setValue)
            actor.CanEvade = Convert.ToBoolean(newValue);
          else
            newValue = actor.CanEvade;
          return;
        case "CanRetaliate":
          if (setValue)
            actor.CanRetaliate = Convert.ToBoolean(newValue);
          else
            newValue = actor.CanRetaliate;
          return;
        case "ChildRegenRate":
          if (setValue)
            actor.RegenData.ChildRegenRate = Convert.ToSingle(newValue);
          else
            newValue = actor.RegenData.ChildRegenRate;
          return;
        case "DamageModifier":
          if (setValue)
            actor.CombatData.DamageModifier = Convert.ToSingle(newValue);
          else
            newValue = actor.CombatData.DamageModifier;
          return;
        //case "DeathExplosionSize":
        //  if (setValue)
        //    actor.ExplodeData.DeathExplosionSize = Convert.ToSingle(newValue);
        //  else
        //    newValue = actor.ExplodeData.DeathExplosionSize;
        //  return;
        //case "DeathExplosionType":
        //  if (setValue)
        //    actor.ExplodeData.DeathExplosionType = newValue.ToString();
        //  else
        //    newValue = actor.ExplodeData.DeathExplosionType;
        //  return;
        //case "DeathExplosionTrigger":
        //  if (setValue)
        //    actor.ExplodeData.DeathExplosionTrigger = Convert.ToBoolean(newValue);
        //  else
        //    newValue = actor.ExplodeData.DeathExplosionTrigger;
        //  return;
        /*
      case "EnableExplosions":
        if (setValue)
          actor.ExplodeData.EnableExplosions = Convert.ToBoolean(newValue);
        else
          newValue = actor.ExplodeData.EnableExplosions;
        return;
        */
        //case "ExplosionCooldown":
        //  if (setValue)
        //    actor.ExplodeData.ExplosionCooldown = Convert.ToSingle(newValue);
        //  else
        //    newValue = actor.ExplodeData.ExplosionCooldown;
        //  return;
        //case "ExplosionRate":
        //  if (setValue)
        //    actor.ExplodeData.ExplosionRate = Convert.ToSingle(newValue);
        //  else
        //    newValue = actor.ExplodeData.ExplosionRate;
        //  return;
        //case "ExplosionSize":
        //  if (setValue)
        //    actor.ExplodeData.ExplosionSize = Convert.ToSingle(newValue);
        //  else
        //    newValue = actor.ExplodeData.ExplosionSize;
        //  return;
        //case "ExplosionType":
        //  if (setValue)
        //    actor.ExplodeData.ExplosionType = newValue.ToString();
        //  else
        //    newValue = actor.ExplodeData.ExplosionType;
        //  return;
        case "HuntWeight":
          if (setValue)
            actor.HuntWeight = Convert.ToInt32(newValue);
          else
            newValue = actor.HuntWeight;
          return;
        case "IsCombatObject":
          if (setValue)
            actor.CombatData.IsCombatObject = Convert.ToBoolean(newValue);
          else
            newValue = actor.CombatData.IsCombatObject;
          return;
        case "MaxSpeed":
          if (setValue)
            actor.MoveData.MaxSpeed = Convert.ToSingle(newValue);
          else
            newValue = actor.MoveData.MaxSpeed;
          return;
        case "MaxSpeedChangeRate":
          if (setValue)
            actor.MoveData.MaxSpeedChangeRate = Convert.ToSingle(newValue);
          else
            newValue = actor.MoveData.MaxSpeedChangeRate;
          return;
        case "MaxStrength":
          if (setValue)
            actor.Health.SetMaxHP(actor, Convert.ToSingle(newValue), true);
          else
            newValue = actor.Health.MaxHP;
          return;
        case "MaxTurnRate":
          if (setValue)
            actor.MoveData.MaxTurnRate = Convert.ToSingle(newValue);
          else
            newValue = actor.MoveData.MaxTurnRate;
          return;
        case "MinSpeed":
          if (setValue)
            actor.MoveData.MinSpeed = Convert.ToSingle(newValue);
          else
            newValue = actor.MoveData.MinSpeed;
          return;
        case "SideBarName":
          if (setValue)
            actor.SideBarName = newValue.ToString();
          else
            newValue = actor.SideBarName;
          return;
        case "SetSpawnerEnable":
          if (setValue)
            actor.SetSpawnerEnable(Convert.ToBoolean(newValue));
          else
            newValue = actor.SpawnerInfo?.Enabled ?? false;
          return;
        case "Strength":
          if (setValue)
            actor.Health.SetHP(actor, Convert.ToSingle(newValue));
          else
            newValue = actor.Health.HP;
          return;
        case "Scale":
          if (setValue)
          {
            float newscale = Convert.ToSingle(newValue);
            actor.Transform.Scale = newscale;
          }
          else
            newValue = actor.Transform.Scale;
          return;
      }
    }

  }
}
