using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Transport_Box2ATI : Groups.SpinningDebris
  {
    internal Transport_Box2ATI(Factory owner) : base(owner, "Transport Box 2")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 12);

      MaxSpeed = 500;
      MinSpeed = 100;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"transport\transport_box2.x");
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      ainfo.DyingMoveComponent = new DyingSpin(100, 450);
    }
  }
}

