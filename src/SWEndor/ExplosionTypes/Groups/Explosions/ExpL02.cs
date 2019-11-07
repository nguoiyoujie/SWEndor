using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;
using SWEndor.Sound;

namespace SWEndor.ExplosionTypes.Instances
{
  public class ExpL02 : Groups.Explosion
  {
    internal ExpL02(Factory owner) : base(owner, "EXPL02", "ExpL02")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 1);

      RenderData.RadarSize = 50;
      RenderData.RadarType = RadarType.FILLED_CIRCLE_L;
      RenderData.CullDistance = -1;

      atlasX = 4;
      atlasY = 4;
      MeshData = MeshDataDecorator.CreateBillboardAtlasAnimation(Name, 25000, "explosion/large/tex.jpg", CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, atlasX, atlasY);

      InitialSoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.ExpLg, 999999) };
    }
  }
}

