﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class AcclamatorATI : Groups.StarDestroyer
  {
    internal AcclamatorATI(Factory owner) : base(owner, "Acclamator Assault Ship")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 650.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 110.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 8.0f;
      MaxTurnRate = 1.6f;

      CullDistance = 30000;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"acclamator\acclamator.x");

      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("engine_big", 500.0f, new TV_3DVECTOR(0, 0, -100), true) };
      AddOns = new AddOnInfo[]
      {
        // Sides
        new AddOnInfo("Acclamator Turbolaser Tower", new TV_3DVECTOR(101, 10, 460), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnInfo("Acclamator Turbolaser Tower", new TV_3DVECTOR(150, 10, 360), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnInfo("Acclamator Turbolaser Tower", new TV_3DVECTOR(198, 10, 260), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnInfo("Acclamator Turbolaser Tower", new TV_3DVECTOR(-101, 10, 460), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnInfo("Acclamator Turbolaser Tower", new TV_3DVECTOR(-150, 10, 360), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnInfo("Acclamator Turbolaser Tower", new TV_3DVECTOR(-198, 10, 260), new TV_3DVECTOR(0, -72, 0), true)
        
        // Front
        , new AddOnInfo("Acclamator Turbolaser Tower", new TV_3DVECTOR(0, 10, 610), new TV_3DVECTOR(-90, 0, 0), true)

        // Top
        , new AddOnInfo("Acclamator Turbolaser Tower", new TV_3DVECTOR(145, 42, 70), new TV_3DVECTOR(-90, 0, -15), true)
        , new AddOnInfo("Acclamator Turbolaser Tower", new TV_3DVECTOR(-145, 42, 70), new TV_3DVECTOR(-90, 0, 15), true)
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.CameraSystemInfo.CamLocations = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 143, 65) };
      ainfo.CameraSystemInfo.CamTargets = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 143, 2000) };

      ainfo.ExplosionInfo.DeathExplosionSize = 1.5f;
    }
  }
}

