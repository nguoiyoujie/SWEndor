using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class ExplosionSmATI : Groups.Explosion
  {
    private static ExplosionSmATI _instance;
    public static ExplosionSmATI Instance()
    {
      if (_instance == null) { _instance = new ExplosionSmATI(); }
      return _instance;
    }

    private ExplosionSmATI() : base("ExplosionSm")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 1;
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      RadarSize = 5;

      EnableDistanceCull = false;


      SourceMesh = Globals.Engine.TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        LoadAlphaTextureFromFolder(Globals.ImagePath, "explosion/large");
        SourceMesh = Globals.Engine.TVScene.CreateBillboard(texanimframes[0], 0, 0, 0, 100, 100, Key, true);
        SourceMesh.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
        SourceMesh.SetBillboardType(CONST_TV_BILLBOARDTYPE.TV_BILLBOARD_FREEROTATION);

        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }

      InitialSoundSources = new SoundSourceInfo[] { new SoundSourceInfo("exp_resto", 500) };
    }
  }
}

