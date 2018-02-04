using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class BigIonLaserATI : ProjectileGroup
  {
    private static BigIonLaserATI _instance;
    public static BigIonLaserATI Instance()
    {
      if (_instance == null) { _instance = new BigIonLaserATI(); }
      return _instance;
    }

    private BigIonLaserATI() : base("Large Ion Cannon Laser")
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
        ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("ExplosionSm"));
        acinfo.Position = ainfo.GetPosition();
        ActorInfo expl = ActorInfo.Create(acinfo);
        expl.Scale *= 10;

        ainfo.ActorState = ActorState.DEAD;
      }
    }

    public override void ProcessHit(ActorInfo ainfo, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(ainfo, hitby, impact, normal);
      List<ActorInfo> children = hitby.GetAllChildren(1);
      List<ActorInfo> rm = new List<ActorInfo>();
      foreach (ActorInfo c in children)
      {
        if (c.CreationState != CreationState.ACTIVE
          || !c.TypeInfo.IsHardPointAddon)
          rm.Add(c);
      }

      foreach (ActorInfo r in rm)
      {
        children.Remove(r);
      }

      if (children.Count > 0)
      {
        foreach (ActorInfo child in children)
        {
          child.Strength -= 0.5f * child.MaxStrength;

          float empduration = 10000;
          
          foreach (WeaponInfo w in child.Weapons.Values)
          {
            if (w.WeaponCooldown < Game.Instance().GameTime + empduration + 2)
            {
              w.WeaponCooldown = Game.Instance().GameTime + empduration + 2;
            }
          }

          foreach (ActorInfo child2 in child.GetAllChildren(1))
          {
            if (child2.TypeInfo is ElectroATI)
            {
              child2.SetStateF("CyclesRemaining", empduration / child2.TypeInfo.TimedLife);
              return;
            }
          }
          ActorCreationInfo acinfo = new ActorCreationInfo(ElectroATI.Instance());
          acinfo.Position = child.GetPosition();
          ActorInfo electro = ActorInfo.Create(acinfo);
          electro.AddParent(child);
          electro.SetStateF("CyclesRemaining", empduration / electro.TypeInfo.TimedLife);
        }
      }

      if (hitby.TypeInfo.IsShip)
      {
        ActionManager.ForceClearQueue(hitby);
        ActionManager.QueueNext(hitby, new Actions.Rotate(hitby.GetRelativePositionFUR(1000, -800, -200), hitby.MaxSpeed, 0.1f, false));
        ActionManager.QueueNext(hitby, new Actions.Lock());
      }

      if (hitby.GetStateF("TIEspawnRemaining") > 0)
        hitby.SetStateF("TIEspawnRemaining", 0);
    }
  }
}

