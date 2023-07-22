using SWEndor.Game.ActorTypes.Components;
using SWEndor.Game.Core;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.Factories;
using System.Collections.Generic;

namespace SWEndor.Game.Actors.Models
{
  internal class SystemInstrument
  {
    public SystemPart PartType;
    public SystemState Status = SystemState.ACTIVE;
    //public float EnergyRequirement;
    //public float EnergyAllocated;
    public int Endurance;
    public int MaxEndurance;
    public float DamageChance;
    public float RecoveryTime;
    public float RecoveryTimeRandom;

    // live data
    public float RecoveryCooldownTime;
    public int RecoveryEndurance;


    public void Init(SystemInstrumentData data)
    {
      PartType = data.PartType;
      Status = SystemState.ACTIVE;
      Endurance = data.Endurance;
      MaxEndurance = data.Endurance;
      DamageChance = data.DamageChance;
      RecoveryTime = data.RecoveryTime;
      RecoveryTimeRandom = data.RecoveryTimeRandom;
      RecoveryEndurance = data.RecoveryEndurance;
    }
  }

  internal struct SystemModel
  {
    public List<SystemInstrument> Instruments;

    public float Energy_inStore;
    public float Energy_inEngine; // 100% for full engine
    public float Energy_inShields; // 100% for full regen
    public float Energy_inLasers; // 100% for full rof

    public float SetPoint_Engine;
    public float SetPoint_Shields;
    public float SetPoint_Lasers;

    // live data
    public float StunRecoverTime;

    static SystemModel()
    {
      ObjectPool<List<SystemInstrument>>.CreateStaticPool(() => new List<SystemInstrument>(16), (a) => a.Clear());
      ObjectPool<SystemInstrument>.CreateStaticPool(() => new SystemInstrument());
    }

    public void Init(ref SystemData data)
    {
      if (Instruments == null)
      {
        Instruments = ObjectPool<List<SystemInstrument>>.GetStaticPool().GetNew();
      }
      if (data.Parts != null)
      {
        foreach (SystemInstrumentData instrdata in data.Parts)
        {
          SystemInstrument instrument = ObjectPool<SystemInstrument>.GetStaticPool().GetNew();
          instrument.Init(instrdata);
          Instruments.Add(instrument);
        }
      }

      Energy_inStore = 0; //data.MaxEnergy_inStore;
      Energy_inEngine = data.MaxEnergy_inEngine;
      SetPoint_Engine = data.MaxEnergy_inEngine;
      Energy_inShields = data.MaxEnergy_inShields;
      SetPoint_Shields = data.MaxEnergy_inShields;
      Energy_inLasers = data.MaxEnergy_inLasers;
      SetPoint_Lasers = data.MaxEnergy_inLasers;
    }

    public void Reset()
    {
      foreach (SystemInstrument instrument in Instruments)
      {
        ObjectPool<SystemInstrument>.GetStaticPool().Return(instrument);
      }
      Instruments.Clear();
      Energy_inStore = 0;
      Energy_inEngine = 0;
      SetPoint_Engine = 0;
      Energy_inShields = 0;
      SetPoint_Shields = 0;
      Energy_inLasers = 0;
      SetPoint_Lasers = 0;
      StunRecoverTime = 0;
    }

    public SystemState GetStatus(Engine engine, SystemPart part)
    {
      SystemState state = SystemState.PASSIVE;
      foreach (SystemInstrument instrument in Instruments)
      {
        // get best status of all relevant parts
        if (instrument.PartType == part)
        {
          if ((int)state < (int)instrument.Status)
          {
            state = instrument.Status;
            if (state == SystemState.ACTIVE)
              break;
          }
        }
      }
      if (state == SystemState.ACTIVE && IsStunned(engine)) { state = SystemState.DISABLED; }
      return state;
    }

    public void SetStatus(SystemPart part, SystemState newstate)
    {
      // set all relevant parts
      foreach (SystemInstrument instrument in Instruments)
      {
        if (instrument.PartType == part && instrument.Status != newstate)
        {
          instrument.Status = newstate;
        }
      }
    }

    public bool IsStunned(Engine engine)
    {
      return StunRecoverTime > engine.Game.GameTime;
    }

    public void InflictStun(Engine engine, float duration)
    {
      StunRecoverTime = engine.Game.GameTime + duration;
    }

    public void DamageRandom(Engine engine, ActorInfo actor, float chanceModifier, ref SystemData data)
    {
      if (Instruments.Count == 0 || chanceModifier == 0)
        return;

      foreach (SystemInstrument instrument in Instruments)
      {
        if (instrument.Status == SystemState.DISABLED || instrument.Status == SystemState.ACTIVE)
        {
          if (engine.Random.NextDouble() < instrument.DamageChance * chanceModifier)
          {
            if (instrument.Endurance > 0)
              instrument.Endurance--;

            if (instrument.Endurance <= 0)
            {
              instrument.Status = SystemState.DAMAGED;
              instrument.RecoveryCooldownTime = engine.Game.GameTime + instrument.RecoveryTime + (float)(engine.Random.NextDouble() * instrument.RecoveryTimeRandom);
              if (actor.IsPlayer && !actor.IsSystemOperational(instrument.PartType)) // check IsSystemOperational... in case the unit sports more than one instrument
                engine.Screen2D.MessageSystemsText(TextLocalization.Get(TextLocalKeys.SUBSYSTEM_LOST).F(instrument.PartType.GetDisplayName())
                                                 , 3
                                                 , ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_WARNING));
            }
          }
        }
      }
    }

    public void Tick(Engine engine, ActorInfo actor, float time)
    {
      foreach (SystemInstrument instrument in Instruments)
      {
        // repair attempt
        if (instrument.Status == SystemState.DAMAGED && instrument.RecoveryEndurance > 0)
        {
          if (instrument.RecoveryCooldownTime < engine.Game.GameTime)
          {
            bool functionalbeforerepair = actor.IsSystemOperational(instrument.PartType);
            instrument.RecoveryCooldownTime = 0;
            instrument.Status = SystemState.ACTIVE;
            instrument.Endurance = instrument.RecoveryEndurance;

            if (actor.IsPlayer && !functionalbeforerepair)
              engine.Screen2D.MessageSystemsText(TextLocalization.Get(TextLocalKeys.SUBSYSTEM_RECOVERED).F(instrument.PartType.GetDisplayName())
                                               , 3
                                               , ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_NORMAL));
          }
        }
      }
    }

    public void DisableRandom(Engine engine, ActorInfo actor, ref SystemData data)
    {
      if (Instruments.Count == 0)
        return;

      SystemInstrument instrument = Instruments[engine.Random.Next(0, Instruments.Count)];
      if (instrument.Status == SystemState.ACTIVE)
      {
        instrument.Status = SystemState.DISABLED;
        instrument.RecoveryCooldownTime = engine.Game.GameTime + instrument.RecoveryTime + (float)(engine.Random.NextDouble() * instrument.RecoveryTimeRandom);
        if (actor.IsPlayer && !actor.IsSystemOperational(instrument.PartType)) // check IsSystemOperational... in case the unit sports more than one instrument
          engine.Screen2D.MessageSystemsText(TextLocalization.Get(TextLocalKeys.SUBSYSTEM_DISABLED).F(instrument.PartType.GetDisplayName())
                                           , 3
                                           , ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_WARNING));
      }
    }

    public void Distribute(Engine engine, ref SystemData data, float time)
    {
      bool chargeActive = GetStatus(engine, SystemPart.ENERGY_CHARGER) == SystemState.ACTIVE;
      bool engineActive = GetStatus(engine, SystemPart.ENGINE) == SystemState.ACTIVE;
      bool shieldActive = GetStatus(engine, SystemPart.SHIELD_GENERATOR) == SystemState.ACTIVE;
      bool laserActive = GetStatus(engine, SystemPart.LASER_WEAPONS) == SystemState.ACTIVE;
      bool storeActive = GetStatus(engine, SystemPart.ENERGY_STORE) == SystemState.ACTIVE;

      float engineDemand = engineActive ? (SetPoint_Engine - Energy_inEngine).Clamp(-data.Energy_TransferRate, data.Energy_TransferRate) : 0;
      float shieldDemand = shieldActive ? (SetPoint_Shields - Energy_inShields).Clamp(-data.Energy_TransferRate, data.Energy_TransferRate) : 0;
      float laserDemand = laserActive ? (SetPoint_Lasers - Energy_inLasers).Clamp(-data.Energy_TransferRate, data.Energy_TransferRate) : 0;
      float demand = engineDemand + shieldDemand + laserDemand;

      float supply = (chargeActive ? data.Energy_Income : data.Energy_NoChargerIncome) * time;
      if (demand > supply)
        if (demand <= supply + Energy_inStore)
        {
          supply = demand;
          Energy_inStore = storeActive ? supply - demand : 0;
        }

      Energy_inEngine += supply * engineDemand / demand;
      Energy_inShields += supply * shieldDemand / demand;
      Energy_inLasers += supply * laserDemand / demand;
    }
  }
}
