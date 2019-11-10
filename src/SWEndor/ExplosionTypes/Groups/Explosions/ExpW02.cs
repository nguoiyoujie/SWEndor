using MTV3D65;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ExplosionTypes.Instances
{
  internal class ExpW02 : Groups.Explosion
  {
    internal ExpW02(Factory owner) : base(owner, "EXPW02", "ExpW02")
    {
      TimedLifeData = new TimedLifeData(true, 2);

      RenderData.RadarSize = 0;
      RenderData.CullDistance = -1;
      ExplRenderData = new ExplRenderData(1, 1, 7500);

      MeshData = MeshDataDecorator.CreateHorizon(Name, 50, Path.Combine("explosion", "wave", @"tex0000.jpg"), CONST_TV_BLENDINGMODE.TV_BLEND_ADDALPHA);
    }
  }
}

