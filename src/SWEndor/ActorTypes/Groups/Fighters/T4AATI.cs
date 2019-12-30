using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class T4AATI : Groups.RebelWing
  {
    internal T4AATI(Factory owner) : base(owner, "T4A", "T-4A Lambda-class Shuttle")
    {
      SystemData.MaxShield = 6;
      SystemData.MaxHull = 6;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 300;
      MoveLimitData.MinSpeed = 50;
      MoveLimitData.MaxSpeedChangeRate = 100;
      MoveLimitData.MaxTurnRate = 36;

      ScoreData = new ScoreData(200, 500);
      DyingMoveData.Spin(Engine.Random, 90, 180);

      RegenData = new RegenData(false, 0.18f, 0, 0.1f, 0);

      MeshData = new MeshData(Engine, Name, @"shuttle\t4a.x");

      CameraData.Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 2, 18), new TV_3DVECTOR(0, 2, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
     };

      //SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineAWing, 200f, new TV_3DVECTOR(0, 0, -30), true, isEngineSound: true) };
      AddOnData.AddOns = new AddOnData[] { new AddOnData("T4ALSR", new TV_3DVECTOR(0, -2, -12), new TV_3DVECTOR(90, 0, 0), true) };

      //Loadouts = new string[] { "A_WG_TORP", "A_WG_MISL", "A_WG_LASR" };
    }
  }
}

