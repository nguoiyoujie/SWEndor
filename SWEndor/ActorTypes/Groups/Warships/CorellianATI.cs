﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class CorellianATI : Groups.Warship
  {
    internal CorellianATI(Factory owner) : base(owner, "Corellian Corvette")
    {
      MaxStrength = 575.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 100.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 50.0f;
      MaxTurnRate = 9f;

      ZTilt = 5.5f;
      ZNormFrac = 0.015f;

      CullDistance = 20000;

      Score_perStrength = 10;
      Score_DestroyBonus = 5000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"corellian\corellian.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"corellian\corellian_far.x");

      Cameras = new ActorCameraInfo[] {
        new ActorCameraInfo(new TV_3DVECTOR(0, 55, -35), new TV_3DVECTOR(0, 55, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 300, -800), new TV_3DVECTOR(0, 0, 2000))
        };
      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("engine_big", 500f, new TV_3DVECTOR(0, 0, -200), true) };
      AddOns = new AddOnInfo[]
      {
        new AddOnInfo("Corellian Turbolaser Tower", new TV_3DVECTOR(-35, 15, 110), new TV_3DVECTOR(-90, 0, 10), true)
        , new AddOnInfo("Corellian Turbolaser Tower", new TV_3DVECTOR(-35, 15, 80), new TV_3DVECTOR(-90, 0, 10), true)
        , new AddOnInfo("Corellian Turbolaser Tower", new TV_3DVECTOR(35, 15, 110), new TV_3DVECTOR(-90, 0, -10), true)
        , new AddOnInfo("Corellian Turbolaser Tower", new TV_3DVECTOR(35, 15, 80), new TV_3DVECTOR(-90, 0, -10), true)
        , new AddOnInfo("Corellian Turbolaser Tower", new TV_3DVECTOR(0, -45, 150), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnInfo("Corellian Turbolaser Tower", new TV_3DVECTOR(0, 45, 150), new TV_3DVECTOR(-90, 0, 0), true)
      };
    }
  }
}

