using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Explosions;
using SWEndor.ExplosionTypes;
using Primrose.FileFormat.INI;
using SWEndor.Models;
using System.Collections.Generic;

namespace SWEndor.Projectiles.Components
{
  internal struct DamageSpecialData
  {
#pragma warning disable 0649 // values are filled by the attribute
    [INIValue]
    public bool NeverDisappear;

    [INIValue]
    public float EMPDuration;

    [INIValue]
    public float EMPDamage;

    [INIValue]
    public float EMPDamageRandom;

    [INIValue]
    public float EMPPercentDamage;

    [INIValue]
    public float EMPPercentDamageRandom;

    [INIValue]
    public int EMPAffectsChildren;

    [INIValue]
    public float ReduceDyingTimerTo;
#pragma warning restore 0649
    
    public void ProcessHit(Engine engine, ActorInfo target)
    {
      if (ReduceDyingTimerTo > 0)
        if (target.DyingTimeRemaining > ReduceDyingTimerTo)
          target.DyingTimerSet(ReduceDyingTimerTo, false);

      if (EMPAffectsChildren != 0 && EMPDuration > 0)
      {
        List<ActorInfo> children = new List<ActorInfo>();
        foreach (ActorInfo c in target.Children)
        {
          if (c != null
            && c.Active
            && c.TypeInfo.AIData.TargetType.Has(TargetType.ADDON))
            children.Add(c);
        }

        if (children.Count > 0)
        {
          if (EMPAffectsChildren == -1)
          {
            foreach (ActorInfo child in children)
            {
              float dmg = EMPDamage * (float)(engine.Random.NextDouble() * EMPDamageRandom) + (EMPPercentDamage + (float)(engine.Random.NextDouble() * EMPPercentDamageRandom)) * child.HP;
              child.InflictDamage(dmg, DamageType.ALWAYS_100PERCENT, child.GetGlobalPosition());

              for (int i = 0; i < child.WeaponDefinitions.Weapons.Length; i++)
                if (child.WeaponDefinitions.Weapons[i].Port.Cooldown < engine.Game.GameTime + EMPDuration + 2)
                  child.WeaponDefinitions.Weapons[i].Port.Cooldown = engine.Game.GameTime + EMPDuration + 2;

              // TO-DO: Replace this with configurable
              ExplosionCreationInfo acinfo = new ExplosionCreationInfo(engine.ExplosionTypeFactory.Get("ELECTRO"));
              ExplosionInfo electro = engine.ExplosionFactory.Create(acinfo);
              electro.AttachedActorID = child.ID;
              electro.AnimInfo.CyclesRemaining = EMPDuration / electro.TypeInfo.TimedLifeData.TimedLife;
            }
          }
          else if (EMPAffectsChildren > 0)
          {
            for (int shock = EMPAffectsChildren; shock > 0; shock--)
            {
              ActorInfo child = children[engine.Random.Next(0, children.Count)];
              float dmg = EMPDamage * (float)(engine.Random.NextDouble() * EMPDamageRandom) + (EMPPercentDamage + (float)(engine.Random.NextDouble() * EMPPercentDamageRandom)) * child.HP;
              child.InflictDamage(dmg, DamageType.ALWAYS_100PERCENT, child.GetGlobalPosition());

              for (int i = 0; i < child.WeaponDefinitions.Weapons.Length; i++)
                if (child.WeaponDefinitions.Weapons[i].Port.Cooldown < engine.Game.GameTime + EMPDuration + 2)
                  child.WeaponDefinitions.Weapons[i].Port.Cooldown = engine.Game.GameTime + EMPDuration + 2;

              // TO-DO: Replace this with configurable
              ExplosionCreationInfo acinfo = new ExplosionCreationInfo(engine.ExplosionTypeFactory.Get("ELECTRO"));
              ExplosionInfo electro = engine.ExplosionFactory.Create(acinfo);
              electro.AttachedActorID = child.ID;
              electro.AnimInfo.CyclesRemaining = EMPDuration / electro.TypeInfo.TimedLifeData.TimedLife;
            }
          }
        }
      }
    }
  }
}
