using MTV3D65;
using SWEndor.AI;
using SWEndor.AI.Actions;
using System.Collections.Generic;
using System.IO;
using SWEndor.Weapons;
using SWEndor.Actors;

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
      EnableDistanceCull = false;
      // Projectile
      ImpactCloseEnoughDistance = 150;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\ion_sm_laser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.Scale *= 4;
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      if (ainfo.ActorState == ActorState.DYING || ainfo.ActorState == ActorState.DEAD)
      {
        ActorCreationInfo acinfo = new ActorCreationInfo(Globals.Engine.ActorTypeFactory.Get("ExplosionSm"));
        acinfo.Position = ainfo.GetPosition();
        ActorInfo expl = ActorInfo.Create(Owner.Engine.ActorFactory, acinfo);
        expl.Scale *= 10;

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
        foreach (int i in children)
        {
          ActorInfo child = Owner.Engine.ActorFactory.Get(i);
          child.CombatInfo.Strength -= 0.5f * child.CombatInfo.MaxStrength;
          float empduration = 10000;
          
          foreach (WeaponInfo w in child.WeaponSystemInfo.Weapons.Values)
            if (w.WeaponCooldown < Globals.Engine.Game.GameTime + empduration + 2)
              w.WeaponCooldown = Globals.Engine.Game.GameTime + empduration + 2;

          foreach (int i2 in child.GetAllChildren(1))
          {
            ActorInfo child2 = Owner.Engine.ActorFactory.Get(i2);

            if (child2.TypeInfo is ElectroATI)
            {
              child2.CycleInfo.CyclesRemaining = empduration / child2.TypeInfo.TimedLife;
              return;
            }
          }
          ActorCreationInfo acinfo = new ActorCreationInfo(Owner.Get("Electro"));
          acinfo.Position = child.GetPosition();
          ActorInfo electro = ActorInfo.Create(Owner.Engine.ActorFactory, acinfo);
          electro.AddParent(child.ID);
          electro.CycleInfo.CyclesRemaining = empduration / electro.TypeInfo.TimedLife;
        }
      }

      if (hitby.TypeInfo.TargetType.HasFlag(TargetType.SHIP))
      {
        Owner.Engine.ActionManager.ForceClearQueue(hitbyActorID);
        Owner.Engine.ActionManager.QueueNext(hitbyActorID, new Rotate(hitby.GetRelativePositionFUR(1000, -800, -200), hitby.MovementInfo.MaxSpeed, 0.1f, false));
        Owner.Engine.ActionManager.QueueNext(hitbyActorID, new Lock());
      }
    }
  }
}

