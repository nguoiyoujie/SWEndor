using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.ActorTypes.Instances
{
  public class ExplosionATI : Groups.Explosion
  {
    internal ExplosionATI(Factory owner) : base(owner, "Explosion")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 0.5f;
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = false;
      RadarSize = 2;

      SourceMesh = Globals.Engine.TrueVision.TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        LoadAlphaTextureFromFolder(Globals.ImagePath, "explosion/small");
        SourceMesh = Globals.Engine.TrueVision.TVScene.CreateBillboard(texanimframes[0], 0, 0, 0, 10, 10, Key, true);
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

