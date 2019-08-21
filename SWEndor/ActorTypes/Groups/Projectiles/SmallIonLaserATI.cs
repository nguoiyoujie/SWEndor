using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class SmallIonLaserATI : Groups.LaserProjectile
  {
    internal SmallIonLaserATI(Factory owner) : base(owner, "Ion Laser")
    {
      TimedLifeData = new TimedLifeData(true, 5);

      ImpactDamage = 5;
      MaxSpeed = Globals.LaserSpeed * 0.6f;
      MinSpeed = Globals.LaserSpeed * 0.6f;

      ImpactCloseEnoughDistance = 75;
      IsLaser = false; // not the same speed

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\ion_sm_laser.x");
    }

    public override void ProcessHit(ActorInfo owner, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      if (owner == null || hitby == null)
        return;

      base.ProcessHit(owner, hitby, impact, normal);
      
      List<ActorInfo> children = new List<ActorInfo>(hitby.Children);
      List<ActorInfo> rm = new List<ActorInfo>();
      foreach (ActorInfo c in children)
      {
        if (c == null
          || !c.Active
          || !c.TypeInfo.TargetType.HasFlag(TargetType.ADDON))
          rm.Add(c);
      }

      foreach (ActorInfo r in rm)
        children.Remove(r);
        
      if (children.Count > 0)
      {
        for (int shock = 3; shock > 0; shock--)
        {
          ActorInfo child = children[Engine.Random.Next(0, children.Count)];
          child.InflictDamage(hitby, 0.1f * Engine.Random.Next(25, 50), DamageType.NORMAL, child.GetGlobalPosition());

          float empduration = 12;
          
          foreach (WeaponInfo w in child.WeaponSystemInfo.Weapons.Values)
            if (w.WeaponCooldown < Game.GameTime + empduration + 2)
              w.WeaponCooldown = Game.GameTime + empduration + 2;

          foreach (ActorInfo child2 in child.Children)
          {
            if (child2.TypeInfo is ElectroATI)
            {
              child2.CycleInfo.CyclesRemaining = empduration / child2.CycleInfo.CyclePeriod;
              return;
            }
          }
          ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Get("Electro"));
          acinfo.Position = child.GetGlobalPosition();
          ActorInfo electro = ActorFactory.Create(acinfo);
          child.AddChild(electro);
          electro.CycleInfo.CyclesRemaining = empduration / electro.CycleInfo.CyclePeriod;
        }
      }
    }
  }
}

