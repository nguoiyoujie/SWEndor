using MTV3D65;
using SWEndor.Weapons;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Actors.Types
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
        ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("ExplosionSm"));
        acinfo.Position = ainfo.GetPosition();
        ActorInfo.Create(acinfo);

        /*
        TVParticleSystem psys = Engine.Instance().TVGlobals.GetParticleSystem(ainfo.Key);
        foreach (TVParticleSystem p in ainfo.ParticleSystems)
        {
          p.Destroy();
        }
        ainfo.ParticleSystems.Clear();
        */
        ainfo.ActorState = ActorState.DEAD;
      }
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      /*
      if (ainfo.ActorState == ActorState.NORMAL)
      {
        TVParticleSystem psys;
        if (ainfo.ParticleSystems.Count == 0)
        {
          psys = Engine.Instance().TVScene.CreateParticleSystem(ainfo.Key);
          ainfo.SetStateF("ParticleEmitterID", psys.CreateEmitter(CONST_TV_EMITTERTYPE.TV_EMITTER_BILLBOARD, 250));

          psys.Enable(true);
          psys.SetEmitterSphereRadius((int)ainfo.GetStateF("ParticleEmitterID"), 15);
          psys.SetEmitterShape((int)ainfo.GetStateF("ParticleEmitterID"), CONST_TV_EMITTERSHAPE.TV_EMITTERSHAPE_SPHEREVOLUME);
          psys.SetEmitterPower((int)ainfo.GetStateF("ParticleEmitterID"), 50, 0.2f);
          psys.SetEmitterDirection((int)ainfo.GetStateF("ParticleEmitterID"), false, new TV_3DVECTOR(0, 1, 0), new TV_3DVECTOR(0.25f, 0, 0.25f));
          psys.SetParticleDefaultColor((int)ainfo.GetStateF("ParticleEmitterID"), new TV_COLOR(0.6f, 0.6f, 1, 1));
          psys.SetEmitterSpeed((int)ainfo.GetStateF("ParticleEmitterID"), 3);

          ainfo.ParticleSystems.Add(psys);
        }
        else
        {
          psys = ainfo.ParticleSystems[0];
        }

        if (psys != null)
        {
          if (ainfo.IsStateFDefined("ParticleEmitterID"))
          {
            psys.SetEmitterPosition((int)ainfo.GetStateF("ParticleEmitterID"), ainfo.GetPosition());
          }
          psys.Update();
        }
      }
      */
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

