using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Transport_Box2ATI : Groups.SpinningDebris
  {
    private static Transport_Box2ATI _instance;
    public static Transport_Box2ATI Instance()
    {
      if (_instance == null) { _instance = new Transport_Box2ATI(); }
      return _instance;
    }

    private Transport_Box2ATI() : base("Transport Box 2")
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

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"transport\transport_box2.x");
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      ainfo.MovementInfo.D_spin_min_rate = 100;
      ainfo.MovementInfo.D_spin_max_rate = 450;
    }
  }
}

