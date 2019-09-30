using MTV3D65;
using SWEndor.AI.Actions;
using SWEndor.Actors;
using SWEndor.AI;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.ExplosionTypes;
using SWEndor.Explosions;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  public class BigIonLaserATI : Groups.LaserProjectile
  {
    internal BigIonLaserATI(Factory owner) : base(owner, "Large Ion Laser")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 30);
      Explodes = new ExplodeData[] {
        new ExplodeData("ExpL00", 1, 10, ExplodeTrigger.ON_DEATH | ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED)
      };

      ImpactDamage = 50;
      MoveLimitData.MaxSpeed = Globals.LaserSpeed * 2f;
      MoveLimitData.MinSpeed = Globals.LaserSpeed * 2f;

      IsLaser = false; // not the same speed

      // Projectile
      ImpactCloseEnoughDistance = 150;

      MeshData = new MeshData(Name, @"projectiles\ion_sm_laser.x", 4);
    }

    public override void ProcessHit(Engine engine, ActorInfo owner, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      if (owner == null || hitby == null)
        return;

      base.ProcessHit(engine, owner, hitby, impact, normal);
      foreach (ActorInfo child in hitby.Children)
      {
        child.InflictDamage(hitby, 0.5f * child.HP, DamageType.NORMAL, child.GetGlobalPosition());

        float empduration = 10000;

        for (int i = 0; i < child.WeaponDefinitions.Weapons.Length; i++)
          if (child.WeaponDefinitions.Weapons[i].WeaponCooldown < engine.Game.GameTime + empduration + 2)
            child.WeaponDefinitions.Weapons[i].WeaponCooldown = engine.Game.GameTime + empduration + 2;

        ExplosionCreationInfo acinfo = new ExplosionCreationInfo(engine.ExplosionTypeFactory.Get("Electro"));
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
  }
}

