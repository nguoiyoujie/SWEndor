using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
using System;
using System.Collections.Generic;

namespace SWEndor.Actors
{
  [Flags]
  public enum DamageType
  {
    NONE = 0,
    ALWAYS_100PERCENT = 0x0001,
    NORMAL = 0x0010,
    COLLISION = 0x0100,

    ALL = 0xFFFF
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

  public partial class ActorInfo
  { 
    public struct HealthModel
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

      //private Dictionary<DamageType, float> DamageModifiers; //move Modifiers to combat data or something


      public void Init(ActorTypeInfo type, ActorCreationInfo acinfo)
      {
        MaxHP = type.MaxStrength;
        HP = (acinfo.InitialStrength > 0) ? acinfo.InitialStrength : type.MaxStrength;
        //DamageModifiers = new Dictionary<DamageType, float>();

        // hard code for now
        //DamageModifiers.Add(DamageType.NORMAL, type.CombatData.DamageModifier);
        //DamageModifiers.Add(DamageType.COLLISION, type.CombatData.CollisionDamageModifier);
        //DamageModifiers.Add(DamageType.ALWAYS_100PERCENT, 1);
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

        float mod = target.Armor.Get(dmg.Type);

        float d = dmg.Value * mod;
        HP = (HP - d).Clamp(-1, MaxHP);

        /*
        if (d > 0)
          Log.Write(Log.DEBUG, LogType.ACTOR_DAMAGED, target, dmg.Source, d, HP);
        else if (d < 0)
          Log.Write(Log.DEBUG, LogType.ACTOR_HEALED, target, dmg.Source, -d, HP);
        */

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

          if (target.Logged)
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

          if (target.Logged)
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

      public void Tick(float time)
      {
        if (HP > DisplayHP)
          DisplayHP = HP;

        if (DisplayHP > HP)
          DisplayHP = (2 * DisplayHP + HP) / 3;
      }
    }

    public void InflictDamage(ActorInfo attacker, float value, DamageType type, TV_3DVECTOR position) { Health.InflictDamage(this, new DamageInfo(attacker, value, type, position)); }
    public void InflictDamage(ActorInfo attacker, float value, DamageType type) { Health.InflictDamage(this, new DamageInfo(attacker, value, type)); }
    public void InflictDamage(ActorInfo attacker, float value) { Health.InflictDamage(this, new DamageInfo(attacker, value)); }
    public void Kill(ActorInfo attacker = null) { Health.Kill(this, attacker); }

    public float HP { get { return Health.HP; } set { Health.SetHP(this, value); } }
    public float MaxHP { get { return Health.MaxHP; } set { Health.SetMaxHP(this, value, false); } }
    public float HP_Perc { get { return Health.Perc; } }
    public float HP_Frac { get { return Health.Frac; } }
    public TV_COLOR HP_Color { get { return Health.Color; } }

    public float DisplayHP { get { return Health.DisplayHP; } }
    public float DisplayHP_Perc { get { return Health.DisplayPerc; } }
    public float DisplayHP_Frac { get { return Health.DisplayFrac; } }
  }
}
