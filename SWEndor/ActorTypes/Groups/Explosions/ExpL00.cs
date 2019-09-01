using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class ExpL00 : Groups.Explosion
  {
    internal ExpL00(Factory owner) : base(owner, "ExpL00")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 1);

      RadarSize = 5;
      EnableDistanceCull = false;


      SourceMesh = TrueVision.TVGlobals.GetMesh(Name);
      if (SourceMesh == null)
      {
        LoadAlphaTextureFromFolder(Globals.ImagePath, "explosion/large");
        SourceMesh = TrueVision.TVScene.CreateBillboard(texanimframes[0], 0, 0, 0, 100, 100, Name, true);
        SourceMesh.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
        SourceMesh.SetBillboardType(CONST_TV_BILLBOARDTYPE.TV_BILLBOARD_FREEROTATION);

        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }

      InitialSoundSources = new SoundSourceInfo[] { new SoundSourceInfo("exp_resto", 500) };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      PlayerCameraInfo.ProximityShake(25, 300, ainfo.Position);
    }
  }
}

