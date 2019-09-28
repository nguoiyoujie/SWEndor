using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Explosions;
using SWEndor.ExplosionTypes;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class SmallIonLaserATI : Groups.LaserProjectile
  {
    internal SmallIonLaserATI(Factory owner) : base(owner, "Ion Laser")
    {
      TimedLifeData = new TimedLifeData(true, 5);

      ImpactDamage = 5;
      MoveLimitData.MaxSpeed = Globals.LaserSpeed * 0.6f;
      MoveLimitData.MinSpeed = Globals.LaserSpeed * 0.6f;

      ImpactCloseEnoughDistance = 75;
      IsLaser = false; // not the same speed

      MeshData = new MeshData(Name, @"projectiles\ion_sm_laser.x");
    }

    public override void ProcessHit(Engine engine, ActorInfo owner, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      if (owner == null || hitby == null)
        return;

      base.ProcessHit(engine, owner, hitby, impact, normal);

      List<ActorInfo> children = new List<ActorInfo>(); // hitby.Children);
      foreach (ActorInfo c in hitby.Children)
      {
        if (c != null
          && c.Active
          && c.TypeInfo.AIData.TargetType.Has(TargetType.ADDON))
          children.Add(c);
      }

      if (children.Count > 0)
      {
        for (int shock = 3; shock > 0; shock--)
        {
          ActorInfo child = children[Engine.Random.Next(0, children.Count)];
          child.InflictDamage(hitby, 0.1f * Engine.Random.Next(25, 50), DamageType.NORMAL, child.GetGlobalPosition());

          float empduration = 12;

          for (int i = 0; i < child.WeaponDefinitions.Weapons.Length; i++)
            if (child.WeaponDefinitions.Weapons[i].WeaponCooldown < Game.GameTime + empduration + 2)
              child.WeaponDefinitions.Weapons[i].WeaponCooldown = Game.GameTime + empduration + 2;

          /*
          foreach (ActorInfo child2 in child.Children)
          {
            if (child2.TypeInfo is ElectroATI)
            {
              child2.CycleInfo.CyclesRemaining = empduration / child2.TypeInfo.TimedLifeData.TimedLife;
              return;
            }
          }
          */
          ExplosionCreationInfo acinfo = new ExplosionCreationInfo(engine.ExplosionTypeFactory.Get("Electro"));
          ExplosionInfo electro = engine.ExplosionFactory.Create(acinfo);
          electro.AttachedActorID = child.ID;
          electro.CycleInfo.CyclesRemaining = empduration / electro.TypeInfo.TimedLifeData.TimedLife;
        }
      }
    }
  }
}

