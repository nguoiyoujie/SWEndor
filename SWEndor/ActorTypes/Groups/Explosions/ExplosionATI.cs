using MTV3D65;
using SWEndor.Actors;
using System;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class ExplosionATI : ExplosionGroup
  {
    private static ExplosionATI _instance;
    public static ExplosionATI Instance()
    {
      if (_instance == null) { _instance = new ExplosionATI(); }
      return _instance;
    }

    private ExplosionATI() : base("Explosion")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 0.5f;
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = false;
      RadarSize = 2;

      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        LoadAlphaTextureFromFolder(Globals.ImagePath, "explosion/small");
        SourceMesh = Engine.Instance().TVScene.CreateBillboard(texanimframes[0], 0, 0, 0, 10, 10, Key, true);
        SourceMesh.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
        SourceMesh.SetBillboardType(CONST_TV_BILLBOARDTYPE.TV_BILLBOARD_FREEROTATION);

        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.CycleInfo.CyclePeriod = 0.5f;
    }
  }
}

