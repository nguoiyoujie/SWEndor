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
      Armor = ArmorInfo.Default;

      MaxStrength = 50;
      ImpactDamage = 10;
      MaxSpeed = 500;
      MinSpeed = 250;
      MaxSpeedChangeRate = 250;
      MaxTurnRate = 55;

      AggressiveTracker = true;
      Score_perStrength = 750;
      Score_DestroyBonus = 10000;

      RegenData = new RegenInfo { SelfRegenRate = 0.1f };

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"falcon\falcon.x");

      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("falcon_engine", 200f, new TV_3DVECTOR(0, 0, -30), true) };
      AddOns = new AddOnInfo[] { new AddOnInfo("Invisible Rebel Turbo Laser", new TV_3DVECTOR(0, 7, 20), new TV_3DVECTOR(90, 0, 0), true) };

      Cameras = new ActorCameraInfo[] {
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, 50), new TV_3DVECTOR(0, 0, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, -50), new TV_3DVECTOR(0, 0, -2000))
     };

      Loadouts = new string[] { "FALC_LASR" };
    }
  }
}

