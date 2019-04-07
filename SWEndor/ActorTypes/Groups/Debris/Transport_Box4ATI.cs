using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class Transport_Box4ATI : SpinningDebrisGroup
  {
    private static Transport_Box4ATI _instance;
    public static Transport_Box4ATI Instance()
    {
      if (_instance == null) { _instance = new Transport_Box4ATI(); }
      return _instance;
    }

    private Transport_Box4ATI() : base("Transport Box 4")
    {
      // Combat
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = false;
      OnTimedLife = true;
      TimedLife = 12f;

      MaxSpeed = 500;
      MinSpeed = 100;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"transport\transport_box4.x");
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      ainfo.MovementInfo.D_spin_min_rate = 100;
      ainfo.MovementInfo.D_spin_max_rate = 450;
    }
  }
}

