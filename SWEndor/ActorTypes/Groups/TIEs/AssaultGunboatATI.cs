using MTV3D65;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class AssaultGunboatATI : Groups.TIE
  {
    internal AssaultGunboatATI(Factory owner) : base(owner, "Assault Gunboat")
    {
      MaxStrength = 24;
      ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 550;
      MoveLimitData.MinSpeed = 200;
      MoveLimitData.MaxSpeedChangeRate = 300;
      MoveLimitData.MaxTurnRate = 56;

      MoveLimitData.ZTilt = 0.75f;
      MoveLimitData.ZNormFrac = 0.004f;

      ScoreData = new ScoreData(800, 2000);

      RegenData = new RegenData(false, 0.05f, 0, 0, 0);

      MeshData = new MeshData(Name, @"gunboat\gunboat.x");

      Cameras = new LookData[]
      {
        new LookData(new TV_3DVECTOR(0, 7, 18), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -150), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };

      Loadouts = new string[] { "GUN__TORP", "GUN__MISL", "GUN__LASR" };
    }
  }
}

