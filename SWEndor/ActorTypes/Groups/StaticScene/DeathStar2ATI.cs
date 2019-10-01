using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  public class DeathStar2ATI : Groups.StaticScene
  {
    int[] texanimframes;

    internal DeathStar2ATI(Factory owner) : base(owner, "DSTAR2", "DeathStar2")
    {
      Explodes = new ExplodeData[]
      {
        new ExplodeData("EXPL02", 1, 1, ExplodeTrigger.ON_DEATH),
        new ExplodeData("EXPW02", 1, 1, ExplodeTrigger.ON_DEATH),
      };

      int[] death = new int[] { 1, 10, 15, 16, 20, 21, 22, 23, 27, 28, 30, 31, 34, 35, 36 };

      MeshData = MeshDataDecorator.CreateAlphaTexturedFlickerWall(Name, 20000, "deathstar/deathstar2.bmp", "deathstar/deathstar2b.jpg", "deathstar/deathstar2alpha.bmp", 32, death, ref texanimframes);

      //AddOns = new AddOnInfo[] { new AddOnInfo("DSLSRSRC", new TV_3DVECTOR(-1300, 2000, -0.04f * size), new TV_3DVECTOR(0, 0, 0), true) };
    }

    public override void Dying(Engine engine, ActorInfo ainfo)
    {
      base.Dying(engine, ainfo);
      ainfo.DyingTimerSet(5, true);
    }

    public override void ProcessState(Engine engine, ActorInfo ainfo)
    {
      base.ProcessState(engine, ainfo);
      if (ainfo.IsDying)
      {
        int k = texanimframes.Length - 1 - (int)(ainfo.CycleInfo.CycleTime / ainfo.CycleInfo.CyclePeriod * texanimframes.Length);
        if (k >= 0 && k < texanimframes.Length)
          ainfo.SetTexture(texanimframes[k]);
      }
    }
  }
}


