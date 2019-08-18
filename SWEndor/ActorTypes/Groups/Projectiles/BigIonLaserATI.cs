using MTV3D65;
using SWEndor.AI.Actions;
using System.IO;
using SWEndor.Weapons;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;

namespace SWEndor.ActorTypes.Instances
{
  public class BigIonLaserATI : Groups.LaserProjectile
  {
    internal BigIonLaserATI(Factory owner) : base(owner, "Large Ion Laser")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 30);
      ExplodeData = new ExplodeData(deathTrigger: DeathExplosionTrigger.TIMENOTEXPIRED_ONLY, deathExplosionType: "ExplosionSm", deathExplosionSize: 10);

      ImpactDamage = 50;
      MaxSpeed = Globals.LaserSpeed * 2f;
      MinSpeed = Globals.LaserSpeed * 2f;

      IsLaser = false; // not the same speed
      EnableDistanceCull = false;
      Scale = 4;

      // Projectile
      ImpactCloseEnoughDistance = 150;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\ion_sm_laser.x");
    }

    public override void ProcessHit(int ownerActorID, int hitbyActorID, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      ActorInfo owner = ActorFactory.Get(ownerActorID);
      ActorInfo hitby = ActorFactory.Get(hitbyActorID);

      if (owner == null || hitby == null)
        return;

      base.ProcessHit(ownerActorID, hitbyActorID, impact, normal);
      if (hitby.Children.Length > 0)
      {
        foreach (int i in hitby.Children)
        {
          ActorInfo child = ActorFactory.Get(i);
          CombatSystem.onNotify(Engine, child.ID, CombatEventType.DAMAGE_FRAC, 0.5f);

          float empduration = 10000;
          
          foreach (WeaponInfo w in child.WeaponSystemInfo.Weapons.Values)
            if (w.WeaponCooldown < Game.GameTime + empduration + 2)
              w.WeaponCooldown = Game.GameTime + empduration + 2;

          foreach (int i2 in child.Children)
          {
            ActorInfo child2 = ActorFactory.Get(i2);

            if (child2.TypeInfo is ElectroATI)
            {
              child2.CycleInfo.CyclesRemaining = empduration / child2.TypeInfo.TimedLifeData.TimedLife;
              return;
            }
          }
          ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Get("Electro"));
          acinfo.Position = child.GetPosition();
          ActorInfo electro = ActorInfo.Create(ActorFactory, acinfo);
          child.AddChild(electro.ID);
          electro.CycleInfo.CyclesRemaining = empduration / electro.TypeInfo.TimedLifeData.TimedLife;
        }
      }

      if (hitby.TypeInfo.TargetType.HasFlag(TargetType.SHIP))
      {
        ActionManager.ForceClearQueue(hitbyActorID);
        ActionManager.QueueNext(hitbyActorID, new Rotate(hitby.GetRelativePositionFUR(1000, -800, -200), hitby.MoveData.MaxSpeed, 0.1f, false));
        ActionManager.QueueNext(hitbyActorID, new Lock());
      }
    }
  }
}

