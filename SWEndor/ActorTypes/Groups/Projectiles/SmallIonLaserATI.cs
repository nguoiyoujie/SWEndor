using MTV3D65;
using SWEndor.Actors;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class SmallIonLaserATI : Group.Projectile
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

    public override void ProcessHit(int ownerActorID, int hitbyActorID, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      ActorInfo owner = ActorInfo.Factory.Get(ownerActorID);
      ActorInfo hitby = ActorInfo.Factory.Get(hitbyActorID);

      if (owner == null || hitby == null)
        return;

      base.ProcessHit(ownerActorID, hitbyActorID, impact, normal);
      List<int> children = hitby.GetAllChildren(1);
      List<int> rm = new List<int>();
      foreach (int i in children)
      {
        ActorInfo c = ActorInfo.Factory.Get(i);
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
          ActorInfo child = ActorInfo.Factory.Get(children[Engine.Instance().Random.Next(0, children.Count)]);
          child.CombatInfo.Strength -= 0.1f * Engine.Instance().Random.Next(25, 50);

          float empduration = 12;
          
          foreach (WeaponInfo w in child.WeaponSystemInfo.Weapons.Values)
            if (w.WeaponCooldown < Game.Instance().GameTime + empduration + 2)
              w.WeaponCooldown = Game.Instance().GameTime + empduration + 2;

          foreach (int i in child.GetAllChildren(1))
          {
            ActorInfo child2 = ActorInfo.Factory.Get(i);
            if (child2.TypeInfo is ElectroATI)
            {
              child2.CycleInfo.CyclesRemaining = empduration / child2.CycleInfo.CyclePeriod;
              return;
            }
          }
          ActorCreationInfo acinfo = new ActorCreationInfo(ElectroATI.Instance());
          acinfo.Position = child.GetPosition();
          ActorInfo electro = ActorInfo.Create(acinfo);
          electro.AddParent(child.ID);
          electro.CycleInfo.CyclesRemaining = empduration / electro.CycleInfo.CyclePeriod;
        }
      }
    }
  }
}

