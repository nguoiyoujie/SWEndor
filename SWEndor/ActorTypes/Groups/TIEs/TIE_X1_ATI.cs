using MTV3D65;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_X1_ATI : Groups.TIE
  {
    internal TIE_X1_ATI(Factory owner) : base(owner, "TIE Advanced X1")
    {
      CombatData = CombatData.DefaultShip;
      ArmorData = ArmorData.Default;

      MaxStrength = 275;
      ImpactDamage = 100;
      MoveLimitData.MaxSpeed = 900;
      MoveLimitData.MinSpeed = 200;
      MoveLimitData.MaxSpeedChangeRate = 500;
      MoveLimitData.MaxTurnRate = 90;
      MoveLimitData.MaxSecondOrderTurnRateFrac = 0.8f;

      MoveLimitData.ZTilt = 3.25f;
      MoveLimitData.ZNormFrac = 0.005f;

      AIData.Attack_AngularDelta = 7.5f;
      AIData.Attack_HighAccuracyAngularDelta = 5;
      AIData.Move_CloseEnough = 1000;

      AIData.AggressiveTracker = true;

      ScoreData = new ScoreData(750, 10000);

      RegenData = new RegenData(false, 0.06f, 0, 0, 0);

      MeshData = new MeshData(Name, @"tie_vader\tie_vader.x");
      Cameras = new LookData[] 
      {
        new LookData(new TV_3DVECTOR(0, 0, 20), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };

      Loadouts = new string[] { "TIEX_TORP", "TIEX_LASR" };
    }
  }
}

