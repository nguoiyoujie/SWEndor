using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Explosions;
using SWEndor.ExplosionTypes;
using SWEndor.Projectiles;
using System.Collections.Generic;

namespace SWEndor.ProjectileTypes.Instances
{
  public class SmallIonLaserATI : Groups.LaserProjectile
  {
    internal SmallIonLaserATI(Factory owner) : base(owner, "LSR_ION", "Ion Laser")
    {
      TimedLifeData = new TimedLifeData(true, 5);

      CombatData.ImpactDamage = 5;
      MoveLimitData.MaxSpeed = Globals.LaserSpeed * 0.6f;
      MoveLimitData.MinSpeed = Globals.LaserSpeed * 0.6f;

      CombatData.ImpactCloseEnoughDistance = 75;
      CombatData.IsLaser = false; // not the same speed

      MeshData = new MeshData(Name, @"projectiles\ion_sm_laser.x", 1, "Laser");
    }

    public override void ProcessHit(Engine engine, ProjectileInfo owner, ActorInfo hitby, TV_3DVECTOR impact)
    {
      if (owner == null || hitby == null)
        return;

      base.ProcessHit(engine, owner, hitby, impact);

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
          child.InflictDamage(0.1f * Engine.Random.Next(25, 50), DamageType.NORMAL, child.GetGlobalPosition());

          float empduration = 12;

          for (int i = 0; i < child.WeaponDefinitions.Weapons.Length; i++)
            if (child.WeaponDefinitions.Weapons[i].WeaponCooldown < engine.Game.GameTime + empduration + 2)
              child.WeaponDefinitions.Weapons[i].WeaponCooldown = engine.Game.GameTime + empduration + 2;

          ExplosionCreationInfo acinfo = new ExplosionCreationInfo(engine.ExplosionTypeFactory.Get("ELECTRO"));
          ExplosionInfo electro = engine.ExplosionFactory.Create(acinfo);
          electro.AttachedActorID = child.ID;
          electro.CycleInfo.CyclesRemaining = empduration / electro.TypeInfo.TimedLifeData.TimedLife;
        }
      }
    }
  }
}

