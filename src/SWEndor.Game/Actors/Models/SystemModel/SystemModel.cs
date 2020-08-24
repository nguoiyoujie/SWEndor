using SWEndor.Game.ActorTypes.Components;
using SWEndor.Game.Core;
using Primrose.Primitives;
using Primrose.Primitives.Extensions;

namespace SWEndor.Game.Actors.Models
{
  internal struct SystemModel
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

    public SystemState GetStatus(SystemPart part)
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
      if (GetStatus(p) > SystemState.DESTROYED)
      {
        SetStatus(p, SystemState.DESTROYED);
        //if (p == SystemPart.COCKPIT) //critical
        //  actor.SetState_Dying();

        if (actor.IsPlayer)
          engine.Screen2D.MessageSystemsText(TextLocalization.Get(TextLocalKeys.SUBSYSTEM_LOST).F(p.GetEnumName().Replace('_', ' '))
                                           , 3
                                           , ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_WARNING));
      }
    }

    public void DisableRandom(Engine engine, ActorInfo actor, ref SystemData data)
    {
      if (data.Parts.Length == 0)
        return;

      SystemPart p = data.Parts[engine.Random.Next(0, data.Parts.Length)];
      if (GetStatus(p) > SystemState.DISABLED)
        SetStatus(p, SystemState.DISABLED);
    }

    public void Distribute(ref SystemData data, float time)
    {
      bool chargeActive = GetStatus(SystemPart.ENERGY_CHARGER) == SystemState.ACTIVE;
      bool engineActive = GetStatus(SystemPart.ENGINE) == SystemState.ACTIVE;
      bool shieldActive = GetStatus(SystemPart.SHIELD_GENERATOR) == SystemState.ACTIVE;
      bool laserActive = GetStatus(SystemPart.LASER_WEAPONS) == SystemState.ACTIVE;
      bool storeActive = GetStatus(SystemPart.ENERGY_STORE) == SystemState.ACTIVE;

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
