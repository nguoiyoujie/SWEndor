using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
using System;
using System.Collections.Generic;

namespace SWEndor.Actors
{
  public enum DamageType
  {
    ALWAYS_100PERCENT = -1,
    NONE,
    NORMAL,
    COLLISION
  }

  public class DamageInfo
  {
    public readonly ActorInfo Source;
    public readonly float Value;
    public readonly DamageType Type;
    public readonly TV_3DVECTOR Position;

    public DamageInfo(ActorInfo source, float value, DamageType type = DamageType.NORMAL, TV_3DVECTOR position = default(TV_3DVECTOR))
    {
      Source = source;
      Value = value;
      Type = type;
      Position = position;
    }
  }

  public struct Health
  {
    public float HP { get; private set; }
    public float MaxHP { get; private set; }
    //public float Level { get; private set; } // ?? for shield implementation?
    public float DisplayHP { get; private set; }

    public bool IsDead { get { return HP <= 0; } }

    public float Frac { get { return HP / MaxHP; } }
    public float Perc { get { return Frac * 100; } }

    public float DisplayFrac { get { return DisplayHP / MaxHP; } }
    public float DisplayPerc { get { return DisplayFrac * 100; } }

    private Dictionary<DamageType, float> DamageModifiers; //move Modifiers to combat data or something


    public void Init(ActorTypeInfo type, ActorCreationInfo acinfo)
    {
      MaxHP = type.MaxStrength;
      HP = (acinfo.InitialStrength > 0) ? acinfo.InitialStrength : type.MaxStrength;
      DamageModifiers = new Dictionary<DamageType, float>();

      // hard code for now
      DamageModifiers.Add(DamageType.NORMAL, type.CombatData.DamageModifier);
      DamageModifiers.Add(DamageType.COLLISION, type.CombatData.CollisionDamageModifier);
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

    public void InflictDamage(ActorInfo target, DamageInfo dmg)
    {
      if (IsDead)
        return;

      float mod = 1;
      DamageModifiers.TryGetValue(dmg.Type, out mod);

      HP = (HP - dmg.Value * mod).Clamp(-1, MaxHP);

      //foreach (INotifyDamage nd in self.TraitsImplementing<INotifyDamage>())
      //  nd.Damaged(self, dmg);

      //if (dmg.Value > 0 && dmg.Source != null && !dmg.Source.Disposed)
      //  foreach (INotifyAppliedDamage nd in dmg.Source.TraitsImplementing<INotifyAppliedDamage>())
      //    nd.AppliedDamage(dmg.Source, self, dmg);

      if (HP <= 0)
      {
        //foreach (INotifyKilled nd in self.TraitsImplementing<INotifyKilled>())
        //  nd.Killed(self, dmg);

        target.SetState_Dying();

        if (dmg.Source == null)
          Log.Write(Log.DEBUG, LogType.ACTOR_KILLED, target);
        else
          Log.Write(Log.DEBUG, LogType.ACTOR_KILLED_BY, target, dmg.Source.TopParent);
      }
    }

    public void Kill(ActorInfo target, ActorInfo attacker)
    {
      InflictDamage(target, new DamageInfo(attacker, MaxHP, DamageType.ALWAYS_100PERCENT));
    }

    public void SetHP(ActorInfo target, float value)
    {
      if (IsDead)
        return;

      HP = value.Clamp(-1, MaxHP);

      if (HP <= 0)
      {
        target.SetState_Dying();
        Log.Write(Log.DEBUG, LogType.ACTOR_KILLED_BY, target, "setting HP to 0");
      }
    }

    public void SetMaxHP(ActorInfo target, float value, bool scaleHP)
    {
      float f = Frac;
      MaxHP = value;
      if (scaleHP)
        HP = f * value;
      else
        HP = HP.Clamp(-1, MaxHP);
    }

    void Tick(float time)
    {
      if (HP > DisplayHP)
        DisplayHP = HP;

      if (DisplayHP > HP)
        DisplayHP = (2 * DisplayHP + HP) / 3;
    }
  }
}
