using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Explosions;
using System.IO;

namespace SWEndor.ExplosionTypes.Instances
{
  public class ExpW02 : Groups.Explosion
  {
    internal ExpW02(Factory owner) : base(owner, "ExpW02")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 2);

      RenderData.RadarSize = 0;
      RenderData.CullDistance = -1;

      MeshData = MeshDataDecorator.CreateHorizon(Name, 50, Path.Combine("explosion", "wave", @"tex0000.jpg"));
    }

    public override void ProcessState(Engine engine, ExplosionInfo ainfo)
    {
      if (!ainfo.IsDyingOrDead)
        ainfo.Scale += 7500 * Game.TimeSinceRender;
    }
  }
}

