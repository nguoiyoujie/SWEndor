using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using System;

namespace SWEndor.ActorTypes.Instances
{
  public class ElectroATI : Groups.Explosion
  {
    internal ElectroATI(Factory owner) : base(owner, "Electro")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 0.5f);

      RenderData.RadarSize = 0;

      MeshData = MeshDataDecorator.CreateBillboardAnimation(Name, 40, "electro", ref texanimframes);
    }

    Action<ActorInfo> persist = (a) =>
    {
      a.SetState_Normal();
      a.DyingTimerSet(a.TypeInfo.TimedLifeData.TimedLife, true);
    };

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.CycleInfo.Action = persist;

      ainfo.CycleInfo.CyclesRemaining = 99;
      ainfo.CycleInfo.CyclePeriod = 0.25f;
    }
  }
}
