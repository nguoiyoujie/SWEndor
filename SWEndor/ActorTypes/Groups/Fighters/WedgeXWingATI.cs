using MTV3D65;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class WedgeXWingATI : Groups.RebelWing
  {
    internal WedgeXWingATI(Factory owner) : base(owner, "X-Wing (Wedge)")
    {
      // Combat
      CombatData = CombatData.DefaultShip;
      Armor = ArmorInfo.Default;

      MaxStrength = 200;
      ImpactDamage = 16;
      MaxSpeed = 500;
      MinSpeed = 250;
      MaxSpeedChangeRate = 500;
      MaxTurnRate = 50;

      AggressiveTracker = true;
      Score_perStrength = 750;
      Score_DestroyBonus = 10000;

      AlwaysShowInRadar = true;

      RegenData = new RegenInfo { SelfRegenRate = 0.1f };

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"xwing\xwing.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"xwing\xwing_far.x");

      Cameras = new ActorCameraInfo[] {
        new ActorCameraInfo(new TV_3DVECTOR(0, 3, 25), new TV_3DVECTOR(0, 3, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new ActorCameraInfo(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
        };

      Loadouts = new string[] { "X_WG_TORP", "X_WG_LASR" };
    }
  }
}

