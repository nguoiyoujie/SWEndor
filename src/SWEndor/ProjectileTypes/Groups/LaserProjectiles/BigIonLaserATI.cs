using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ProjectileTypes.Instances
{
  internal class BigIonLaserATI : Groups.LaserProjectile
  {
    internal BigIonLaserATI(Factory owner) : base(owner, "LSR_IONBIG", "Large Ion Laser")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 30);
      Explodes = new ExplodeData[] {
        new ExplodeData("EXPL00", 1, 10, ExplodeTrigger.ON_DEATH | ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED)
      };

      CombatData.ImpactDamage = 50;
      MoveLimitData.MaxSpeed = Globals.LaserSpeed * 2f;
      MoveLimitData.MinSpeed = Globals.LaserSpeed * 2f;

      CombatData.IsLaser = false; // not the same speed

      // Projectile
      CombatData.ImpactCloseEnoughDistance = 150;

      DamageSpecialData.EMPAffectsChildren = -1;
      DamageSpecialData.EMPPercentDamage = 0.45f;
      DamageSpecialData.EMPPercentDamageRandom = 0.1f;
      DamageSpecialData.EMPDuration = 10000;

      MeshData = new MeshData(Name, @"projectiles\ion_sm_laser.x", 4, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Laser");
    }

    /*
    public override void ProcessHit(Engine engine, ProjectileInfo owner, ActorInfo hitby, TV_3DVECTOR impact)
    {
      if (owner == null || hitby == null)
        return;

      base.ProcessHit(engine, owner, hitby, impact);
      foreach (ActorInfo child in hitby.Children)
      {
        child.InflictDamage(0.5f * child.HP, DamageType.NORMAL, child.GetGlobalPosition());

        float empduration = 10000;

        for (int i = 0; i < child.WeaponDefinitions.Weapons.Length; i++)
          if (child.WeaponDefinitions.Weapons[i].WeaponCooldown < engine.Game.GameTime + empduration + 2)
            child.WeaponDefinitions.Weapons[i].WeaponCooldown = engine.Game.GameTime + empduration + 2;

        ExplosionCreationInfo acinfo = new ExplosionCreationInfo(engine.ExplosionTypeFactory.Get("ELECTRO"));
        ExplosionInfo electro = engine.ExplosionFactory.Create(acinfo);
        electro.AttachedActorID = child.ID;
        electro.CycleInfo.CyclesRemaining = empduration / electro.TypeInfo.TimedLifeData.TimedLife;
      }

      if (hitby.TypeInfo.AIData.TargetType.Has(TargetType.SHIP))
      {
        hitby.ForceClearQueue();
        hitby.QueueNext(new Rotate(hitby.GetRelativePositionFUR(1000, -800, -200), hitby.MoveData.MaxSpeed, 0.1f, false));
        hitby.QueueNext(new Lock());
      }
    }
    */
  }
}

