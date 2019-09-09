using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ExpW01 : Groups.Explosion
  {
    internal ExpW01(Factory owner) : base(owner, "ExpW01")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 2);

      RenderData.RadarSize = 0;
      RenderData.CullDistance = -1;

      MeshData = MeshDataDecorator.CreateHorizon(Name, 50, Path.Combine("explosion", "wave", @"tex0000.jpg"));
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      if (!ainfo.IsDyingOrDead)
        ainfo.Scale += 100 * Game.TimeSinceRender;
    }
  }
}

