using MTV3D65;
using SWEndor.Actors;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class SmallIonLaserATI : ProjectileGroup
  {
    private static SmallIonLaserATI _instance;
    public static SmallIonLaserATI Instance()
    {
      if (_instance == null) { _instance = new SmallIonLaserATI(); }
      return _instance;
    }

    private SmallIonLaserATI() : base("Ion Cannon Laser")
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
        ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeInfo.Factory.Get("ExplosionSm"));
        acinfo.Position = ainfo.GetPosition();
        ActorInfo.Create(acinfo);

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
          || !c.TypeInfo.TargetType.HasFlag(TargetType.ADDON))
          rm.Add(c);
      }

      foreach (ActorInfo r in rm)
      {
        children.Remove(r);
      }

      if (children.Count > 0)
      {
        for (int shock = 3; shock > 0; shock--)
        {
          ActorInfo child = children[Engine.Instance().Random.Next(0, children.Count)];
          child.CombatInfo.Strength -= 0.1f * Engine.Instance().Random.Next(25, 50);

          float empduration = 12;
          
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
              child2.CycleInfo.CyclesRemaining = empduration / child2.CycleInfo.CyclePeriod;
              return;
            }
          }
          ActorCreationInfo acinfo = new ActorCreationInfo(ElectroATI.Instance());
          acinfo.Position = child.GetPosition();
          ActorInfo electro = ActorInfo.Create(acinfo);
          electro.AddParent(child);
          electro.CycleInfo.CyclesRemaining = empduration / electro.CycleInfo.CyclePeriod;
        }
      }
    }
  }
}

