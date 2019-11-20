using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class TIE_sa_ATI : Groups.TIE
  {
    internal TIE_sa_ATI(Factory owner) : base(owner, "TIESA", "TIE Bomber")
    {
      SystemData.MaxShield = 0;
      SystemData.MaxHull = 10;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 300;
      MoveLimitData.MinSpeed = 150;
      MoveLimitData.MaxSpeedChangeRate = 200;
      MoveLimitData.MaxTurnRate = 44;

      ScoreData = new ScoreData(350, 500);

      MeshData = new MeshData(Engine, Name, @"tie_vader\tie_bomber.x");

      Cameras = new LookData[]
      {
        new LookData(new TV_3DVECTOR(0, 0, 12), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };

      Loadouts = new string[] { "TIEB_TORP", "TIEB_ION", "TIEB_LASR" };
    }
  }
}

