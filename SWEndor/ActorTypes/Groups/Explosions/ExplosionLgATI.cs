using MTV3D65;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class ExplosionLgATI : Groups.Explosion
  {
    internal ExplosionLgATI(Factory owner) : base(owner, "ExplosionLg")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 1);

      RadarSize = 20;
      RadarType = RadarType.FILLED_CIRCLE_L;

      EnableDistanceCull = false;

      SourceMesh = TrueVision.TVGlobals.GetMesh(Name);
      if (SourceMesh == null)
      {
        LoadAlphaTextureFromFolder(Globals.ImagePath, "explosion/large");
        SourceMesh = TrueVision.TVScene.CreateBillboard(texanimframes[0], 0, 0, 0, 1000, 1000, Name, true);
        SourceMesh.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
        SourceMesh.SetBillboardType(CONST_TV_BILLBOARDTYPE.TV_BILLBOARD_FREEROTATION);

        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }

      InitialSoundSources = new SoundSourceInfo[] { new SoundSourceInfo("exp_nave", 3000) };
    }
  }
}

