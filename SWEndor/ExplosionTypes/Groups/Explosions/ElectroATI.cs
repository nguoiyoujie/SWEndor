using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Explosions;
using System;

namespace SWEndor.ExplosionTypes.Instances
{
  public class ElectroATI : Groups.Explosion
  {
    internal ElectroATI(Factory owner) : base(owner, "Electro")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 0.5f);

      RenderData.RadarSize = 0;

      atlasX = 2;
      atlasY = 2;
      MeshData = MeshDataDecorator.CreateBillboardAtlasAnimation(Name, 40, "electro/tex.jpg", atlasX, atlasY);
    }

    private static Action<ExplosionInfo> persist = (a) =>
    {
      a.SetState_Normal();
      a.DyingTimerSet(a.TypeInfo.TimedLifeData.TimedLife, true);
    };

    public override void Initialize(Engine engine, ExplosionInfo ainfo)
    {
      base.Initialize(engine, ainfo);
      ainfo.CycleInfo.Action = persist;

      ainfo.CycleInfo.CyclesRemaining = 99;
      ainfo.CycleInfo.CyclePeriod = 0.25f;
    }

    public override void ProcessState(Engine engine, ExplosionInfo ainfo)
    {
      base.ProcessState(engine, ainfo);
      ActorInfo p = engine.ActorFactory.Get(ainfo.AttachedActorID);
      if (p == null)
        ainfo.SetState_Dead();
    }
  }
}
