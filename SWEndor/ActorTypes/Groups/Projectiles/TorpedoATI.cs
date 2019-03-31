﻿using MTV3D65;
using SWEndor.AI.Actions;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class TorpedoATI : ProjectileGroup
  {
    private static TorpedoATI _instance;
    public static TorpedoATI Instance()
    {
      if (_instance == null) { _instance = new TorpedoATI(); }
      return _instance;
    }

    private TorpedoATI() : base("Torpedo")
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

      EnableDistanceCull = false;

      // Projectile
      ImpactCloseEnoughDistance = 100;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\torpedo.x");
      m_particletex = Engine.Instance().TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, @"particle.dds"));
    }

    private int m_particletex;

    public override void ProcessNewState(ActorInfo ainfo)
    {
      if (ainfo.ActorState == ActorState.DYING || ainfo.ActorState == ActorState.DEAD)
      {
        ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("ExplosionSm"));
        acinfo.Position = ainfo.GetPosition();
        ActorInfo.Create(acinfo);

        ainfo.ActorState = ActorState.DEAD;
      }
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"dummy", new TrackerDummyWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new string[] { "1:dummy" };
      ainfo.SecondaryWeapons = new string[] { "none" };
      ainfo.AIWeapons = new string[] { "1:dummy" };
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      if (ainfo.CurrentAction == null || ainfo.CurrentAction is Idle)
        ainfo.ActorState = ActorState.DEAD;


      if (ainfo.ActorState == ActorState.NORMAL)
      {
        if (ainfo.ParticleSystem == null)
        {
          ainfo.ParticleSystem = Engine.Instance().TVScene.CreateParticleSystem();
          int emitter = ainfo.ParticleSystem.CreateEmitter(CONST_TV_EMITTERTYPE.TV_EMITTER_BILLBOARD, 250);
          ainfo.SetStateF("ParticleEmitterID", emitter);

          ainfo.ParticleSystem.Enable(true);
          ainfo.ParticleSystem.SetBillboard(emitter, m_particletex);
          ainfo.ParticleSystem.SetEmitterSphereRadius(emitter, 8);
          ainfo.ParticleSystem.SetEmitterShape(emitter, CONST_TV_EMITTERSHAPE.TV_EMITTERSHAPE_SPHEREVOLUME);
          ainfo.ParticleSystem.SetEmitterPower(emitter, 50, 0.2f);
          ainfo.ParticleSystem.SetEmitterDirection(emitter, false, new TV_3DVECTOR(0, 1, 0), new TV_3DVECTOR(0.5f, 0, 0.5f));
          ainfo.ParticleSystem.SetParticleDefaultColor(emitter, new TV_COLOR(1f, 0.75f, 0, 1));
          ainfo.ParticleSystem.SetEmitterSpeed(emitter, 5);
        }

        if (ainfo.ParticleSystem != null)
        {
          if (ainfo.IsStateFDefined("ParticleEmitterID"))
          {
            ainfo.ParticleSystem.SetEmitterPosition((int)ainfo.GetStateF("ParticleEmitterID"), ainfo.GetPosition());
          }
        }
      }

    }
  }
}

