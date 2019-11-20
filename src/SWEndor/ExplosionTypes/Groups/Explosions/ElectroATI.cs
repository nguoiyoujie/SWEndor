using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ExplosionTypes.Instances
{
  internal class ElectroATI : Groups.Explosion
  {
    internal ElectroATI(Factory owner) : base(owner, "ELECTRO", "Electro")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 10);

      RenderData.RadarSize = 0;

      ExplRenderData = new ExplRenderData(2, 2, 0.25f, 0);
      MeshData = MeshDataDecorator.CreateBillboardAtlasAnimation(Engine, Name, 40, "electro/tex.jpg", CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA, ExplRenderData.AtlasX, ExplRenderData.AtlasY);
    }

    /*
    private static Action<ExplosionInfo> persist = (a) =>
    {
      a.SetState_Normal();
      a.DyingTimerSet(a.TypeInfo.TimedLifeData.TimedLife, true);
    };

    public override void Initialize(Engine engine, ExplosionInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.AnimInfo.Action = persist;

      ainfo.AnimInfo.CyclesRemaining = 99;
      ainfo.AnimInfo.CyclePeriod = 0.25f;
    }
    */

    /*
    public override void ProcessState(Engine engine, ExplosionInfo ainfo)
    {
      base.ProcessState(engine, ainfo);
      ActorInfo p = engine.ActorFactory.Get(ainfo.AttachedActorID);
      if (p == null)
        ainfo.SetState_Dead();
    }
    */
  }
}
