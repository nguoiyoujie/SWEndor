using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class CargoSmATI : Groups.RebelWing
  {
    internal CargoSmATI(Factory owner) : base(owner, "CARGO_SM", "Cargo Ship")
    {
      SystemData.MaxShield = 5;
      SystemData.MaxHull = 7;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 250;
      MoveLimitData.MinSpeed = 25;
      MoveLimitData.MaxSpeedChangeRate = 50;
      MoveLimitData.MaxTurnRate = 20;

      ScoreData = new ScoreData(100, 500);

      RegenData = new RegenData(false, 0.1f, 0, 0, 0);
      DyingMoveData.Spin(90, 180);

      MeshData = new MeshData(Name, @"cargo\cargo1.x");

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 4, 35), new TV_3DVECTOR(0, 2, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
     };

      //SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineAWing, 200f, new TV_3DVECTOR(0, 0, -30), true, isEngineSound: true) };

      //Loadouts = new string[] { "A_WG_TORP", "A_WG_MISL", "A_WG_LASR" };
    }
  }
}

