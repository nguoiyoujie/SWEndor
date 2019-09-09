using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class ExpS00 : Groups.Explosion
  {
    internal ExpS00(Factory owner) : base(owner, "ExpS00")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 0.5f);

      RenderData.RadarSize = 2;

      MeshData = MeshDataDecorator.CreateBillboardAnimation(Name, 10, "explosion/small", ref texanimframes);
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.CycleInfo.CyclePeriod = 0.5f;
      PlayerCameraInfo.ProximityShake(5, 100, ainfo.Position);
    }
  }
}

