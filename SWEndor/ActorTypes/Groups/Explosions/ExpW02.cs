﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class ExpW02 : Groups.Explosion
  {
    internal ExpW02(Factory owner) : base(owner, "ExpW02")
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
        SourceMesh.SetCullMode(CONST_TV_CULLING.TV_DOUBLESIDED);
        SourceMesh.SetTexture(tex);
        SourceMesh.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);

        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      if (!ainfo.IsDyingOrDead)
        ainfo.Scale += 7500 * Game.TimeSinceRender;
    }
  }
}
