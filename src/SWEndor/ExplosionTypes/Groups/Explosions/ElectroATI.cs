using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Explosions;
using System;

namespace SWEndor.ExplosionTypes.Instances
{
  internal class ElectroATI : Groups.Explosion
  {
    internal ElectroATI(Factory owner) : base(owner, "ELECTRO", "Electro")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 0.5f);

      RenderData.RadarSize = 0;

      ExplRenderData = new ExplRenderData(2, 2, 0);
      MeshData = MeshDataDecorator.CreateBillboardAtlasAnimation(Name, 40, "electro/tex.jpg", CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, ExplRenderData.AtlasX, ExplRenderData.AtlasY);
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
