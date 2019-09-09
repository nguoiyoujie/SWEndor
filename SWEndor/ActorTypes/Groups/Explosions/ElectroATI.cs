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

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.CycleInfo.Action = new Action(
        () => 
        {
          ainfo.SetState_Normal();
          ainfo.DyingTimerSet(TimedLifeData.TimedLife, true);
        }
        );

      ainfo.CycleInfo.CyclesRemaining = 99;
      ainfo.CycleInfo.CyclePeriod = 0.25f;
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      if (!ainfo.IsDyingOrDead)
      {
        ActorInfo p = ainfo.Parent;
        if (p != null)
        {
          if (!p.Disposed)
          {
            TV_3DVECTOR pos = p.GetGlobalPosition();
            ainfo.Position = new TV_3DVECTOR(pos.x, pos.y, pos.z);
          }
          else
          {
            p.RemoveChild(ainfo);
          } 
        }
        else
        {
          ainfo.SetState_Dead();
        }
      }
    }
  }
}
