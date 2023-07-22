using MTV3D65;
using SWEndor.Game.ActorTypes;
using SWEndor.Game.ActorTypes.Components;
using Primrose.Primitives;
using Primrose.Primitives.Extensions;
using Primrose;
using SWEndor.Game.Models;

namespace SWEndor.Game.Actors.Models
{
  internal struct HealthModel
  {
    // New model:
    // HP -> Hull + Systems + Shields

    internal float Hull;
    internal float MaxHull;
    internal float Shd;
    internal float MaxShd;

    public float HP { get { return Hull + Shd; } }
    public float MaxHP { get { return MaxHull + MaxShd; } }
    public float DisplayHP { get; private set; }

    //public bool IsDead { get { return HP <= 0; } }
    public float HP_Frac { get { return HP / MaxHP; } }
    public float HP_Perc { get { return HP_Frac * 100; } }
    public float DisplayFrac { get { return DisplayHP / MaxHP; } }
    public float DisplayPerc { get { return DisplayFrac * 100; } }

    public float Shd_Frac { get { return Shd / MaxShd; } }
    public float Shd_Perc { get { return Shd_Frac * 100; } }

    public float Hull_Frac { get { return Hull / MaxHull; } }
    public float Hull_Perc { get { return Hull_Frac * 100; } }

    public void Init(ref CombatData data, ref SystemData sdata, ActorCreationInfo acinfo)
    {
      MaxShd = sdata.MaxShield;
      Shd = (acinfo.InitialStrength > 0) ? acinfo.InitialStrength : MaxShd;
      Hull = sdata.MaxHull;
      MaxHull = sdata.MaxHull;
    }

    public COLOR HP_Color { get { return Color(HP_Frac); } }
    public COLOR Shd_Color { get { return Color(Shd_Frac); } }
    public COLOR Hull_Color { get { return Color(Hull_Frac); } }

    private COLOR Color(float frac)
    {
        float quad = 1.6708f;
        float r = LookUp.Cos(frac * quad);
        float g = LookUp.Sin(frac * quad);
        float b = 0;
        if (r < 0) r = 0;
        if (g < 0) g = 0;
        if (b < 0) b = 0;
        return new COLOR(r, g, b, 1);
    }


    public float InflictDamage(ActorInfo self, DamageInfo dmg)
    {
      if (self.IsDyingOrDead)
        return 0;

      float prevHP = HP;

      // direct shield damage
      float prev = Shd;
      Shd = (Shd - dmg.SpecialShieldDamage).Clamp(0, MaxShd);

      float mod = self.GetArmor(dmg.Type);
      float d = dmg.Value * mod;
      float ds = d - Shd;
      Shd = (Shd - d).Clamp(0, MaxShd);
      float actual = prev - Shd;

      if (d > 0) // damage only
      {
        if (self.IsPlayer)
          if (HP < (int)prevHP) // integer decrement
            self.Engine.PlayerInfo.FlashHit(self.Engine.PlayerInfo.StrengthColor);

        if (Shd <= 0 && !self.IsDyingOrDead) // TO-DO: improve atomicity of ActorState, still may have threading issues
        {
          prev = Hull;
          Hull = (Hull - ds).Clamp(0, MaxHull);
          actual += prev - Hull;
          if (Hull <= 0 && !self.IsDyingOrDead)
          {
             self.SetState_Dying();

#if DEBUG
            if (self.Logged)
              Log.Debug(Globals.LogChannel, LogDecorator.GetFormat(LogType.ACTOR_KILLED), self);
#endif
          }

          if (!self.IsDyingOrDead)
          {
            if (self.TypeInfo.SystemData.AllowSystemDamage)
              for (int i = 0; i < dmg.SpecialSystemDamage; i++)
                self.DamageRandom(dmg.SpecialSystemDamageChanceModifier);

            if (dmg.StunDuration > 0)
              self.InflictStun(dmg.StunDuration);
          }
        }
      }
      return actual;
    }

    public void Kill(ActorInfo self)
    {
      InflictDamage(self, new DamageInfo(MaxHP, DamageType.ALWAYS_100PERCENT));
    }

    public void SetHP(ActorInfo self, float value)
    {
      if (self.IsDyingOrDead)
        return;

      Shd = value.Clamp(-1, MaxShd);

      if (Shd <= 0 && !self.IsDyingOrDead) // TO-DO: improve atomicity of ActorState, still may have threading issues
      {
        self.SetState_Dying();

#if DEBUG
        if (self.Logged)
          Log.Debug(Globals.LogChannel, LogDecorator.GetFormat(LogType.ACTOR_KILLED_BY), self, "setting HP to 0");
#endif
      }
    }

    public void SetMaxHP(float value, bool scaleHP)
    {
      float f = HP_Frac;
      MaxShd = value;
      if (scaleHP)
        Shd = f * value;
      else
        Shd = Shd.Clamp(-1, MaxShd);
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

namespace SWEndor.Game.Actors
{
  public partial class ActorInfo
  {
    internal float InflictDamage(float value, DamageType type, TV_3DVECTOR position, DamageSpecialData specialData)
    {
      using (ScopeCounters.Acquire(Scope))
        return Health.InflictDamage(this, new DamageInfo(value, type, position, specialData));
    }

    public float InflictDamage(float value, DamageType type, TV_3DVECTOR position)
    {
      using (ScopeCounters.Acquire(Scope))
        return Health.InflictDamage(this, new DamageInfo(value, type, position));
    }

    public float InflictDamage(float value, DamageType type)
    {
      using (ScopeCounters.Acquire(Scope))
        return Health.InflictDamage(this, new DamageInfo(value, type));
    }

    public float InflictDamage(float value)
    {
      using (ScopeCounters.Acquire(Scope))
        return Health.InflictDamage(this, new DamageInfo(value, DamageType.ALWAYS_100PERCENT));
    }

    public float HP { get { return Health.HP; } set { using (ScopeCounters.Acquire(Scope)) Health.SetHP(this, value); } }
    public float MaxHP { get { return Health.MaxHP; } set { using (ScopeCounters.Acquire(Scope)) Health.SetMaxHP(value, false); } }
    public float HP_Perc { get { return Health.HP_Perc; } }
    public float HP_Frac { get { return Health.HP_Frac; } }
    public COLOR HP_Color { get { return Health.HP_Color; } }

    public float Shd { get { return Health.Shd; } set { using (ScopeCounters.Acquire(Scope)) Health.Shd = value; } }
    public float MaxShd { get { return Health.MaxShd; } set { using (ScopeCounters.Acquire(Scope)) Health.MaxShd = value; } }
    public float Shd_Perc { get { return Health.Shd_Perc; } }
    public float Shd_Frac { get { return Health.Shd_Frac; } }
    public COLOR Shd_Color { get { return Health.Shd_Color; } }

    public float Hull { get { return Health.Hull; } set { using (ScopeCounters.Acquire(Scope)) Health.Hull = value; } }
    public float MaxHull { get { return Health.MaxHull; } set { using (ScopeCounters.Acquire(Scope)) Health.MaxHull = value; } }
    public float Hull_Perc { get { return Health.Hull_Perc; } }
    public float Hull_Frac { get { return Health.Hull_Frac; } }
    public COLOR Hull_Color { get { return Health.Hull_Color; } }

    public float DisplayHP { get { return Health.DisplayHP; } }
    public float DisplayHP_Perc { get { return Health.DisplayPerc; } }
    public float DisplayHP_Frac { get { return Health.DisplayFrac; } }
  }
}
