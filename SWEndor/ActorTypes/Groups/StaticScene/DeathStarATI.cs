﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class DeathStarATI : Groups.StaticScene
  {
    internal DeathStarATI(Factory owner) : base(owner, "DeathStar")
    {
      ExplodeData = new ExplodeData(deathTrigger: DeathExplosionTrigger.ALWAYS, deathExplosionType: "ExplosionMega");
      float size = 20000;

      SourceMesh = TrueVision.TVGlobals.GetMesh(Name);
      if (SourceMesh == null)
      {
        SourceMesh = TrueVision.TVScene.CreateMeshBuilder(Name);

        string texname = Path.Combine("deathstar", "deathstar.bmp");
        string alphatexname = Path.Combine("deathstar", "deathstaralpha.bmp"); 
        string texpath = Path.Combine(Globals.ImagePath, texname);
        string alphatexpath = Path.Combine(Globals.ImagePath, alphatexname); // alphatexname
        int itex = LoadAlphaTexture(texname, texpath, alphatexpath);

        SourceMesh.CreateBox(size, size, 0.001f);
        SourceMesh.SetTexture(itex);
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }

      AddOns = new AddOnInfo[] { new AddOnInfo("Death Star Laser Source", new TV_3DVECTOR(-0.13f * size, 0.2f * size, -0.04f * size), new TV_3DVECTOR(0, 0, 0), true) };
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);
      if (ainfo.ActorState.IsDying())
      {
        TimedLifeSystem.Activate(Engine, ainfo.ID, 5);
        CombatSystem.Deactivate(Engine, ainfo.ID);
      }
      else if (ainfo.ActorState.IsDead())
      {
        ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Get("Explosion Wave Mega"));
        acinfo.Position = ainfo.GetPosition();
        ActorInfo explwav = ActorInfo.Create(ActorFactory, acinfo);
        MeshSystem.SetScale(Engine, explwav.ID, 10);
      }
    }
  }
}


