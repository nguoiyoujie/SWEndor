using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Scenarios.Scripting.Expressions;
using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class ActorManagement
  {
    public static object Actor_Spawn(Context context, params object[] ps)
    {
      GameScenarioBase gscenario = GameScenarioManager.Instance().Scenario;
      if (gscenario == null)
        return "-1";

      ActorTypeInfo atype = ActorTypeInfo.Factory.Get(ps[0].ToString());
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

    public static object Actor_SetActive(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null)
        return false;

      int id = Convert.ToInt32(ps[0].ToString());
      GameScenarioManager.Instance().Scenario.ActiveActor = ActorInfo.Factory.GetExact(id);
      return GameScenarioManager.Instance().Scenario.ActiveActor != null;
    }

    public static object Actor_IsAlive(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      return GameScenarioManager.Instance().Scenario.ActiveActor.ActorState != ActorState.DEAD;
    }

    public static object Actor_RegisterEvents(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      GameScenarioManager.Instance().Scenario.RegisterEvents(GameScenarioManager.Instance().Scenario.ActiveActor);
      return true;
    }

    public static object Actor_GetLocalPosition(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return new float[] { 0, 0, 0 };

      TV_3DVECTOR vec = GameScenarioManager.Instance().Scenario.ActiveActor.GetLocalPosition();
      return new float[] { vec.x, vec.y, vec.z };
    }

    public static object Actor_GetLocalRotation(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return new float[] { 0, 0, 0 };

      TV_3DVECTOR vec = GameScenarioManager.Instance().Scenario.ActiveActor.GetLocalRotation();
      return new float[] { vec.x, vec.y, vec.z };
    }

    public static object Actor_GetLocalDirection(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return new float[] { 0, 0, 0 };

      TV_3DVECTOR vec = GameScenarioManager.Instance().Scenario.ActiveActor.GetLocalDirection();
      return new float[] { vec.x, vec.y, vec.z };
    }

    public static object Actor_GetPosition(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return new float[] { 0, 0, 0 };

      TV_3DVECTOR vec = GameScenarioManager.Instance().Scenario.ActiveActor.GetPosition();
      return new float[] { vec.x, vec.y, vec.z };
    }

    public static object Actor_GetRotation(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return new float[] { 0, 0, 0 };

      TV_3DVECTOR vec = GameScenarioManager.Instance().Scenario.ActiveActor.GetRotation();
      return new float[] { vec.x, vec.y, vec.z };
    }

    public static object Actor_GetDirection(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return new float[] { 0, 0, 0 };

      TV_3DVECTOR vec = GameScenarioManager.Instance().Scenario.ActiveActor.GetDirection();
      return new float[] { vec.x, vec.y, vec.z };
    }

    public static object Actor_SetLocalPosition(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      GameScenarioManager.Instance().Scenario.ActiveActor.SetLocalPosition(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }

    public static object Actor_SetLocalRotation(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      GameScenarioManager.Instance().Scenario.ActiveActor.SetLocalRotation(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }

    public static object Actor_SetLocalDirection(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      GameScenarioManager.Instance().Scenario.ActiveActor.SetLocalDirection(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }

    public static object Actor_SetRotation(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      GameScenarioManager.Instance().Scenario.ActiveActor.SetRotation(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }

    public static object Actor_SetDirection(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      GameScenarioManager.Instance().Scenario.ActiveActor.SetDirection(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }

    public static object Actor_LookAtPoint(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      TV_3DVECTOR vec = new TV_3DVECTOR(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      if (ps.Length == 3)
        GameScenarioManager.Instance().Scenario.ActiveActor.LookAtPoint(vec);
      else
        GameScenarioManager.Instance().Scenario.ActiveActor.LookAtPoint(vec, Convert.ToBoolean(ps[3].ToString()));

      return true;
    }

    public static object Actor_GetProperty(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return null;

      object result = null;
      ConfigureActorProperty(GameScenarioManager.Instance().Scenario.ActiveActor, ps[0].ToString(), false, ref result);
      return result;
    }

    public static object Actor_SetProperty(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return null;

      ConfigureActorProperty(GameScenarioManager.Instance().Scenario.ActiveActor, ps[0].ToString(), true, ref ps[1]);
      return ps[1];
    }

    private static void ConfigureActorProperty(ActorInfo actor, string key, bool setValue, ref object newValue)
    {
      switch (key)
      {
        //case "ActorState":
        //  if (setValue)
        //    actor.ActorState = (ActorState)Enum.Parse(typeof(ActorState), newValue.ToString());
        //  else
        //    newValue = actor.ActorState;
        //  return;
        case "AllowRegen":
          if (setValue)
            actor.RegenerationInfo.AllowRegen = Convert.ToBoolean(newValue);
          else
            newValue = actor.RegenerationInfo.AllowRegen;
          return;
        case "ApplyZBalance":
          if (setValue)
            actor.MovementInfo.ApplyZBalance = Convert.ToBoolean(newValue);
          else
            newValue = actor.MovementInfo.ApplyZBalance;
          return;
        case "CamDeathCircleHeight":
          if (setValue)
            actor.CameraSystemInfo.CamDeathCircleHeight = Convert.ToSingle(newValue);
          else
            newValue = actor.CameraSystemInfo.CamDeathCircleHeight;
          return;
        case "CamDeathCirclePeriod":
          if (setValue)
            actor.CameraSystemInfo.CamDeathCirclePeriod = Convert.ToSingle(newValue);
          else
            newValue = actor.CameraSystemInfo.CamDeathCirclePeriod;
          return;
        case "CamDeathCircleRadius":
          if (setValue)
            actor.CameraSystemInfo.CamDeathCircleRadius = Convert.ToSingle(newValue);
          else
            newValue = actor.CameraSystemInfo.CamDeathCircleRadius;
          return;
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
            actor.RegenerationInfo.ChildRegenRate = Convert.ToSingle(newValue);
          else
            newValue = actor.RegenerationInfo.ChildRegenRate;
          return;
        case "DamageModifier":
          if (setValue)
            actor.CombatInfo.DamageModifier = Convert.ToSingle(newValue);
          else
            newValue = actor.CombatInfo.DamageModifier;
          return;
        case "DeathExplosionSize":
          if (setValue)
            actor.ExplosionInfo.DeathExplosionSize = Convert.ToSingle(newValue);
          else
            newValue = actor.ExplosionInfo.DeathExplosionSize;
          return;
        case "DeathExplosionType":
          if (setValue)
            actor.ExplosionInfo.DeathExplosionType = newValue.ToString();
          else
            newValue = actor.ExplosionInfo.DeathExplosionType;
          return;
        case "EnableDeathExplosion":
          if (setValue)
            actor.ExplosionInfo.EnableDeathExplosion = Convert.ToBoolean(newValue);
          else
            newValue = actor.ExplosionInfo.EnableDeathExplosion;
          return;
        /*
      case "EnableExplosions":
        if (setValue)
          actor.ExplosionInfo.EnableExplosions = Convert.ToBoolean(newValue);
        else
          newValue = actor.ExplosionInfo.EnableExplosions;
        return;
        */
        case "ExplosionCooldown":
          if (setValue)
            actor.ExplosionInfo.ExplosionCooldown = Convert.ToSingle(newValue);
          else
            newValue = actor.ExplosionInfo.ExplosionCooldown;
          return;
        case "ExplosionRate":
          if (setValue)
            actor.ExplosionInfo.ExplosionRate = Convert.ToSingle(newValue);
          else
            newValue = actor.ExplosionInfo.ExplosionRate;
          return;
        case "ExplosionSize":
          if (setValue)
            actor.ExplosionInfo.ExplosionSize = Convert.ToSingle(newValue);
          else
            newValue = actor.ExplosionInfo.ExplosionSize;
          return;
        case "ExplosionType":
          if (setValue)
            actor.ExplosionInfo.ExplosionType = newValue.ToString();
          else
            newValue = actor.ExplosionInfo.ExplosionType;
          return;
        case "HuntWeight":
          if (setValue)
            actor.HuntWeight = Convert.ToInt32(newValue);
          else
            newValue = actor.HuntWeight;
          return;
        case "IsCombatObject":
          if (setValue)
            actor.CombatInfo.IsCombatObject = Convert.ToBoolean(newValue);
          else
            newValue = actor.CombatInfo.IsCombatObject;
          return;
        case "MaxSecondOrderTurnRateFrac":
          if (setValue)
            actor.MovementInfo.MaxSecondOrderTurnRateFrac = Convert.ToSingle(newValue);
          else
            newValue = actor.MovementInfo.MaxSecondOrderTurnRateFrac;
          return;
        case "MaxSpeed":
          if (setValue)
            actor.MovementInfo.MaxSpeed = Convert.ToSingle(newValue);
          else
            newValue = actor.MovementInfo.MaxSpeed;
          return;
        case "MaxSpeedChangeRate":
          if (setValue)
            actor.MovementInfo.MaxSpeedChangeRate = Convert.ToSingle(newValue);
          else
            newValue = actor.MovementInfo.MaxSpeedChangeRate;
          return;
        case "MaxStrength":
          if (setValue)
            actor.CombatInfo.MaxStrength = Convert.ToSingle(newValue);
          else
            newValue = actor.CombatInfo.MaxStrength;
          return;
        case "MaxTurnRate":
          if (setValue)
            actor.MovementInfo.MaxTurnRate = Convert.ToSingle(newValue);
          else
            newValue = actor.MovementInfo.MaxTurnRate;
          return;
        case "MinSpeed":
          if (setValue)
            actor.MovementInfo.MinSpeed = Convert.ToSingle(newValue);
          else
            newValue = actor.MovementInfo.MinSpeed;
          return;
        case "SideBarName":
          if (setValue)
            actor.SideBarName = newValue.ToString();
          else
            newValue = actor.SideBarName;
          return;
        case "ZNormFrac":
          if (setValue)
            actor.MovementInfo.ZNormFrac = Convert.ToSingle(newValue);
          else
            newValue = actor.MovementInfo.ZNormFrac;
          return;
        case "ZTilt":
          if (setValue)
            actor.MovementInfo.ZTilt = Convert.ToSingle(newValue);
          else
            newValue = actor.MovementInfo.ZTilt;
          return;
        case "SetSpawnerEnable":
          if (setValue)
            actor.SetSpawnerEnable(Convert.ToBoolean(newValue));
          else
            newValue = actor.SpawnerInfo?.Enabled ?? false;
          return;
      }
    }

  }
}
