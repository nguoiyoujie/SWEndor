using MTV3D65;
using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface003_00ATI : Groups.StaticScene
  {
    internal Surface003_00ATI(Factory owner) : base(owner, "Surface003_00ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 1000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface003.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.Scale = new TV_3DVECTOR(4, 1, 4);
    }
  }
}

