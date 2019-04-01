using MTV3D65;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class Explosion2ATI : ExplosionGroup
  {
    private static Explosion2ATI _instance;
    public static Explosion2ATI Instance()
    {
      if (_instance == null) { _instance = new Explosion2ATI(); }
      return _instance;
    }

    private Explosion2ATI() : base("Explosion2")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 1;
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = false;
      RadarSize = 5;

      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        LoadAlphaTextureFromFolder(Globals.ImagePath, "explosion/large");
        SourceMesh = Engine.Instance().TVScene.CreateBillboard(texanimframes[0], 0, 0, 0, 40, 40, Key, true);
        SourceMesh.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
        SourceMesh.SetBillboardType(CONST_TV_BILLBOARDTYPE.TV_BILLBOARD_FREEROTATION);

        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }
  }
}

