using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;
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
          ainfo.StateModel.MakeNormal(ainfo);

          // use a different timer, like an animation timer
          ainfo.DyingTimer.Set(TimedLifeData.TimedLife).Start();
        }
        );

      ainfo.CycleInfo.CyclesRemaining = 99;
      ainfo.CycleInfo.CyclePeriod = 0.25f;
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      if (!ainfo.StateModel.IsDyingOrDead)
      {
        ActorInfo p = ainfo.Relation.Parent;
        if (p != null)
        {
          if (!p.Disposed)
          {
            TV_3DVECTOR pos = p.GetGlobalPosition();
            ainfo.Transform.Position = new TV_3DVECTOR(pos.x, pos.y, pos.z);
          }
          else
          {
            p.RemoveChild(ainfo);
          } 
        }
        else
        {
          ainfo.StateModel.MakeDead(ainfo);
        }
      }
    }
  }
}
