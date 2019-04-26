using MTV3D65;
using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ExplosionWaveMegaATI : Groups.Explosion
  {
    internal ExplosionWaveMegaATI(Factory owner) : base(owner, "Explosion Wave Mega")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 2f;
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = false;
      RadarSize = 0;

      EnableDistanceCull = false;


      SourceMesh = FactoryOwner.Engine.TrueVision.TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = FactoryOwner.Engine.TrueVision.TVScene.CreateMeshBuilder(Key);

        string texname = Path.Combine("explosion", "wave", @"tex0000.jpg");
        string texpath = Path.Combine(Globals.ImagePath, texname);
        int tex = LoadAlphaTexture(texname, texpath);

        SourceMesh.CreateBox(100, 0.001f, 100f);
        SourceMesh.SetTexture(tex);
        SourceMesh.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);

        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      if (ainfo.ActorState == ActorState.NORMAL)
      {
        ainfo.Scale += new TV_3DVECTOR(7500, 0, 7500) * Globals.Engine.Game.TimeSinceRender;
      }
    }
  }
}

