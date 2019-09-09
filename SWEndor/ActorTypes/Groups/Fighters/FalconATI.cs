using MTV3D65;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class FalconATI : Groups.RebelWing
  {
    internal FalconATI(Factory owner) : base(owner, "Millennium Falcon")
    {
      // Combat
      CombatData = CombatData.DefaultShip;
      ArmorData = ArmorData.Default;

      MaxStrength = 50;
      ImpactDamage = 10;
      MoveLimitData.MaxSpeed = 500;
      MoveLimitData.MinSpeed = 250;
      MoveLimitData.MaxSpeedChangeRate = 250;
      MoveLimitData.MaxTurnRate = 55;

      AIData.AggressiveTracker = true;
      ScoreData = new ScoreData(750, 10000);


      RegenData = new RegenData(false, 0.1f, 0, 0, 0);

      MeshData = new MeshData(Name, @"falcon\falcon.x");

      SoundSources = new SoundSourceData[] { new SoundSourceData("falcon_engine", 200f, new TV_3DVECTOR(0, 0, -30), true) };
      AddOns = new AddOnData[] { new AddOnData("Invisible Rebel Turbo Laser", new TV_3DVECTOR(0, 7, 20), new TV_3DVECTOR(90, 0, 0), true) };

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 0, 50), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -50), new TV_3DVECTOR(0, 0, -2000))
     };

      Loadouts = new string[] { "FALC_LASR" };
    }
  }
}

