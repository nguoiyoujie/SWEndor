using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class DeathStarATI : Groups.StaticScene
  {
    internal DeathStarATI(Factory owner) : base(owner, "DeathStar")
    {
      Explodes = new ExplodeInfo[]
      {
        new ExplodeInfo("ExpL02", 1, 1, ExplodeTrigger.ON_DEATH),
        new ExplodeInfo("ExpW02", 1, 1, ExplodeTrigger.ON_DEATH),
      };

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

    public override void Dying<A1>(A1 self)
    {
      base.Dying(self);
      ActorInfo ainfo = self as ActorInfo;
      if (ainfo == null)
        return;

      ainfo.DyingTimer.Set(5).Start();
      CombatSystem.Deactivate(Engine, ainfo.ID);
    }

    public override void Dead<A1>(A1 self)
    {
      base.Dead(self);
      /*
      ActorInfo ainfo = self as ActorInfo;
      if (ainfo == null)
        return;

      ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Get("ExpW02"));
      acinfo.Position = ainfo.GetPosition();
      ActorInfo explwav = ActorFactory.Create(acinfo);
      explwav.CoordData.Scale = 10;
      */
    }
  }
}


