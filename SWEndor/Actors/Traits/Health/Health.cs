using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
using SWEndor.Primitives.Traits;
using System;
using System.Collections.Generic;

namespace SWEndor.Actors.Traits
{
  public enum DamageType
  {
    ALWAYS_100PERCENT = -1,
    NONE,
    NORMAL,
    COLLISION
  }

  public class DamageInfo<A>
  {
    public readonly A Source;
    public readonly float Value;
    public readonly DamageType Type;
    public readonly TV_3DVECTOR Position;

    public DamageInfo(A source, float value, DamageType type = DamageType.NORMAL, TV_3DVECTOR position = default(TV_3DVECTOR))
    {
      Source = source;
      Value = value;
      Type = type;
      Position = position;
    }
  }

  public class Health : IHealth, ITick
  {
    public float HP { get; private set; }
    public float MaxHP { get; private set; }
    public float Level { get; private set; } // ?? 0 for base, higher for 'shields'
    public float DisplayHP { get; private set; }

    public bool IsDead { get { return HP <= 0; } }

    public float Frac { get { return HP / MaxHP; } }
    public float Perc { get { return Frac * 100; } }

    public float DisplayFrac { get { return DisplayHP / MaxHP; } }
    public float DisplayPerc { get { return DisplayFrac * 100; } }

    public readonly ThreadSafeDictionary<DamageType, float> DamageModifiers = new ThreadSafeDictionary<DamageType, float>();


    public void Init(ActorTypeInfo type, ActorCreationInfo acinfo)
    {
      MaxHP = type.MaxStrength;
      HP = (acinfo.InitialStrength > 0) ? acinfo.InitialStrength : type.MaxStrength;

      DamageModifiers.Put(DamageType.NORMAL, type.CombatData.DamageModifier);
      DamageModifiers.Put(DamageType.COLLISION, type.CombatData.CollisionDamageModifier);
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

    public void InflictDamage<A1,A2>(A1 self, DamageInfo<A2> dmg)
      where A1 : ITraitOwner
      where A2 : ITraitOwner
    {
      if (IsDead)
        return;

      float mod = (DamageModifiers.ContainsKey(dmg.Type)) ? DamageModifiers.Get(dmg.Type) : 1;

      HP = (HP - dmg.Value * mod).Clamp(-1, MaxHP);

      foreach (INotifyDamage nd in self.TraitsImplementing<INotifyDamage>())
        nd.Damaged(self, dmg);

      if (dmg.Value > 0 && dmg.Source != null && !dmg.Source.Disposed)
        foreach (INotifyAppliedDamage nd in dmg.Source.TraitsImplementing<INotifyAppliedDamage>())
          nd.AppliedDamage(dmg.Source, self, dmg);

      if (HP <= 0)
      {
        foreach (INotifyKilled nd in self.TraitsImplementing<INotifyKilled>())
          nd.Killed(self, dmg);

        self.Trait<IStateModel>().MakeDying(self);

        if (dmg.Source == null)
          Log.Write(Log.DEBUG, "'{0}' was killed.", self);
        else
          Log.Write(Log.DEBUG, "'{0}' killed by '{1}'", self, dmg.Source);
      }
    }

    public void Kill<A1, A2>(A1 self, A2 attacker)
      where A1 : ITraitOwner
      where A2 : ITraitOwner
    {
      InflictDamage(self, new DamageInfo<A2>(attacker, MaxHP, DamageType.ALWAYS_100PERCENT));
    }

    public void SetHP<A1>(A1 self, float value)
      where A1 : ITraitOwner
    {
      if (IsDead)
        return;

      HP = value.Clamp(-1, MaxHP);

      if (HP <= 0)
      {
        self.Trait<IStateModel>().MakeDying(self);
        Log.Write(Log.DEBUG, "{0} #{1} was killed by setting HP to 0.", self.Name, self.ID);
      }
    }

    public void SetMaxHP<A1>(A1 self, float value, bool scaleHP)
      where A1 : ITraitOwner
    {
      float f = Frac;
      MaxHP = value;
      if (scaleHP)
        HP = f * value;
      else
        HP = HP.Clamp(-1, MaxHP);
    }

    void ITick.Tick<A>(A self, float time)
    {
      if (HP > DisplayHP)
        DisplayHP = HP;

      if (DisplayHP > HP)
        DisplayHP = (2 * DisplayHP + HP) / 3;
    }
  }
}
