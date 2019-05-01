using MTV3D65;
using SWEndor.AI.Actions;
using System.Collections.Generic;
using System.IO;
using SWEndor.Weapons;
using SWEndor.Actors;
using SWEndor.Actors.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class BigIonLaserATI : Group.Projectile
  {
    internal BigIonLaserATI(Factory owner) : base(owner, "Large Ion Laser")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 30;
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = true;
      ImpactDamage = 50;
      MaxSpeed = Globals.LaserSpeed * 2f;
      MinSpeed = Globals.LaserSpeed * 2f;

      NoAI = true;
      IsLaser = true;
      EnableDistanceCull = false;
      // Projectile
      ImpactCloseEnoughDistance = 150;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\ion_sm_laser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.ExplosionInfo.DeathExplosionType = "ExplosionSm";
      ainfo.ExplosionInfo.DeathExplosionSize = 10;

      ainfo.Scale *= 4;
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
          child.CombatInfo.onNotify(Actors.Components.CombatEventType.DAMAGE, 0.5f * child.CombatInfo.MaxStrength);
          float empduration = 10000;
          
          foreach (WeaponInfo w in child.WeaponSystemInfo.Weapons.Values)
            if (w.WeaponCooldown < Game.GameTime + empduration + 2)
              w.WeaponCooldown = Game.GameTime + empduration + 2;

          foreach (int i2 in child.Children)
          {
            ActorInfo child2 = ActorFactory.Get(i2);

            if (child2.TypeInfo is ElectroATI)
            {
              child2.CycleInfo.CyclesRemaining = empduration / child2.TypeInfo.TimedLife;
              return;
            }
          }
          ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Get("Electro"));
          acinfo.Position = child.GetPosition();
          ActorInfo electro = ActorInfo.Create(ActorFactory, acinfo);
          child.AddChild(electro.ID);
          electro.CycleInfo.CyclesRemaining = empduration / electro.TypeInfo.TimedLife;
        }
      }

      if (hitby.TypeInfo.TargetType.HasFlag(TargetType.SHIP))
      {
        ActionManager.ForceClearQueue(hitbyActorID);
        ActionManager.QueueNext(hitbyActorID, new Rotate(hitby.GetRelativePositionFUR(1000, -800, -200), hitby.MoveComponent.MaxSpeed, 0.1f, false));
        ActionManager.QueueNext(hitbyActorID, new Lock());
      }
    }
  }
}

