using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Explosions;
using SWEndor.ExplosionTypes;
using Primitives.FileFormat.INI;
using SWEndor.Models;
using System.Collections.Generic;

namespace SWEndor.Projectiles.Components
{
  internal struct DamageSpecialData
  {
    public bool NeverDisappear;
    public float EMPDuration;
    public float EMPDamage;
    public float EMPDamageRandom;
    public float EMPPercentDamage;
    public float EMPPercentDamageRandom;
    public int EMPAffectsChildren;
    public float ReduceDyingTimerTo;

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

    public void LoadFromINI(INIFile f, string sectionname)
    {
      this = new DamageSpecialData();
      NeverDisappear = f.GetBool(sectionname, "NeverDisappear", NeverDisappear);
      EMPDuration = f.GetFloat(sectionname, "EMPDuration", EMPDuration);
      EMPDamage = f.GetFloat(sectionname, "EMPDamage", EMPDamage);
      EMPDamageRandom = f.GetFloat(sectionname, "EMPDamageRandom", EMPDamageRandom);
      EMPPercentDamage = f.GetFloat(sectionname, "EMPPercentDamage", EMPPercentDamage);
      EMPPercentDamageRandom = f.GetFloat(sectionname, "EMPPercentDamageRandom", EMPPercentDamageRandom);
      EMPAffectsChildren = f.GetInt(sectionname, "EMPAffectsChildren", EMPAffectsChildren);
      ReduceDyingTimerTo = f.GetFloat(sectionname, "ReduceDyingTimerTo", ReduceDyingTimerTo);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetBool(sectionname, "NeverDisappear", NeverDisappear);
      f.SetFloat(sectionname, "EMPDuration", EMPDuration);
      f.SetFloat(sectionname, "EMPDamage", EMPDamage);
      f.SetFloat(sectionname, "EMPDamageRandom", EMPDamageRandom);
      f.SetFloat(sectionname, "EMPPercentDamage", EMPPercentDamage);
      f.SetFloat(sectionname, "EMPPercentDamageRandom", EMPPercentDamageRandom);
      f.SetInt(sectionname, "EMPAffectsChildren", EMPAffectsChildren);
      f.SetFloat(sectionname, "ReduceDyingTimerTo", ReduceDyingTimerTo);
    }
  }
}
