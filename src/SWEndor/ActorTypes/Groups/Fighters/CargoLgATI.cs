using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class CargoLgATI : Groups.RebelWing
  {
    internal CargoLgATI(Factory owner) : base(owner, "CARGO_LG", "Cargo Freighter")
    {
      SystemData.MaxShield = 15;
      SystemData.MaxHull = 10;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 250;
      MoveLimitData.MinSpeed = 25;
      MoveLimitData.MaxSpeedChangeRate = 50;
      MoveLimitData.MaxTurnRate = 20;

      ScoreData = new ScoreData(200, 500);

      RegenData = new RegenData(false, 0.08f, 0, 0, 0);
      DyingMoveData.Spin(Engine.Random, 30, 90);

      MeshData = new MeshData(Engine, Name, @"cargo\cargo3.x");

      CameraData.Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 5, 48), new TV_3DVECTOR(0, 2, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
     };

      //SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineAWing, 200f, new TV_3DVECTOR(0, 0, -30), true, isEngineSound: true) };
      AddOnData.AddOns = new AddOnData[] { new AddOnData("CRGLSR", new TV_3DVECTOR(0, 9, -68), new TV_3DVECTOR(-90, 0, 0), true) };

      //Loadouts = new string[] { "A_WG_TORP", "A_WG_MISL", "A_WG_LASR" };
    }
  }
}

