using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Transport_Box3ATI : Groups.SpinningDebris
  {
    internal Transport_Box3ATI(Factory owner) : base(owner, "BOX3", "Transport Box 3")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 12);

      MoveLimitData.MaxSpeed = 500;
      MoveLimitData.MinSpeed = 100;

      MeshData = new MeshData(Name, @"transport\transport_box3.x");
      DyingMoveData.Spin(100, 450);
    }
  }
}

