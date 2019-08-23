using MTV3D65;
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

        SourceMesh.AddWall(itex, -size / 2, 0, size / 2, 0, size, -size / 2);
        //SourceMesh.CreateBox(size, size, 0.001f);
        SourceMesh.SetTexture(itex);
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }

      AddOns = new AddOnInfo[] { new AddOnInfo("Death Star Laser Source", new TV_3DVECTOR(-0.13f * size, 0.2f * size, -0.04f * size), new TV_3DVECTOR(0, 0, 0), true) };
    }

    public override void Dying(ActorInfo ainfo)
    {
      base.Dying(ainfo);

      ainfo.DyingTimerSet(5, true);
      CombatSystem.Deactivate(Engine, ainfo);
    }

    public override void Dead(ActorInfo ainfo)
    {
      base.Dead(ainfo);

      ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Get("Explosion Wave Mega"));
      acinfo.Position = ainfo.GetGlobalPosition();
      ActorInfo explwav = ActorFactory.Create(acinfo);
      explwav.Scale = 10;
    }
  }
}


