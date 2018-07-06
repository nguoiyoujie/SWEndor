using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class MissileATI : ProjectileGroup
  {
    private static MissileATI _instance;
    public static MissileATI Instance()
    {
      if (_instance == null) { _instance = new MissileATI(); }
      return _instance;
    }

    private MissileATI() : base("Missile")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 10;
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = true;
      ImpactDamage = 12;
      MaxSpeed = 1500;
      MinSpeed = 1500;
      AlwaysAccurateRotation = true;

      // Projectile
      ImpactCloseEnoughDistance = 60;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\torpedo.x");
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

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"dummy", new TrackerDummyWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new List<string> { "1:dummy" };
      ainfo.SecondaryWeapons = new List<string> { "none" };
      ainfo.AIWeapons = new List<string> { "1:dummy" };
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      if (ainfo.CurrentAction == null || ainfo.CurrentAction is Actions.Idle)
        ainfo.ActorState = ActorState.DEAD;

      /*
      if (ainfo.ActorState == ActorState.NORMAL)
      {
        TVParticleSystem psys;
        if (ainfo.ParticleSystems.Count == 0)
        {
          psys = Engine.Instance().TVScene.CreateParticleSystem(ainfo.Key);
          ainfo.SetStateF("ParticleEmitterID", psys.CreateEmitter(CONST_TV_EMITTERTYPE.TV_EMITTER_BILLBOARD, 250));

          psys.Enable(true);
          psys.SetEmitterSphereRadius((int)ainfo.GetStateF("ParticleEmitterID"), 8);
          psys.SetEmitterShape((int)ainfo.GetStateF("ParticleEmitterID"), CONST_TV_EMITTERSHAPE.TV_EMITTERSHAPE_SPHEREVOLUME);
          psys.SetEmitterPower((int)ainfo.GetStateF("ParticleEmitterID"), 50, 0.2f);
          psys.SetEmitterDirection((int)ainfo.GetStateF("ParticleEmitterID"), false, new TV_3DVECTOR(0, 1, 0), new TV_3DVECTOR(0.5f, 0, 0.5f));
          psys.SetParticleDefaultColor((int)ainfo.GetStateF("ParticleEmitterID"), new TV_COLOR(1f, 0.75f, 0, 1));
          psys.SetEmitterSpeed((int)ainfo.GetStateF("ParticleEmitterID"), 5);

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
  }
}

