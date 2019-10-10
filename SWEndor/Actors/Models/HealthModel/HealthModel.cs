using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Components;
using SWEndor.Primitives;
using SWEndor.Primitives.Extensions;
using System;

namespace SWEndor.Actors.Models
{
  public struct HealthModel
  {
    public float HP { get; private set; }
    public float MaxHP { get; private set; }
    public float DisplayHP { get; private set; }

    public bool IsDead { get { return HP <= 0; } }
    public float Frac { get { return HP / MaxHP; } }
    public float Perc { get { return Frac * 100; } }
    public float DisplayFrac { get { return DisplayHP / MaxHP; } }
    public float DisplayPerc { get { return DisplayFrac * 100; } }

    public void Init(ref CombatData data, ActorCreationInfo acinfo)
    {
      MaxHP = data.MaxStrength;
      HP = (acinfo.InitialStrength > 0) ? acinfo.InitialStrength : MaxHP;
    }

    public TV_COLOR Color
    {
      get
      {
        double quad = 1.6708;
        float sr = Frac;
        float r = (float)Math.Cos(sr * quad);
        float g = (float)Math.Sin(sr * quad);
        float b = 0;
        if (r < 0) r = 0;
        if (g < 0) g = 0;
        if (b < 0) b = 0;
        return new TV_COLOR(r, g, b, 1);
      }
    }

    public void InflictDamage(ActorInfo self, DamageInfo dmg)
    {
      if (HP <= 0)
        return;

      float mod = self.GetArmor(dmg.Type);
      float d = dmg.Value * mod;
      HP = (HP - d).Clamp(-1, MaxHP);

      /*
      if (d > 0)
        Log.Write(Log.DEBUG, LogType.ACTOR_DAMAGED, target, dmg.Source, d, HP);
      else if (d < 0)
        Log.Write(Log.DEBUG, LogType.ACTOR_HEALED, target, dmg.Source, -d, HP);
      */

      if (HP <= 0 && !self.IsDyingOrDead) // TO-DO: improve atomicity of ActorState, still may have threading issues
      {
        self.SetState_Dying();

#if DEBUG
        if (self.Logged)
          Log.Write(Log.DEBUG, LogType.ACTOR_KILLED, self);
#endif
      }
    }

    public void Kill(ActorInfo self)
    {
      InflictDamage(self, new DamageInfo(MaxHP, DamageType.ALWAYS_100PERCENT));
    }

    public void SetHP(ActorInfo self, float value)
    {
      if (IsDead)
        return;

      HP = value.Clamp(-1, MaxHP);

      if (HP <= 0 && !self.IsDyingOrDead) // TO-DO: improve atomicity of ActorState, still may have threading issues
      {
        self.SetState_Dying();

#if DEBUG
        if (self.Logged)
          Log.Write(Log.DEBUG, LogType.ACTOR_KILLED_BY, self, "setting HP to 0");
#endif
      }
    }

    public void SetMaxHP(float value, bool scaleHP)
    {
      float f = Frac;
      MaxHP = value;
      if (scaleHP)
        HP = f * value;
      else
        HP = HP.Clamp(-1, MaxHP);
    }

    public void Tick(float time)
    {
      if (HP > DisplayHP)
        DisplayHP = HP;

      if (DisplayHP > HP)
        DisplayHP = (2 * DisplayHP + HP) / 3;
    }
  }
}

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public void InflictDamage(float value, DamageType type, TV_3DVECTOR position)
    {
      using (ScopeCounterManager.Acquire(Scope))
        Health.InflictDamage(this, new DamageInfo(value, type, position));
    }

    public void InflictDamage(float value, DamageType type)
    {
      using (ScopeCounterManager.Acquire(Scope))
        Health.InflictDamage(this, new DamageInfo(value, type));
    }

    public void InflictDamage(float value)
    {
      using (ScopeCounterManager.Acquire(Scope))
        Health.InflictDamage(this, new DamageInfo(value));
    }

    public float HP { get { return Health.HP; } set { using (ScopeCounterManager.Acquire(Scope)) Health.SetHP(this, value); } }
    public float MaxHP { get { return Health.MaxHP; } set { using (ScopeCounterManager.Acquire(Scope)) Health.SetMaxHP(value, false); } }
    public float HP_Perc { get { return Health.Perc; } }
    public float HP_Frac { get { return Health.Frac; } }
    public TV_COLOR HP_Color { get { return Health.Color; } }

    public float DisplayHP { get { return Health.DisplayHP; } }
    public float DisplayHP_Perc { get { return Health.DisplayPerc; } }
    public float DisplayHP_Frac { get { return Health.DisplayFrac; } }
  }
}
