﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.AI.Actions;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TorpedoATI : Group.Projectile
  {
    internal TorpedoATI(Factory owner) : base(owner, "Torpedo")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 25;
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = true;
      ImpactDamage = 12;
      MaxSpeed = 750;
      MinSpeed = 750;
      MaxTurnRate = 75;

      //AlwaysAccurateRotation = true;

      CullDistance = 12000;
      //EnableDistanceCull = false;

      // Projectile
      ImpactCloseEnoughDistance = 100;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\torpedo.x");
      m_particletex = TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, @"particle.dds"));
    }

    private int m_particletex;

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"dummy", new TrackerDummyWeapon() }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:dummy" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:dummy" };

      ainfo.ExplosionInfo.DeathExplosionType = "ExplosionSm";
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      if (ainfo.CurrentAction == null || ainfo.CurrentAction is Idle)
        ainfo.ActorState = ActorState.DEAD;


      if (ainfo.ActorState == ActorState.NORMAL && !ActorInfo.IsFarMode(ainfo.GetEngine(), ainfo.ID))
      {
        if (ainfo.ParticleSystem == null)
        {
          ainfo.ParticleSystem = TrueVision.TVScene.CreateParticleSystem();
          int emitter = ainfo.ParticleSystem.CreateEmitter(CONST_TV_EMITTERTYPE.TV_EMITTER_BILLBOARD, 250);
          ainfo.ParticleEmitterID = emitter;

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
          if (ainfo.ParticleEmitterID >= 0)
            ainfo.ParticleSystem.SetEmitterPosition(ainfo.ParticleEmitterID, ainfo.GetPosition());
      }
    }
  }
}

