using MTV3D65;
using SWEndor.Actors.Data;

namespace SWEndor.ActorTypes.Instances
{
  public class Explosion2ATI : Groups.Explosion
  {
    internal Explosion2ATI(Factory owner) : base(owner, "Explosion2")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 1);

      RadarSize = 5;

      SourceMesh = TrueVision.TVGlobals.GetMesh(Name);
      if (SourceMesh == null)
      {
        LoadAlphaTextureFromFolder(Globals.ImagePath, "explosion/large");
        SourceMesh = TrueVision.TVScene.CreateBillboard(texanimframes[0], 0, 0, 0, 40, 40, Name, true);
        SourceMesh.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
        SourceMesh.SetBillboardType(CONST_TV_BILLBOARDTYPE.TV_BILLBOARD_FREEROTATION);

        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }
  }
}

