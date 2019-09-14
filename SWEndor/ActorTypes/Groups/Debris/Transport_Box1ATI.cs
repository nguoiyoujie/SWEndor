using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Transport_Box1ATI : Groups.SpinningDebris
  {
    internal Transport_Box1ATI(Factory owner) : base(owner, "Transport Box 1")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 12);

      MoveLimitData.MaxSpeed = 500;
      MoveLimitData.MinSpeed = 100;

      MeshData = new MeshData(Name, @"transport\transport_box1.x");
      DyingMoveData.Spin(100, 450);
    }
  }
}

