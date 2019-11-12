﻿using MTV3D65;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  internal class LandoFalconATI : Groups.RebelWing
  {
    internal LandoFalconATI(Factory owner) : base(owner, "LANDO", "Millennium Falcon (Lando)")
    {
      // Combat
      CombatData = CombatData.DefaultShip;
      ArmorData = ArmorData.Default;

      SystemData.MaxShield = 50;
      SystemData.MaxHull = 150;
      CombatData.ImpactDamage = 10;
      MoveLimitData.MaxSpeed = 500;
      MoveLimitData.MinSpeed = 250;
      MoveLimitData.MaxSpeedChangeRate = 250;
      MoveLimitData.MaxTurnRate = 55;

      AIData.AggressiveTracker = true;
      ScoreData = new ScoreData(750, 10000);

      RenderData.AlwaysShowInRadar = true;

      RegenData = new RegenData(false, 0.2f, 0, 0, 0);

      MeshData = new MeshData(Name, @"falcon\falcon.x");

      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineFalcon, 200f, new TV_3DVECTOR(0, 0, -30), true, isEngineSound: true) };
      AddOns = new AddOnData[] { new AddOnData("INVLSR", new TV_3DVECTOR(0, 7, 20), new TV_3DVECTOR(90, 0, 0), true) };

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 0, 50), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -50), new TV_3DVECTOR(0, 0, -2000))
      };

      Loadouts = new string[] { "FALC_LASR" };
    }
  }
}
