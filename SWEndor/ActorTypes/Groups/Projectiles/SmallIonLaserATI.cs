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

      // Projectile
      ImpactCloseEnoughDistance = 75;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\ion_sm_laser.x");
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      if (ainfo.ActorState == ActorState.DYING || ainfo.ActorState == ActorState.DEAD)
      {
        ActorCreationInfo acinfo = new ActorCreationInfo(Globals.Engine.ActorTypeFactory.Get("ExplosionSm"));
        acinfo.Position = ainfo.GetPosition();
        ActorInfo.Create(Owner.Engine.ActorFactory, acinfo);

        ainfo.ActorState = ActorState.DEAD;
      }
    }

    public override void ProcessHit(int ownerActorID, int hitbyActorID, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      ActorInfo owner = Owner.Engine.ActorFactory.Get(ownerActorID);
      ActorInfo hitby = Owner.Engine.ActorFactory.Get(hitbyActorID);

      if (owner == null || hitby == null)
        return;

      base.ProcessHit(ownerActorID, hitbyActorID, impact, normal);
      List<int> children = hitby.GetAllChildren(1);
      List<int> rm = new List<int>();
      foreach (int i in children)
      {
        ActorInfo c = Owner.Engine.ActorFactory.Get(i);
        if (c == null
          || c.CreationState != CreationState.ACTIVE
          || !c.TypeInfo.TargetType.HasFlag(TargetType.ADDON))
          rm.Add(c.ID);
      }

      foreach (int r in rm)
        children.Remove(r);

      if (children.Count > 0)
      {
        for (int shock = 3; shock > 0; shock--)
        {
          ActorInfo child = Owner.Engine.ActorFactory.Get(children[Globals.Engine.Random.Next(0, children.Count)]);
          child.CombatInfo.Strength -= 0.1f * Globals.Engine.Random.Next(25, 50);

          float empduration = 12;
          
          foreach (WeaponInfo w in child.WeaponSystemInfo.Weapons.Values)
            if (w.WeaponCooldown < Globals.Engine.Game.GameTime + empduration + 2)
              w.WeaponCooldown = Globals.Engine.Game.GameTime + empduration + 2;

          foreach (int i in child.GetAllChildren(1))
          {
            ActorInfo child2 = Owner.Engine.ActorFactory.Get(i);
            if (child2.TypeInfo is ElectroATI)
            {
              child2.CycleInfo.CyclesRemaining = empduration / child2.CycleInfo.CyclePeriod;
              return;
            }
          }
          ActorCreationInfo acinfo = new ActorCreationInfo(Owner.Get("Electro"));
          acinfo.Position = child.GetPosition();
          ActorInfo electro = ActorInfo.Create(Owner.Engine.ActorFactory, acinfo);
          electro.AddParent(child.ID);
          electro.CycleInfo.CyclesRemaining = empduration / electro.CycleInfo.CyclePeriod;
        }
      }
    }
  }
}

