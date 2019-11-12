﻿using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  internal class NebulonB2ATI : Groups.Warship
  {
    internal NebulonB2ATI(Factory owner) : base(owner, "NEB2", "Nebulon-B2 Frigate")
    {
      SystemData.MaxShield = 600;
      SystemData.MaxHull = 750;
      CombatData.ImpactDamage = 60.0f;
      MoveLimitData.MaxSpeed = 36.0f;
      MoveLimitData.MinSpeed = 0.0f;
      MoveLimitData.MaxSpeedChangeRate = 10.0f;
      MoveLimitData.MaxTurnRate = 2.5f;

      RenderData.CullDistance = 30000;
      ScoreData = new ScoreData(15, 10000);
      RegenData = new RegenData(false, 0.5f, 0, 0, 0);

      MeshData = new MeshData(Name, @"nebulonb\nebulonb2.x", 0.75f);
      DyingMoveData.Sink(0.02f, 5f, 0.8f);

      Cameras = new LookData[] { new LookData(new TV_3DVECTOR(0, 120, -300), new TV_3DVECTOR(0, 120, 2000)) };
      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineShip, 1500.0f, new TV_3DVECTOR(0, 100, -300), true, isEngineSound: true) };
      AddOns = new AddOnData[]
      {
        new AddOnData("NEBLLSR", new TV_3DVECTOR(0, 40, 220), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("NEBLLSR", new TV_3DVECTOR(0, 95, -520), new TV_3DVECTOR(-90, 0, 0), true)
        , new AddOnData("NEBLLSR", new TV_3DVECTOR(0, -145, -550), new TV_3DVECTOR(90, 0, 0), true)
        , new AddOnData("NEBLMPOD", new TV_3DVECTOR(-80, -45, -485), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnData("NEBLMPOD", new TV_3DVECTOR(80, -45, -485), new TV_3DVECTOR(0, 0, 0), true)
      };
    }
  }
}
