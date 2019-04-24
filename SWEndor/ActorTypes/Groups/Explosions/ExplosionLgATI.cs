﻿using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class ExplosionLgATI : Groups.Explosion
  {
    private static ExplosionLgATI _instance;
    public static ExplosionLgATI Instance()
    {
      if (_instance == null) { _instance = new ExplosionLgATI(); }
      return _instance;
    }

    private ExplosionLgATI() : base("ExplosionLg")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 1;
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      RadarSize = 20;

      EnableDistanceCull = false;

      SourceMesh = Globals.Engine.TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        LoadAlphaTextureFromFolder(Globals.ImagePath, "explosion/large");
        SourceMesh = Globals.Engine.TVScene.CreateBillboard(texanimframes[0], 0, 0, 0, 1000, 1000, Key, true);
        SourceMesh.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
        SourceMesh.SetBillboardType(CONST_TV_BILLBOARDTYPE.TV_BILLBOARD_FREEROTATION);

        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }

      InitialSoundSources = new SoundSourceInfo[] { new SoundSourceInfo("exp_nave", 3000) };
    }
  }
}

