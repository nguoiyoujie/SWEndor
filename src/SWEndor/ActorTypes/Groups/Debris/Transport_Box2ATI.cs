using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;

namespace SWEndor.ActorTypes.Instances
{
  internal class Transport_Box2ATI : Groups.SpinningDebris
  {
    internal Transport_Box2ATI(Factory owner) : base(owner, "BOX2", "Transport Box 2")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 12);

      MoveLimitData.MaxSpeed = 500;
      MoveLimitData.MinSpeed = 100;

      MeshData = new MeshData(Engine, Name, @"transport\transport_box2.x", 1, CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, "Burn");
      DyingMoveData.Spin(Engine.Random, 100, 450);
    }

    public override void ProcessState(Engine engine, ActorInfo ainfo)
    {
      base.ProcessState(engine, ainfo);
    }
  }
}

