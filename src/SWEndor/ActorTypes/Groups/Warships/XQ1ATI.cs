using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class XQ1ATI : Groups.Warship
  {
    internal XQ1ATI(Factory owner) : base(owner, "XQ1", "XQ1-Platform")
    {
      SystemData.MaxShield = 300;
      SystemData.MaxHull = 450;
      CombatData.ImpactDamage = 60.0f;
      MoveLimitData.MaxSpeed = 0;
      MoveLimitData.MinSpeed = 0;

      RenderData.CullDistance = 80000;
      ScoreData = new ScoreData(10, 10000);

      CameraData.Cameras = new LookData[] { new LookData(new TV_3DVECTOR(0, 45, 660), new TV_3DVECTOR(0, 45, 2000)) };
      CameraData.DeathCamera = new DeathCameraData(1500, 250, 30);

      MeshData = new MeshData(Engine, Name, @"platform\xq1.x", 3f);
      RegenData = new RegenData(false, 2.5f, 0, 0.25f, 0);

      AddOnData.AddOns = new AddOnData[]
      {
        // roof
        new AddOnData("XQLSR", new TV_3DVECTOR(0, 35, 115), new TV_3DVECTOR(-90, 0, 0), true),
        new AddOnData("XQLSR", new TV_3DVECTOR(89, 35, -38), new TV_3DVECTOR(-90, 0, 120), true),
        new AddOnData("XQLSR", new TV_3DVECTOR(-89, 35, -38), new TV_3DVECTOR(-90, 0, -120), true),

        // platform
        new AddOnData("XQLSR", new TV_3DVECTOR(245, 2, -289), new TV_3DVECTOR(-90, 0, 120), true),
        new AddOnData("XQLSR", new TV_3DVECTOR(-245, 2, -289), new TV_3DVECTOR(-90, 0, 120), true),
        new AddOnData("XQLSR", new TV_3DVECTOR(384, 2, -48), new TV_3DVECTOR(-90, 0, -120), true),
        new AddOnData("XQLSR", new TV_3DVECTOR(-384, 2, -48), new TV_3DVECTOR(-90, 0, -120), true),
        new AddOnData("XQLSR", new TV_3DVECTOR(138, 2, 375), new TV_3DVECTOR(-90, 0, 0), true),
        new AddOnData("XQLSR", new TV_3DVECTOR(-138, 2, 375), new TV_3DVECTOR(-90, 0, 0), true),

        new AddOnData("HANGAR", new TV_3DVECTOR(0, -12, 40), new TV_3DVECTOR(0, 0, 0), true)
      };

      SpawnerData.SpawnTypes = new string[] { "TIE" };
      SpawnerData.SpawnMoveDelay = 4;
      SpawnerData.SpawnInterval = 5;
      SpawnerData.SpawnsRemaining = 60;
      SpawnerData.SpawnLocations = new TV_3DVECTOR[]{ new TV_3DVECTOR(40, 0, -60)
                                               , new TV_3DVECTOR(40, 0, 20)
                                               , new TV_3DVECTOR(-40, 0, -60)
                                               , new TV_3DVECTOR(-40, 0, 20)
                                               };

      SpawnerData.SpawnSpeed = -1;
      SpawnerData.SpawnManualPositioningMult = new TV_3DVECTOR(10, 6, 0);
    }
  }
}

