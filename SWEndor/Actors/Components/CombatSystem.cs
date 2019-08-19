using MTV3D65;
using SWEndor.Actors.Data;
using System;

namespace SWEndor.Actors.Components
{
  public enum CombatEventType
  {
    // TimedLifeData
    TIME_START,
    TIME_STOP,
    TIME_LOWERTO,
    TIME_RAISETO,
    TIME_DECAY,

    // CombatData, SysData
    SET_STRENGTH,
    DAMAGE,
    DAMAGE_FRAC,
    COLLISIONDAMAGE,
    RECOVER,
    RECOVER_FRAC
  }

  public class CombatSystem
  {
    internal static void Deactivate(Engine engine, ActorInfo actor)
    {
      engine.ActorDataSet.CombatData[actor.dataID].IsCombatObject = false;
    }

    internal static void onNotify(Engine engine, ActorInfo actor, CombatEventType type, float parameter)
    {
      onNotify(engine
              , actor
              , type
              , parameter
              , ref engine.ActorDataSet.CombatData[actor.dataID]
              , ref engine.TimedLifeDataSet.list[actor.dataID]
              , ref engine.SysDataSet.list[actor.dataID]
              );
    }

    private static void onNotify(Engine engine, ActorInfo actor, CombatEventType type, float parameter, ref CombatData cdata, ref TimedLifeData tdata, ref SysData sdata)
    {
      switch (type)
      {
        case CombatEventType.TIME_DECAY:
          tdata.TimedLife -= engine.Game.TimeSinceRender;
          if (tdata.TimedLife < 0f)
            Dying(engine, actor);
          break;

        case CombatEventType.TIME_START:
          tdata.OnTimedLife = true;
          tdata.TimedLife = parameter;
          break;

        case CombatEventType.TIME_STOP:
          tdata.OnTimedLife = false;
          break;

        case CombatEventType.TIME_LOWERTO:
          if (tdata.TimedLife > parameter)
            tdata.TimedLife = parameter;
          break;

        case CombatEventType.TIME_RAISETO:
          if (tdata.TimedLife < parameter)
            tdata.TimedLife = parameter;
          break;

        case CombatEventType.SET_STRENGTH:
          sdata.Strength = parameter;
          if (cdata.IsCombatObject && sdata.Strength <= 0)
            Dying(engine, actor);
          break;

        case CombatEventType.DAMAGE:
          sdata.Strength -= parameter * cdata.DamageModifier;
          if (cdata.IsCombatObject && sdata.Strength <= 0)
            Dying(engine, actor);
          break;

        case CombatEventType.COLLISIONDAMAGE:
          sdata.Strength -= parameter * cdata.CollisionDamageModifier;
          if (cdata.IsCombatObject && sdata.Strength <= 0)
            Dying(engine, actor);
          break;

        case CombatEventType.DAMAGE_FRAC:
          sdata.Strength -= parameter * sdata.MaxStrength;
          if (cdata.IsCombatObject && sdata.Strength <= 0)
            Dying(engine, actor);
          break;

        case CombatEventType.RECOVER:
          sdata.Strength += parameter;
          break;

        case CombatEventType.RECOVER_FRAC:
          sdata.Strength += parameter * sdata.MaxStrength;
          break;
      }
    }

    private static void Dying(Engine engine, ActorInfo actor)
    {
      if (!actor.ActorState.IsDyingOrDead())
        actor.ActorState = ActorState.DYING;
      else if (actor.ActorState.IsDying())
        actor.ActorState = ActorState.DEAD;
    }

    public static void Process(Engine engine, ActorInfo actor)
    {
      // Expired
      if (engine.TimedLifeDataSet.OnTimedLife_get(actor))
        onNotify(engine, actor, CombatEventType.TIME_DECAY, engine.Game.TimeSinceRender);
    }
  }
}
