using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Primitives.Extensions;

namespace SWEndor.Actors.Models
{
  public enum SystemState : byte // 4 values 
  {
    ABSENT = 0,
    DESTROYED = 1,
    DISABLED = 2,
    ACTIVE = 3
  }

  public enum SystemPart : byte // up to 64 / 4 = 16 values
  {
    //COCKPIT, // Critical system, all controls
    ENGINE, // Allow speed changes and rotation
    //RUDDERS, // Allow rotation
    SHIELD_GENERATOR, // required to regen shields
    RADAR, // UI
    SCANNER, // UI, required for missile alert
    TARGETING_SYSTEM, // UI, required for guided projectiles
    COMLINK, // allow communication with squad
    ENERGY_STORE, // stores 'energy'
    ENERGY_CHARGER, // 'energy' income
    LASER_WEAPONS, // required for laser projectiles
    PROJECTILE_LAUNCHERS, // required for guided projectiles
    HYPERDRIVE
  }

  public struct SystemModel
  {
    private ulong State;
    public float Energy_inStore;
    public float Energy_inEngine; // 100% for full engine
    public float Energy_inShields; // 100% for full regen
    public float Energy_inLasers; // 100% for full rof

    public float SetPoint_Engine;
    public float SetPoint_Shields;
    public float SetPoint_Lasers;

    //public int Projectiles;

    public void Init(ref SystemData data)
    {
      foreach (SystemPart p in data.Parts)
        SetStatus(p, SystemState.ACTIVE);

      //SetStatus(SystemPart.COCKPIT, SystemState.ACTIVE); // Always active

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
      State = 0;
      Energy_inStore = 0;
      Energy_inEngine = 0;
      SetPoint_Engine = 0;
      Energy_inShields = 0;
      SetPoint_Shields = 0;
      Energy_inLasers = 0;
      SetPoint_Lasers = 0;
    }

    public SystemState Status(SystemPart part)
    {
      ulong s = (State >> 2 * (byte)part) & 0x3; // 0-3
      return (SystemState)s;
    }

    public void SetStatus(SystemPart part, SystemState newstate)
    {
      State = (State & ~(0x3U << (2 * (byte)part))) | ((ulong)newstate << (2 * (byte)part));
    }

    public void DamageRandom(Engine engine, ActorInfo actor, ref SystemData data)
    {
      if (data.Parts.Length == 0)
        return;

      SystemPart p = data.Parts[engine.Random.Next(0, data.Parts.Length)];
      if (Status(p) > SystemState.DESTROYED)
      {
        SetStatus(p, SystemState.DESTROYED);
        //if (p == SystemPart.COCKPIT) //critical
        //  actor.SetState_Dying();
      }
    }

    public void DisableRandom(Engine engine, ActorInfo actor, ref SystemData data)
    {
      if (data.Parts.Length == 0)
        return;

      SystemPart p = data.Parts[engine.Random.Next(0, data.Parts.Length)];
      if (Status(p) > SystemState.DISABLED)
        SetStatus(p, SystemState.DISABLED);
    }

    public void Distribute(ref SystemData data, float time)
    {
      bool chargeActive = Status(SystemPart.ENERGY_CHARGER) == SystemState.ACTIVE;
      bool engineActive = Status(SystemPart.ENGINE) == SystemState.ACTIVE;
      bool shieldActive = Status(SystemPart.SHIELD_GENERATOR) == SystemState.ACTIVE;
      bool laserActive = Status(SystemPart.LASER_WEAPONS) == SystemState.ACTIVE;
      bool storeActive = Status(SystemPart.ENERGY_STORE) == SystemState.ACTIVE;

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
