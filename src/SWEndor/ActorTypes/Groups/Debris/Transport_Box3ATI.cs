using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Transport_Box3ATI : Groups.SpinningDebris
  {
    internal Transport_Box3ATI(Factory owner) : base(owner, "BOX3", "Transport Box 3")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 12);

      MoveLimitData.MaxSpeed = 500;
      MoveLimitData.MinSpeed = 100;

      MeshData = new MeshData(Engine, Name, @"transport\transport_box3.x", 1, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Burn");
      DyingMoveData.Spin(Engine.Random, 100, 450);
    }
  }
}

