using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  public class CargoMdATI : Groups.RebelWing
  {
    internal CargoMdATI(Factory owner) : base(owner, "CARGO_MD", "Cargo Freighter")
    {
      CombatData.MaxStrength = 18;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 250;
      MoveLimitData.MinSpeed = 25;
      MoveLimitData.MaxSpeedChangeRate = 50;
      MoveLimitData.MaxTurnRate = 20;

      ScoreData = new ScoreData(100, 500);

      RegenData = new RegenData(false, 0.04f, 0, 0, 0);
      DyingMoveData.Spin(90, 120);

      MeshData = new MeshData(Name, @"cargo\cargo2.x");

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 5, 48), new TV_3DVECTOR(0, 2, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
     };

      //SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineAWing, 200f, new TV_3DVECTOR(0, 0, -30), true, isEngineSound: true) };

      //Loadouts = new string[] { "A_WG_TORP", "A_WG_MISL", "A_WG_LASR" };
    }
  }
}

