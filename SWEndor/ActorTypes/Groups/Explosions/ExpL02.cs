using MTV3D65;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class ExpL02 : Groups.Explosion
  {
    internal ExpL02(Factory owner) : base(owner, "ExpL02")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 1);

      RenderData.RadarSize = 50;
      RenderData.RadarType = RadarType.FILLED_CIRCLE_L;
      RenderData.CullDistance = -1;

      MeshData = MeshDataDecorator.CreateBillboardAnimation(Name, 25000, "explosion/large", ref texanimframes);

      InitialSoundSources = new SoundSourceData[] { new SoundSourceData("exp_nave", 999999) };
    }
  }
}

