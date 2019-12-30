using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class AssaultGunboatATI : Groups.TIE
  {
    internal AssaultGunboatATI(Factory owner) : base(owner, "GUN", "Assault Gunboat")
    {
      SystemData.MaxShield = 16;
      SystemData.MaxHull = 8;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 550;
      MoveLimitData.MinSpeed = 200;
      MoveLimitData.MaxSpeedChangeRate = 300;
      MoveLimitData.MaxTurnRate = 56;

      MoveLimitData.ZTilt = 0.75f;
      MoveLimitData.ZNormFrac = 0.004f;

      ScoreData = new ScoreData(800, 2000);

      RegenData = new RegenData(false, 0.25f, 0, 0, 0);

      MeshData = new MeshData(Engine, Name, @"gunboat\gunboat.x");

      CameraData.Cameras = new LookData[]
      {
        new LookData(new TV_3DVECTOR(0, 7, 18), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -150), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };

      WeapSystemData.Loadouts = new WeapData[]
      {
        new WeapData("TORP", "SEC_1_AI", "NO_AUTOAIM", "GUN__TORP", "GUN__TORP", "WING_TORP", "WING_TORP"),
        new WeapData("MISL", "SEC_1_AI", "NO_AUTOAIM", "GUN__MISL", "GUN__MISL", "WING_MISL", "WING_MISL"),
        new WeapData("LASR", "PRI_12_AI", "NO_AUTOAIM", "DEFAULT", "GUN__LASR", "WING_LSR_G", "WING_LASER"),
      };
    }
  }
}

