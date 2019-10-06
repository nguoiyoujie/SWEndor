using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  public class JV7ATI : Groups.RebelWing
  {
    internal JV7ATI(Factory owner) : base(owner, "JV7", "JV-7 Delta-class Shuttle")
    {
      CombatData.MaxStrength = 10;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 350;
      MoveLimitData.MinSpeed = 50;
      MoveLimitData.MaxSpeedChangeRate = 200;
      MoveLimitData.MaxTurnRate = 48;

      ScoreData = new ScoreData(300, 600);

      RegenData = new RegenData(false, 0.15f, 0, 0.1f, 0);
      DyingMoveData.Spin(90, 180);

      MeshData = new MeshData(Name, @"shuttle\jv7.x");

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 2, 18), new TV_3DVECTOR(0, 2, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
     };

      //SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineAWing, 200f, new TV_3DVECTOR(0, 0, -30), true, isEngineSound: true) };
      AddOns = new AddOnData[] { new AddOnData("JV7LSR", new TV_3DVECTOR(0, -2, -12), new TV_3DVECTOR(90, 0, 0), true) };

      Loadouts = new string[] { "JV7_MISL", "JV7_LASR" };
    }
  }
}

