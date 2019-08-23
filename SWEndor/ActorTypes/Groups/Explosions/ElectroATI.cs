using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using System;

namespace SWEndor.ActorTypes.Instances
{
  public class ElectroATI : Groups.Explosion
  {
    internal ElectroATI(Factory owner) : base(owner, "Electro")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 0.5f);

      RadarSize = 0;

      SourceMesh = TrueVision.TVGlobals.GetMesh(Name);
      if (SourceMesh == null)
      {
        LoadAlphaTextureFromFolder(Globals.ImagePath, "electro");
        SourceMesh = TrueVision.TVScene.CreateBillboard(texanimframes[0], 0, 0, 0, 40, 40, Name, true);
        SourceMesh.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
        SourceMesh.SetBillboardType(CONST_TV_BILLBOARDTYPE.TV_BILLBOARD_FREEROTATION);

        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.CycleInfo.Action = new Action(
        () => 
        {
          ainfo.SetState_Normal();
          //ainfo.CombatSystem.onNotify(Engine, ainfo.ID, CombatEventType.TIMEACTIVATE , TimedLifeData.TimedLife);
          ainfo.DyingTimer.Set(TimedLifeData.TimedLife, true);
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
        ActorInfo p = ainfo.Relation.Parent;
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
