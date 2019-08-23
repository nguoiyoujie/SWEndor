using MTV3D65;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TransportATI : Groups.Warship
  {
    internal TransportATI(Factory owner) : base(owner, "Transport")
    {
      MaxStrength = 250.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 40.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 5.0f;
      MaxTurnRate = 3f;

      CullDistance = 20000;

      Score_perStrength = 8;
      Score_DestroyBonus = 3000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"transport\transport.x");

      Cameras = new ActorCameraInfo[] { new ActorCameraInfo(new TV_3DVECTOR(0, 86, -150), new TV_3DVECTOR(0, 86, 2000)) };
      Debris = new DebrisSpawnerInfo[] {
        new DebrisSpawnerInfo("Transport Box 1", new TV_3DVECTOR(60, -30, 100), -1000, 1000, -2000, 2000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("Transport Box 1", new TV_3DVECTOR(0, -30, 100), -1000, 1000, -2000, 2000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("Transport Box 1", new TV_3DVECTOR(-60, -30, 100), -1000, 1000, -2000, 2000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("Transport Box 2", new TV_3DVECTOR(60, -30, 100), -1000, 1000, -2000, 2000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("Transport Box 2", new TV_3DVECTOR(0, -30, 100), -1000, 1000, -2000, 2000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("Transport Box 2", new TV_3DVECTOR(-60, -30, 100), -1000, 1000, -2000, 2000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("Transport Box 3", new TV_3DVECTOR(60, -30, 100), -1000, 1000, -2000, 2000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("Transport Box 3", new TV_3DVECTOR(0, -30, 100), -1000, 1000, -2000, 2000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("Transport Box 3", new TV_3DVECTOR(-60, -30, 100), -1000, 1000, -2000, 2000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("Transport Box 4", new TV_3DVECTOR(60, -30, 100), -1000, 1000, -2000, 2000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("Transport Box 4", new TV_3DVECTOR(0, -30, 100), -1000, 1000, -2000, 2000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("Transport Box 4", new TV_3DVECTOR(-60, -30, 100), -1000, 1000, -2000, 2000, -2500, 2500, 0.5f),
      };

      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("engine_big", 500.0f, new TV_3DVECTOR(0, 0, -150), true) };
      AddOns = new AddOnInfo[]
      {
        new AddOnInfo("Transport Turbolaser Tower", new TV_3DVECTOR(0, 70, 200), new TV_3DVECTOR(-90, 0, 0), true)
      };
    }
  }
}

