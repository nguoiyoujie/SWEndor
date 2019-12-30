using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class TIE_X1_ATI : Groups.TIE
  {
    internal TIE_X1_ATI(Factory owner) : base(owner, "TIEX", "TIE Advanced X1")
    {
      CombatData = CombatData.DefaultShip;
      ArmorData = ArmorData.Default;

      SystemData.MaxShield = 250;
      SystemData.MaxHull = 10;
      CombatData.ImpactDamage = 100;
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

      RegenData = new RegenData(false, 0.18f, 0, 0, 0);

      MeshData = new MeshData(Engine, Name, @"tie_vader\tie_vader.x");
      CameraData.Cameras = new LookData[]
      {
        new LookData(new TV_3DVECTOR(0, 0, 12), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };

      WeapSystemData.Loadouts = new WeapData[]
      {
        new WeapData("TORP", "SEC_1_AI", "NO_AUTOAIM", "TIEX_TORP", "TIEX_TORP", "WING_TORP", "WING_TORP"),
        new WeapData("LASR", "PRI_124_AI", "AUTOAIM", "DEFAULT", "TIEX_LASR", "WING_LSR_GADV", "WING_LASER"),
      };
    }
  }
}

