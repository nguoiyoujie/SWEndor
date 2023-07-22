using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using Primrose.FileFormat.INI;
using System.Collections.Generic;

namespace SWEndor.Game.Models
{
  internal struct DamageSpecialData
  {
#pragma warning disable 0649 // values are filled by the attribute
    [INIValue]
    public bool NeverDisappear;

    // Anti-shield
    [INIValue]
    public float ShieldDamage;

    // Anti-System
    [INIValue]
    public int SystemDamageCount;

    [INIValue]
    public float SystemDamageChanceModifier;

    // Stun
    [INIValue]
    public float StunDuration;

    // Anti-Large Ship Subsystems
    [INIValue]
    public float AddonStunDuration;

    [INIValue]
    public float AddonDamage;

    [INIValue]
    public float AddonDamageRandom;

    [INIValue]
    public float AddonPercentDamage;

    [INIValue]
    public float AddonPercentDamageRandom;

    [INIValue]
    public int AffectsAddonNumber;

    [INIValue]
    public float ReduceDyingTimerTo;
#pragma warning restore 0649

    private static List<ActorInfo> _children = new List<ActorInfo>();

    public readonly static DamageSpecialData Default =
      new DamageSpecialData
      {
        SystemDamageCount = 1,
        SystemDamageChanceModifier = 1
      };

    // perhaps move this to HealthModel.InflictDamage?
    public void ProcessHit(Engine engine, ActorInfo target)
    {
      if (ReduceDyingTimerTo > 0)
        if (target.DyingTimeRemaining > ReduceDyingTimerTo)
          target.DyingTimerSet(ReduceDyingTimerTo, false);

      if (AffectsAddonNumber != 0)
      {
        lock (_children)
        {
          _children.Clear();
          foreach (ActorInfo c in target.Children)
          {
            if (c != null
              && c.Active
              && c.TargetType.Has(TargetType.ADDON))
              _children.Add(c);
          }

          if (_children.Count > 0)
          {
            if (AffectsAddonNumber == -1) // all addons
            {
              foreach (ActorInfo child in _children)
              {
                float dmg = AddonDamage * (float)(engine.Random.NextDouble() * AddonDamageRandom) + (AddonPercentDamage + (float)(engine.Random.NextDouble() * AddonPercentDamageRandom)) * child.HP;
                child.InflictDamage(dmg, DamageType.ALWAYS_100PERCENT, child.GetGlobalPosition());
                if (AddonStunDuration > 0) { child.InflictStun(AddonStunDuration); }

                //or (int i = 0; i < child.WeaponDefinitions.Weapons.Length; i++)
                // if (child.WeaponDefinitions.Weapons[i].Port.Cooldown < engine.Game.GameTime + EMPDuration + 2)
                //   child.WeaponDefinitions.Weapons[i].Port.Cooldown = engine.Game.GameTime + EMPDuration + 2;
                //
                /// TO-DO: Replace this with configurable
                //xplosionCreationInfo acinfo = new ExplosionCreationInfo(engine.ExplosionTypeFactory.Get("ELECTRO"));
                //xplosionInfo electro = engine.ExplosionFactory.Create(acinfo);
                //lectro.AttachedActorID = child.ID;
                //lectro.AnimInfo.CyclesRemaining = EMPDuration / electro.TypeInfo.TimedLifeData.TimedLife;
              }
            }
            else if (AffectsAddonNumber > 0)
            {
              for (int shock = AffectsAddonNumber; shock > 0; shock--)
              {
                ActorInfo child = _children[engine.Random.Next(0, _children.Count)];
                float dmg = AddonDamage * (float)(engine.Random.NextDouble() * AddonDamageRandom) + (AddonPercentDamage + (float)(engine.Random.NextDouble() * AddonPercentDamageRandom)) * child.HP;
                child.InflictDamage(dmg, DamageType.ALWAYS_100PERCENT, child.GetGlobalPosition());
                if (AddonStunDuration > 0) { child.InflictStun(AddonStunDuration); }

                //for (int i = 0; i < child.WeaponDefinitions.Weapons.Length; i++)
                //  if (child.WeaponDefinitions.Weapons[i].Port.Cooldown < engine.Game.GameTime + EMPDuration + 2)
                //    child.WeaponDefinitions.Weapons[i].Port.Cooldown = engine.Game.GameTime + EMPDuration + 2;
                //
                //// TO-DO: Replace this with configurable
                //ExplosionCreationInfo acinfo = new ExplosionCreationInfo(engine.ExplosionTypeFactory.Get("ELECTRO"));
                //ExplosionInfo electro = engine.ExplosionFactory.Create(acinfo);
                //electro.AttachedActorID = child.ID;
                //electro.AnimInfo.CyclesRemaining = EMPDuration / electro.TypeInfo.TimedLifeData.TimedLife;
              }
            }
          }
        }
      }
    }
  }
}
