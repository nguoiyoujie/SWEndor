using MTV3D65;
using SWEndor.AI.Actions;
using System.IO;
using SWEndor.Weapons;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.AI;
using SWEndor.ActorTypes.Components;
using SWEndor.Actors.Models;

namespace SWEndor.ActorTypes.Instances
{
  public class BigIonLaserATI : Groups.LaserProjectile
  {
    internal BigIonLaserATI(Factory owner) : base(owner, "Large Ion Laser")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 30);
      Explodes = new ExplodeInfo[] {
        new ExplodeInfo("ExpL00", 1, 10, ExplodeTrigger.ON_DEATH | ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED)
      };

      ImpactDamage = 50;
      MoveLimitData.MaxSpeed = Globals.LaserSpeed * 2f;
      MoveLimitData.MinSpeed = Globals.LaserSpeed * 2f;

      IsLaser = false; // not the same speed
      Scale = 4;

      // Projectile
      ImpactCloseEnoughDistance = 150;

      MeshData = new MeshData(Name, @"projectiles\ion_sm_laser.x");
    }

    public override void ProcessHit(ActorInfo owner, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      if (owner == null || hitby == null)
        return;

      base.ProcessHit(owner, hitby, impact, normal);
      foreach (ActorInfo child in hitby.Children)
      {
        child.InflictDamage(hitby, 0.5f * child.HP, DamageType.NORMAL, child.GetGlobalPosition());
        //CombatSystem.onNotify(Engine, child, CombatEventType.DAMAGE_FRAC, 0.5f);

        float empduration = 10000;

        for (int i = 0; i < child.WeaponDefinitions.Weapons.Length; i++)
          if (child.WeaponDefinitions.Weapons[i].WeaponCooldown < Game.GameTime + empduration + 2)
            child.WeaponDefinitions.Weapons[i].WeaponCooldown = Game.GameTime + empduration + 2;

        foreach (ActorInfo child2 in child.Children)
        {
          if (child2.TypeInfo is ElectroATI)
          {
            child2.CycleInfo.CyclesRemaining = empduration / child2.TypeInfo.TimedLifeData.TimedLife;
            return;
          }
        }
        ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Get("Electro"));
        //acinfo.Position = child.GetGlobalPosition();
        ActorInfo electro = ActorFactory.Create(acinfo);
        child.AddChild(electro);
        electro.UseParentCoords = true;
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

