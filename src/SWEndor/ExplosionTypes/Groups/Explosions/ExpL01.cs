using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;
using SWEndor.Sound;

namespace SWEndor.ExplosionTypes.Instances
{
  internal class ExpL01 : Groups.Explosion
  {
    internal ExpL01(Factory owner) : base(owner, "EXPL01", "ExpL01")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 1);
      ShakeData = new ShakeData(80, 2000);

      RenderData.RadarSize = 20;
      RenderData.RadarType = RadarType.FILLED_CIRCLE_L;
      RenderData.CullDistance = -1;

      ExplRenderData = new ExplRenderData(4, 4, 1, 0);
      MeshData = MeshDataDecorator.CreateBillboardAtlasAnimation(Name, 1000, "explosion/large/tex.jpg", CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, ExplRenderData.AtlasX, ExplRenderData.AtlasY);

      InitialSoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.ExpLg, 3000) };
    }
  }
}

