using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ExplosionWaveMegaATI : Groups.Explosion
  {
    internal ExplosionWaveMegaATI(Factory owner) : base(owner, "Explosion Wave Mega")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 2);

      RadarSize = 0;

      EnableDistanceCull = false;


      SourceMesh = TrueVision.TVGlobals.GetMesh(Name);
      if (SourceMesh == null)
      {
        SourceMesh = TrueVision.TVScene.CreateMeshBuilder(Name);

        string texname = Path.Combine("explosion", "wave", @"tex0000.jpg");
        string texpath = Path.Combine(Globals.ImagePath, texname);
        int tex = LoadAlphaTexture(texname, texpath);

        SourceMesh.AddFloor(tex, -50, -50, 50, 50);
        //SourceMesh.CreateBox(100, 0.00001f, 100f);
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
        MeshSystem.EnlargeScale(ainfo.Engine, ainfo, 7500 * Game.TimeSinceRender);
      }
    }
  }
}

