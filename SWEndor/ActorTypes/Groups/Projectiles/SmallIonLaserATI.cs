using MTV3D65;
using SWEndor.Actors;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class SmallIonLaserATI : Group.Projectile
  {
    internal SmallIonLaserATI(Factory owner) : base(owner, "Ion Laser")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 5;
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = true;
      ImpactDamage = 5;
      MaxSpeed = Globals.LaserSpeed * 0.6f;
      MinSpeed = Globals.LaserSpeed * 0.6f;

      NoAI = true;
      IsLaser = true;

      // Projectile
      ImpactCloseEnoughDistance = 75;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\ion_sm_laser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.ExplosionInfo.DeathExplosionType = "Explosion";
    }

    public override void ProcessHit(int ownerActorID, int hitbyActorID, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      ActorInfo owner = ActorFactory.Get(ownerActorID);
      ActorInfo hitby = ActorFactory.Get(hitbyActorID);

      if (owner == null || hitby == null)
        return;

      base.ProcessHit(ownerActorID, hitbyActorID, impact, normal);
      /*
      List<int> children = hitby.Children;
      List<int> rm = new List<int>();
      foreach (int i in children)
      {
        ActorInfo c = ActorFactory.Get(i);
        if (c == null
          || c.CreationState != CreationState.ACTIVE
          || !c.TypeInfo.TargetType.HasFlag(TargetType.ADDON))
          rm.Add(c.ID);
      }

      foreach (int r in rm)
        children.Remove(r);
        */
      if (hitby.Children.Length > 0)
      {
        for (int shock = 3; shock > 0; shock--)
        {
          ActorInfo child = ActorFactory.Get(hitby.Children[Engine.Random.Next(0, hitby.Children.Length)]);
          child.CombatInfo.onNotify(Actors.Components.CombatEventType.DAMAGE, 0.1f * Engine.Random.Next(25, 50));

          float empduration = 12;
          
          foreach (WeaponInfo w in child.WeaponSystemInfo.Weapons.Values)
            if (w.WeaponCooldown < Game.GameTime + empduration + 2)
              w.WeaponCooldown = Game.GameTime + empduration + 2;

          foreach (int i in child.Children)
          {
            ActorInfo child2 = ActorFactory.Get(i);
            if (child2.TypeInfo is ElectroATI)
            {
              child2.CycleInfo.CyclesRemaining = empduration / child2.CycleInfo.CyclePeriod;
              return;
            }
          }
          ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Get("Electro"));
          acinfo.Position = child.GetPosition();
          ActorInfo electro = ActorInfo.Create(ActorFactory, acinfo);
          child.AddChild(electro.ID);
          electro.CycleInfo.CyclesRemaining = empduration / electro.CycleInfo.CyclePeriod;
        }
      }
    }
  }
}

