﻿using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  internal class YT1300ATI : Groups.RebelWing
  {
    internal YT1300ATI(Factory owner) : base(owner, "YT1300", "YT-1300")
    {
      // Combat
      CombatData = CombatData.DefaultShip;
      ArmorData = ArmorData.Default;

      SystemData.MaxShield = 40;
      SystemData.MaxHull = 10;
      CombatData.ImpactDamage = 10;
      MoveLimitData.MaxSpeed = 500;
      MoveLimitData.MinSpeed = 250;
      MoveLimitData.MaxSpeedChangeRate = 250;
      MoveLimitData.MaxTurnRate = 55;

      AIData.AggressiveTracker = true;
      ScoreData = new ScoreData(750, 10000);


      RegenData = new RegenData(false, 0.2f, 0, 0, 0);

      MeshData = new MeshData(Engine, Name, @"falcon\falcon.x");

      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineFalcon, 200f, new TV_3DVECTOR(0, 0, -30), true, isEngineSound: true) };
      AddOns = new AddOnData[] { new AddOnData("INVLSR", new TV_3DVECTOR(0, 7, 20), new TV_3DVECTOR(90, 0, 0), true) };

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 0, 50), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -50), new TV_3DVECTOR(0, 0, -2000))
     };

      Loadouts = new WeapData[] { new WeapData("LASR", "PRI_1_AI", "NO_AUTOAIM", "DEFAULT", "FALC_LASR", "WING_LSR_R", "WING_LASER") };
    }
  }
}
