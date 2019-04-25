using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Transport_Box2ATI : Groups.SpinningDebris
  {
    internal Transport_Box2ATI(Factory owner) : base(owner, "Transport Box 2")
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

