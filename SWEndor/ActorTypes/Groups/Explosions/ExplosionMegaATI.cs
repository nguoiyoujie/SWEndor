using MTV3D65;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class ExplosionMegaATI : ExplosionGroup
  {
    private static ExplosionMegaATI _instance;
    public static ExplosionMegaATI Instance()
    {
      if (_instance == null) { _instance = new ExplosionMegaATI(); }
      return _instance;
    }

    private ExplosionMegaATI() : base("ExplosionMega")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 1;
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      RadarSize = 50;

      EnableDistanceCull = false;

      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        LoadAlphaTextureFromFolder(Globals.ImagePath, "explosion/large");
        SourceMesh = Engine.Instance().TVScene.CreateBillboard(texanimframes[0], 0, 0, 0, 25000, 25000, Key, true);
        SourceMesh.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
        SourceMesh.SetBillboardType(CONST_TV_BILLBOARDTYPE.TV_BILLBOARD_FREEROTATION);

        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }

      InitialSoundSources = new SoundSourceInfo[] { new SoundSourceInfo("exp_nave", 999999) };
    }
  }
}

