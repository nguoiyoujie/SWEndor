using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Transport_Box3ATI : Groups.SpinningDebris
  {
    internal Transport_Box3ATI(Factory owner) : base(owner, "Transport Box 3")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 12);

      MaxSpeed = 500;
      MinSpeed = 100;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"transport\transport_box3.x");
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      ainfo.DyingMoveComponent = new DyingSpin(100, 450);
    }
  }
}

