using MTV3D65;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ExplosionTypes.Instances
{
  internal class ExpW01 : Groups.Explosion
  {
    internal ExpW01(Factory owner) : base(owner, "EXPW01", "ExpW01")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 2);

      RenderData.RadarSize = 0;
      RenderData.CullDistance = -1;
      ExplRenderData = new ExplRenderData(1, 1, 100);

      MeshData = MeshDataDecorator.CreateHorizon(Name, 50, Path.Combine("explosion", "wave", @"tex0000.jpg"), CONST_TV_BLENDINGMODE.TV_BLEND_ADDALPHA);
    }
  }
}

