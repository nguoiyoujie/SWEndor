using MTV3D65;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class TIE_X1_ATI : Groups.TIE
  {
    internal TIE_X1_ATI(Factory owner) : base(owner, "TIEX", "TIE Advanced X1")
    {
      CombatData = CombatData.DefaultShip;
      ArmorData = ArmorData.Default;

      SystemData.Parts = new SystemPart[] { SystemPart.ENGINE, SystemPart.ENERGY_CHARGER, SystemPart.ENERGY_STORE, SystemPart.LASER_WEAPONS, SystemPart.PROJECTILE_LAUNCHERS, SystemPart.RADAR, SystemPart.SCANNER, SystemPart.TARGETING_SYSTEM, SystemPart.COMLINK, SystemPart.HYPERDRIVE, SystemPart.SHIELD_GENERATOR };
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

      MeshData = new MeshData(Name, @"tie_vader\tie_vader.x");
      Cameras = new LookData[] 
      {
        new LookData(new TV_3DVECTOR(0, 0, 12), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };

      Loadouts = new string[] { "TIEX_TORP", "TIEX_LASR" };
    }
  }
}

