using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;

namespace SWEndor.ActorTypes.Instances
{
  public class ExpS00 : Groups.Explosion
  {
    internal ExpS00(Factory owner) : base(owner, "ExpS00")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 0.5f);

      RadarSize = 2;

      SourceMesh = TrueVision.TVGlobals.GetMesh(Name);
      if (SourceMesh == null)
      {
        LoadAlphaTextureFromFolder(Globals.ImagePath, "explosion/small");
        SourceMesh = TrueVision.TVScene.CreateBillboard(texanimframes[0], 0, 0, 0, 10, 10, Name, true);
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

