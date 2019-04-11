using MTV3D65;
using SWEndor.Actors;
using System;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class ElectroATI : ExplosionGroup
  {
    private static ElectroATI _instance;
    public static ElectroATI Instance()
    {
      if (_instance == null) { _instance = new ElectroATI(); }
      return _instance;
    }

    private ElectroATI() : base("Electro")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 0.5f;
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = false;
      RadarSize = 0;

      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        LoadAlphaTextureFromFolder(Globals.ImagePath, "electro");
        SourceMesh = Engine.Instance().TVScene.CreateBillboard(texanimframes[0], 0, 0, 0, 40, 40, Key, true);
        SourceMesh.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
        SourceMesh.SetBillboardType(CONST_TV_BILLBOARDTYPE.TV_BILLBOARD_FREEROTATION);

        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.CycleInfo.Action = new Action(() => { ainfo.ActorState = ActorState.NORMAL; ainfo.CombatInfo.TimedLife = TimedLife; });
      ainfo.CycleInfo.CyclesRemaining = 99;
      ainfo.CycleInfo.CyclePeriod = 0.25f;
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      if (ainfo.ActorState == ActorState.NORMAL)
      {
        ActorInfo p = ActorInfo.Factory.GetExact(ainfo.ParentID);
        if (p != null)
        {
          if (p.CreationState != CreationState.DISPOSED)
          {
            TV_3DVECTOR pos = p.GetPosition();
            ainfo.SetLocalPosition(pos.x, pos.y, pos.z);
          }
          else
          {
            ainfo.RemoveParent();
          } 
        }
        else
        {
          ainfo.ActorState = ActorState.DEAD;
        }
      }
    }
  }
}
