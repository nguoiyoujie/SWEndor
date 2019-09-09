using MTV3D65;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class MissileBoatATI : Groups.TIE
  {
    internal MissileBoatATI(Factory owner) : base(owner, "Missile Boat")
    {
      MaxStrength = 12;
      ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 475;
      MoveLimitData.MinSpeed = 175;
      MoveLimitData.MaxSpeedChangeRate = 250;
      MoveLimitData.MaxTurnRate = 48;

      MoveLimitData.ZTilt = 0.75f;
      MoveLimitData.ZNormFrac = 0.006f;

      ScoreData = new ScoreData(800, 2000);

      RegenData = new RegenData(false, 0.05f, 0, 0, 0);

      MeshData = new MeshData(Name, @"tie_vader\tie_vader.x");

      Cameras = new LookData[]
      {
        new LookData(new TV_3DVECTOR(0, 0, 20), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };

      Loadouts = new string[] { "MIS__TORP", "MIS__MISL", "MIS__LASR" };
    }
  }
}

